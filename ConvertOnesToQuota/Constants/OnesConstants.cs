using System.Collections.Generic;

namespace ConvertOnesToQuota.Constants
{
    public static class OnesConstants
    {
        public const double DefaultTheatricalOnesValue = 1.0D;
        public const double DefaultEpisodicOnesValue = 0.2D;

        public const string ShowOnesExtractDataType = "S";

        public const int RejectedOnesStatusId = 4;
        public const int DeletedOnesStatusId = 5;

        public static readonly List<int> IgnoreOnesStatuses = new List<int>
        {
            RejectedOnesStatusId,
            DeletedOnesStatusId
        };

        public const string HolidayOnesType = "H";
        public const string LeavingOnesType = "L";

        public static readonly List<string> IgnoredOnesTypes = new List<string>
        {
            HolidayOnesType,
            LeavingOnesType
        };
    }
}
