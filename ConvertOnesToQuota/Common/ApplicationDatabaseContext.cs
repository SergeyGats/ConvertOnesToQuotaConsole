using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Common
{
    public class ApplicationDatabaseContext : DbContext
    {
        public ApplicationDatabaseContext()
            : base("MpcContext")
        {

        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtistLevel> ArtistLevels { get; set; }
        public DbSet<EntityTpsRelation> EntityTpsRelations { get; set; }
        public DbSet<Ones> Ones { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowFacility> ShowFacilities { get; set; }
        public DbSet<ShowOnesQuota> ShowOnesQuotas { get; set; }
        public DbSet<ShowOnesQuotaScenario> ShowOnesQuotaScenarios { get; set; }
        public DbSet<ShowOnesQuotaUniqueFilter> ShowOnesQuotaUniqueFilters { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DataTable ExecuteStoredProc(
            string storedProc,
            List<SqlParameter> pv,
            int retryCount = 0,
            int retryThreadSleep = 1000)
        {
            var dataSet = new DataSet();
            var connection = Database.Connection;
            var flag1 = connection.State == ConnectionState.Closed;

            try
            {
                if (flag1)
                {
                    connection.Open();
                }

                var flag2 = true;
                var selectCommand = (SqlCommand)null;
                while (flag2)
                {
                    flag2 = false;
                    try
                    {
                        var str = storedProc;

                        if (!storedProc.ToLower().StartsWith("select "))
                        {
                            str = "exec " + str;
                        }

                        selectCommand = new SqlCommand();
                        selectCommand.Connection = (SqlConnection)connection;

                        if (Database.CurrentTransaction != null)
                        {
                            selectCommand.Transaction =
                                (SqlTransaction) Database.CurrentTransaction.UnderlyingTransaction;
                        }

                        if (pv != null && pv.Count > 0)
                        {
                            var num = 0;
                            foreach (var sqlParameter in pv)
                            {
                                if (!sqlParameter.ParameterName.StartsWith("@"))
                                {
                                    sqlParameter.ParameterName = "@" + sqlParameter.ParameterName;
                                }

                                if (sqlParameter.Value == null)
                                {
                                    sqlParameter.Value = DBNull.Value;
                                }

                                if (sqlParameter.SqlDbType == SqlDbType.DateTime &&
                                    sqlParameter.Value.Equals(0))
                                {
                                    sqlParameter.Value = DBNull.Value;
                                }

                                str += num > 0 ? ", " : " ";
                                str += sqlParameter.ParameterName;
                                selectCommand.Parameters.Add(sqlParameter);
                                ++num;
                            }
                        }
                        selectCommand.CommandText = str;
                        new SqlDataAdapter(selectCommand).Fill(dataSet);
                    }
                    catch (SqlException ex)
                    {
                        if (retryCount > 0)
                        {
                            --retryCount;
                            flag2 = true;
                            if (selectCommand != null && selectCommand.Parameters.Count > 0)
                            {
                                selectCommand.Parameters.Clear();
                                Thread.Sleep(retryThreadSleep);
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            finally
            {
                if (flag1)
                {
                    connection.Close();
                }
            }
            return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : (DataTable)null;
        }
    }
}
