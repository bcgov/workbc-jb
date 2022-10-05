using System.IO;
using System.Linq;
using System.Reflection;

namespace WorkBC.Shared.Utilities
{
    public static class ResourceFileHelper
    {
        public static string ReadFile(string filename)
        {
            Assembly assembly;
            string resource;

            try
            {
                assembly = Assembly.GetExecutingAssembly();
                resource = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));
            }
            catch
            {
                assembly = Assembly.GetCallingAssembly();
                resource = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));
            }

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Error loading resource " + filename);
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}