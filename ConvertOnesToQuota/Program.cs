using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Constants;
using ConvertOnesToQuota.Filters;
using ConvertOnesToQuota.Models;
using ConvertOnesToQuota.Repositories;

namespace ConvertOnesToQuota
{
    internal class Program
    {
        private static readonly ApplicationDatabaseContext _context = new ApplicationDatabaseContext();

        private static readonly ArtistLevelRepository _artistLevelRepository = new ArtistLevelRepository(_context);
        private static readonly EntityTpsRelationRepository _entityTpsRelationRepository = new EntityTpsRelationRepository(_context);
        private static readonly OnesRepository _onesRepository = new OnesRepository(_context);
        private static readonly ShowFacilityRepository _showFacilityRepository = new ShowFacilityRepository(_context);
        private static readonly ShowOnesQuotaScenarioRepository _quotaScenarioRepository = new ShowOnesQuotaScenarioRepository(_context);
        private static readonly ShowRepository _showRepository = new ShowRepository(_context);
        private static readonly UserProfileRepository _userProfileRepository = new UserProfileRepository(_context);

        private static void Main(string[] args)
        {
            //Console.WriteLine("Warning. All records in quota tables will be deleted.");
            //_context.Database.ExecuteSqlCommand(CommonConstants.ClearAllQuotaTablesScriptText);

            while (true)
            {
                Console.WriteLine("Business units:");
                foreach (var businessUnit in CommonConstants.BusinessUnits)
                {
                    Console.WriteLine($"{businessUnit.Value,-30}{businessUnit.Key,5}");
                }

                Console.WriteLine("\nWrite the business unit id and press enter:");
                int buId;

                while (!int.TryParse(Console.ReadLine(), out buId) || !CommonConstants.BusinessUnits.ContainsKey(buId))
                {
                    Console.Write("This is not valid input. Please enter business unit id:\n");
                }

                _quotaScenarioRepository.DeleteQuotaScenariosForBusinessUnit(buId);

                Console.WriteLine("\nGetting shows");
                var shows = _showRepository.GetShowsByBuId(buId);

                Console.WriteLine("Getting disciplines");
                var disciplines = GetDisciplinesForBusinessUnitIncludingTps(buId);

                Console.WriteLine("Getting artist levels\n");
                var artistLevelIds = _artistLevelRepository.GetArtistLevels().Select(l => l.Id).ToList();

                var buIds = _entityTpsRelationRepository.GetEntityTpsRelations(buId).Select(x => x.TpsEntityId).ToList();
                buIds.Add(buId);

                var userId = _userProfileRepository
                    .GetItemAsQueryable(u =>
                        !u.Deleted && u.Active && u.BuId == buId && u.UserTypeId == CommonConstants.GlobalResourceManagerUserTypeId)
                    .Select(u => u.UserId)
                    .First();

                var showsCount = shows.Count;
                var i = 1;

                foreach (var show in shows)
                {
                    try
                    {
                        var showFacilities = _showFacilityRepository.GetShowFacilities(show.ShowId).ToList();
                        var showFacilityIds = showFacilities.Select(x => x.EntityId).ToList();
                        var disciplineIds = disciplines.Select(x => x.Id).ToList();

                        if (show.ProducerUserId.HasValue)
                        {
                            userId = show.ProducerUserId.Value;
                        }

                        var filter = new OnesForMasterScenarioFilter
                        {
                            ShowOnesScenarioId = CommonConstants.ShowOnesMasterScenarioId,
                            ShowId = show.ShowId,
                            BuIds = buIds,
                            ArtistLevelIds = artistLevelIds,
                            SiteIds = showFacilityIds,
                            DisciplineIds = disciplineIds
                        };

                        var ones = _onesRepository.GetOnesForMasterScenario(filter).ToList();

                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                var quotaScenario = new ShowOnesQuotaScenario
                                {
                                    ShowId = show.ShowId,
                                    UserId = userId,
                                    Name = CommonConstants.MasterScenarioName,
                                    CreatedDateTimeUtc = DateTime.UtcNow,
                                    IsMaster = true
                                };

                                var dbQuotaScenarioId = _quotaScenarioRepository.Save(quotaScenario);

                                var uniqueFilters = CreateUniqueFilters(dbQuotaScenarioId, disciplines, showFacilities);
                                _context.BulkInsert(uniqueFilters);

                                var isEpisodicShow = show.ShowCategoryId == CommonConstants.EpisodicShowCategoryId;
                                List<ShowOnesQuota> levelQuotas;

                                if (isEpisodicShow)
                                {
                                    levelQuotas = CreateLevelQuotasFromDailyUnassignedOnes(ones, uniqueFilters, dbQuotaScenarioId);
                                }
                                else
                                {
                                    levelQuotas = CreateLevelQuotasFromWeeklyUnassignedOnes(ones, uniqueFilters, dbQuotaScenarioId);
                                }

                                _context.BulkInsert(levelQuotas);

                                var siteQuotas = CreateSiteQuotas(levelQuotas);
                                _context.BulkInsert(siteQuotas);

                                transaction.Commit();
                                //transaction.Rollback();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Console.WriteLine($"Show code:{show.ShowCode}\n" +
                                                  $"Error message: {ex.Message}\n" +
                                                  $"Inner exception: {ex.InnerException}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Show code:{show.ShowCode}\n" +
                                          $"Error message: {ex.Message}\n" +
                                          $"Inner exception: {ex.InnerException}");
                    }

                    var progressValue = (double)i / showsCount * 100;
                    var completedPercent = Math.Round(progressValue, 1, MidpointRounding.AwayFromZero);
                    Console.WriteLine($"Convert completed on: {completedPercent}%");

                    i++;
                }

                Console.WriteLine($"Finish converting for {CommonConstants.BusinessUnits.First(bu => bu.Key == buId).Value}\n");
            }
        }

