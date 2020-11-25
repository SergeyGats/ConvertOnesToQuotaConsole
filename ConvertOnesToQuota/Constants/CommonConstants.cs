using System.Collections.Generic;

namespace ConvertOnesToQuota.Constants
{
    public static class CommonConstants
    {
        public const int ShowOnesMasterScenarioId = 0;
        public const int OnesMetricTypeId = 3;
        public const string MasterScenarioName = "Master";
        public const int EpisodicShowCategoryId = 2;
        public const int GlobalResourceManagerUserTypeId = 3;
        public const int UndefinedArtistLevelId = 0;

        public static readonly Dictionary<int, string> BusinessUnits = new Dictionary<int, string>
        {
            {2,"MPC Film"},
            {1001,"Technicolor India"},
            {2001,"Mill Film"},
            {3001,"Trace-old"},
            {4001,"Trace-DU"},
            {5001,"MR X"},
            {7001, "MPC Episodic"},
            {8001,"Technicolor Los Angeles"},
            {9001,"FEV Management and Central"}
        };

        public static readonly List<string> IgnoreDisciplineNames = new List<string>
        {
            "3D DMP",
            "Assets",
            "Indirect Dept"
        };

        public const string ClearAllQuotaTablesScriptText =
            "DELETE FROM dwh.ShowOnesQuota " +
            "DELETE FROM dbo.Milestones " +
            "DELETE FROM dwh.ShowOnesQuotaUniqueFilter " +
            "DELETE FROM dwh.ShowOnesQuotaScenario " +
            "DBCC CHECKIDENT('dwh.ShowOnesQuota', RESEED, 0) " +
            "DBCC CHECKIDENT('dbo.Milestones', RESEED, 0) " +
            "DBCC CHECKIDENT('dwh.ShowOnesQuotaUniqueFilter', RESEED, 0) " +
            "DBCC CHECKIDENT('dwh.ShowOnesQuotaScenario', RESEED, 0)";
    }
}
