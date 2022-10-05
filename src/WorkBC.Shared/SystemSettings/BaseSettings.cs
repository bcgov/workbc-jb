using System.Collections.Generic;

namespace WorkBC.Shared.SystemSettings
{
    public class BaseSettings
    {
        protected string GetSetting(Dictionary<string, string> settings, string key)
        {
            return settings.ContainsKey(key) ? settings[key] : "";
        }
    }
}