        private static List<ShowOnesQuota> CreateLevelQuotasFromDailyUnassignedOnes(List<Ones> onesCollection,
            List<ShowOnesQuotaUniqueFilter> uniqueFilters,
            int scenarioId)
        {
            var showOnesQuotas = new List<ShowOnesQuota>();

            var groupsOnes = onesCollection
                .Where(o => o.DayOfPeriod != null)
                .GroupBy(o => new { o.Artist.LevelId, DayOfPeriod = o.DayOfPeriod.Value, o.DisciplineId, o.SiteEntityId });

            foreach (var groupOnes in groupsOnes)
            {
                var sum = 0D;
                var siteId = groupOnes.First().SiteEntityId;
                var disciplineId = groupOnes.First().DisciplineId;
                var seniorityLevelId = groupOnes.First().Artist.LevelId;
                var date = groupOnes.Key.DayOfPeriod;

                foreach (var ones in groupOnes)
                {
                    sum += GetSumOnesValueAndOt(ones);
                }

                if (sum > 0)
                {
                    var uniqueFilter = uniqueFilters.FirstOrDefault(f => f.DisciplineId == disciplineId &&
                        f.SiteId == siteId &&
                        f.ShowOnesQuotaScenarioId == scenarioId);

                    if (uniqueFilter != null)
                    {
                        var showOnesQuota = new ShowOnesQuota
                        {
                            ShowOnesQuotaUniqueFilterId = uniqueFilter.Id,
                            SeniorityLevelId = seniorityLevelId ?? CommonConstants.UndefinedArtistLevelId,
                            MetricTypeId = CommonConstants.OnesMetricTypeId,
                            Value = Convert.ToDecimal(sum),
                            Date = date
                        };

                        showOnesQuotas.Add(showOnesQuota);
                    }
                }
            }

            return showOnesQuotas;
        }

        private static List<ShowOnesQuota> CreateLevelQuotasFromWeeklyUnassignedOnes(List<Ones> onesCollection,
            List<ShowOnesQuotaUniqueFilter> uniqueFilters,
            int scenarioId)
        {
            var showOnesQuotas = new List<ShowOnesQuota>();

            var groupsOnes = onesCollection
                .Where(o => o.DayOfPeriod == null)
                .GroupBy(o => new { o.Artist.LevelId, o.Period, o.DisciplineId, o.SiteEntityId });

            foreach (var groupOnes in groupsOnes)
            {
                var sum = 0D;
                var siteId = groupOnes.First().SiteEntityId;
                var disciplineId = groupOnes.First().DisciplineId;
                var seniorityLevelId = groupOnes.First().Artist.LevelId;

                var date = groupOnes.Key.Period;

                foreach (var ones in groupOnes)
                {
                    sum += GetSumOnesValueAndOt(ones);
                }

                if (sum > 0)
                {
                    var uniqueFilter = uniqueFilters.FirstOrDefault(f => f.DisciplineId == disciplineId &&
                        f.SiteId == siteId &&
                        f.ShowOnesQuotaScenarioId == scenarioId);

                    if (uniqueFilter != null)
                    {
                        var value = Math.Round(sum, 1, MidpointRounding.AwayFromZero);

                        var showOnesQuota = new ShowOnesQuota
                        {
                            ShowOnesQuotaUniqueFilterId = uniqueFilter.Id,
                            SeniorityLevelId = seniorityLevelId ?? CommonConstants.UndefinedArtistLevelId,
                            MetricTypeId = CommonConstants.OnesMetricTypeId,
                            Value = Convert.ToDecimal(value),
                            Date = date
                        };

                        showOnesQuotas.Add(showOnesQuota);
                    }
                }
            }

            return showOnesQuotas;
        }

