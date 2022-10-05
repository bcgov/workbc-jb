using System;
using System.Collections.Generic;

namespace WorkBC.Shared.SystemSettings
{
    public class Settings
    {
        public EmailSettings Email;
        public JbAccountSettings JbAccount;
        public JbSearchSettings JbSearch;
        public JbLibSettings Shared;

        public Settings(Dictionary<string, string> settings)
        {
            Email = new EmailSettings(settings);
            JbAccount = new JbAccountSettings(settings);
            Shared = new JbLibSettings(settings);
            JbSearch = new JbSearchSettings(settings);
        }

        // Cache Timestamp. Used for debugging and singleton invalidation
        public DateTime CacheTimestamp { get; set; }
        public DateTime SingletonCreated { get; set; }
    }
}