        private static List<ShowOnesQuota> CreateSiteQuotas(List<ShowOnesQuota> quotas)
        {
            var showOnesQuotas = new List<ShowOnesQuota>();

            var groupsQuotas = quotas
                .GroupBy(q => new
                {
                    q.Date,
                    q.ShowOnesQuotaUniqueFilterId
                });

            foreach (var groupQuotas in groupsQuotas)
            {
                var uniqueFilterId = groupQuotas.First().ShowOnesQuotaUniqueFilterId;
                var seniorityLevelId = CommonConstants.UndefinedArtistLevelId;
                var date = groupQuotas.Key.Date;
                var sum = groupQuotas.Sum(q => q.Value);
                var value = Math.Round(sum, 1, MidpointRounding.AwayFromZero);

                if (sum != 0)
                {
                    var showOnesQuota = new ShowOnesQuota
                    {
                        ShowOnesQuotaUniqueFilterId = uniqueFilterId,
                        SeniorityLevelId = seniorityLevelId,
                        MetricTypeId = CommonConstants.OnesMetricTypeId,
                        Value = value,
                        Date = date
                    };

                    showOnesQuotas.Add(showOnesQuota);
                }
            }

            return showOnesQuotas;
        }

        private static List<Discipline> GetDisciplinesForBusinessUnitIncludingTps(int buId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@BUID", buId),
            };

            var dataTable = _context.ExecuteStoredProc("GetDisciplinesByBusiness", parameters);

            var disciplines = dataTable.AsEnumerable()
                .Select(s => new Discipline
                {
                    Id = s.Field<int>("DisciplineId"),
                    Name = s.Field<string>("DisciplineName"),
                    IsIdl = s.Field<bool>("IsIDL"),
                    BuId = s.Field<int>("BuId")
                })
                .Where(x => !CommonConstants.IgnoreDisciplineNames.Contains(x.Name))
                .Where(x => !x.IsIdl)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .ToList();

            return disciplines;
        }

        private static List<ShowOnesQuotaUniqueFilter> CreateUniqueFilters(int scenarioId, List<Discipline> disciplines,
            List<ShowFacility> showFacilities)
        {
            var uniqueFilters = new List<ShowOnesQuotaUniqueFilter>();

            foreach (var discipline in disciplines)
            {
                foreach (var showFacility in showFacilities)
                {
                    if (showFacility.FacilityBuId == discipline.BuId)
                    {
                        var uniqueFilter = new ShowOnesQuotaUniqueFilter
                        {
                            SiteId = showFacility.EntityId,
                            DisciplineId = discipline.Id,
                            ShowOnesQuotaScenarioId = scenarioId
                        };

                        uniqueFilters.Add(uniqueFilter);
                    }
                }
            }

            return uniqueFilters;
        }

        private static double GetSumOnesValueAndOt(Ones ones)
        {
            var sum = 0D;

            if (ones.Value != null)
            {
                var value = ones.Value.Value / (ones.IsEpisodic
                    ? OnesConstants.DefaultEpisodicOnesValue
                    : OnesConstants.DefaultTheatricalOnesValue);

                sum += value;
            }

            if (ones.OverTime != null)
            {
                var ot = ones.OverTime.Value / (ones.IsEpisodic
                    ? OnesConstants.DefaultEpisodicOnesValue
                    : OnesConstants.DefaultTheatricalOnesValue);

                sum += ot;
            }

            return sum;
        }
    }
}