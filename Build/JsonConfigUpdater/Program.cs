using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonConfigUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DirectoryInfo artifactsRoot = GetArtifactsDirectory();

            if (artifactsRoot == null || !Directory.Exists(artifactsRoot.FullName + "\\Build"))
            {
                Console.WriteLine("UNKNOWN ERROR: repoRoot IS INVALID");
                return;
            }

            CommandLineOptions options = null;

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => { options = o; })
                .WithNotParsed(o => { Environment.Exit(0); });

            Console.WriteLine("Artifacts Root: " + artifactsRoot.FullName);

            //get the list of files
            IEnumerable<string> settingsFiles = ReadConfigList("Folders");
            IEnumerable<string> zippedSettingsFiles = ReadConfigList("ZipFiles");

            foreach (string file in settingsFiles)
            {
                UpdateJsonFile(artifactsRoot, file + "\\appsettings.json", false, options);
            }

            foreach (string file in zippedSettingsFiles)
            {
                UpdateJsonFile(artifactsRoot, file, true, options);
            }
        }

        /// <summary>
        ///     Updates an appsettings.json file in a published folder or a zip package
        /// </summary>
        private static void UpdateJsonFile(DirectoryInfo artifactsRoot, string file, bool isZip, CommandLineOptions options)
        {
            string jsonText = isZip
                ? ReadFromZip(artifactsRoot, file)
                : ReadFromDirectory(artifactsRoot, file);

            IDictionary env = Environment.GetEnvironmentVariables();

            var appSettings = JsonConvert.DeserializeObject<dynamic>(jsonText);

            dynamic tokens = AllTokens(appSettings);

            foreach (JToken token in tokens)
            {
                if (!token.HasValues)
                {
                    // environment variables are uppercase. Periods are not valid so underscores are used instead
                    string variableName = token.Path.Replace(".", "_").ToUpper();

                    // ReSharper disable once UseStringInterpolation
                    Console.Write(string.Format("  {0}{1}", token.Path, new string(' ', 60)).Substring(0, 60));

                    // if there is an environment variable whose name matches an item in the Json then update the Json item value
                    if (env.Contains(variableName))
                    {
                        var envValue = env[variableName].ToString();
                        UpdateAppSettings(appSettings, token.Path, envValue);
                        Console.WriteLine("[X]");
                    }
                    else if (variableName == "KEYCLOAK_CLIENTSECRET" && !string.IsNullOrEmpty(options.KeycloakClientSecret))
                    {
                        UpdateAppSettings(appSettings, token.Path, options.KeycloakClientSecret);
                        Console.WriteLine("[X]");
                    }
                    else if (variableName == "VERSION_RELEASENAME" && !string.IsNullOrEmpty(options.ReleaseName))
                    {
                        UpdateAppSettings(appSettings, token.Path, options.ReleaseName);
                        Console.WriteLine("[X]");
                    }
                    else if (variableName == "VERSION_RELEASEDATE")
                    {
                        UpdateAppSettings(appSettings, token.Path, DateTime.Now.ToString("yyyy-MM-dd h:mm tt"));
                        Console.WriteLine("[X]");
                    }
                    else
                    {
                        // no match found
                        Console.WriteLine("[ ]");
                    }
                }
            }

            // convert the object back to Json and save it to disk
            jsonText = JsonConvert.SerializeObject(appSettings, Formatting.Indented);

            if (isZip)
            {
                // save the file back to the zip
                WriteToZip(artifactsRoot, file, jsonText);
            }
            else
            {
                // save the file to the disk
                WriteToDirectory(artifactsRoot, file, jsonText);
            }

            //blank line between files
            Console.WriteLine();
        }

        /// <summary>
        ///     Writes a file to a zip on the disk
        /// </summary>
        private static void WriteToZip(DirectoryInfo artifactsRoot, string file, string jsonText)
        {
            string[] tokens = file.Split('\\');
            string zipName = tokens[0];
            string fileToSave = file.Replace(zipName + "\\", "");
            string tempRoot = artifactsRoot.FullName + "\\Temp";

            // create a top level temp directory if it doesn't exist
            if (!Directory.Exists(tempRoot))
            {
                Directory.CreateDirectory(tempRoot);
            }

            // create a temp directory for the individual zip if it doesn't exist
            string tempDirectory = tempRoot + "\\" + zipName;

            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }

            // save the file in a temporary folder
            string tempFile = "Temp\\" + zipName + "\\appsettings.json";

            WriteToDirectory(artifactsRoot, tempFile, jsonText);

            using (ZipFile zip = ZipFile.Read(artifactsRoot.FullName + "\\" + zipName))
            {
                foreach (ZipEntry e in zip)
                {
                    // check if you want to extract e or not
                    if (e.FileName.ToLower().EndsWith("/appsettings.json"))
                    {
                        string path = Regex.Replace(e.FileName, "/appsettings.json", "", RegexOptions.IgnoreCase);
                        zip.UpdateFile(artifactsRoot.FullName + "\\" + tempFile, path);
                        zip.Save();
                        return;
                    }
                }
            }
        }

        /// <summary>
        ///     Reads a file from a zip on the disk
        /// </summary>
        private static string ReadFromZip(DirectoryInfo artifactsRoot, string zipName)
        {
            Console.WriteLine("Updating appsettings.json in " + zipName);

            using (ZipFile zip = ZipFile.Read(artifactsRoot.FullName + "\\" + zipName))
            {
                foreach (ZipEntry e in zip)
                {
                    // check if you want to extract e or not
                    if (e.FileName.ToLower().EndsWith("/appsettings.json"))
                    {
                        using (var reader = new MemoryStream())
                        {
                            e.Extract(reader);
                            reader.Position = 0;
                            var stringReader = new StreamReader(reader);
                            return stringReader.ReadToEnd();
                        }
                    }
                }
            }

            return "{\"ReadFromZip\":\"Failed for " + zipName + "\"}";
        }

        /// <summary>
        ///     Writes a file to a directory on the disk
        /// </summary>
        private static void WriteToDirectory(DirectoryInfo artifactsRoot, string file, string jsonText)
        {
            string fullPath = string.Format("{0}\\{1}", artifactsRoot.FullName, file);
            File.WriteAllText(fullPath, jsonText);
        }

        /// <summary>
        ///     Reads a file from a directory on the disk
        /// </summary>
        private static string ReadFromDirectory(DirectoryInfo artifactsRoot, string file)
        {
            string fullPath = string.Format("{0}\\{1}", artifactsRoot.FullName, file);
            Console.WriteLine("Updating " + fullPath);
            return File.ReadAllText(fullPath);
        }

        /// <summary>
        ///     Updates a value n the appSettings objects
        /// </summary>
        private static void UpdateAppSettings(dynamic appSettings, string jsonPath, string val)
        {
            string[] t = jsonPath.Split('.');

            switch (t.Length)
            {
                case 1:
                    appSettings[t[0]] = val;
                    break;
                case 2:
                    appSettings[t[0]][t[1]] = val;
                    break;
                case 3:
                    appSettings[t[0]][t[1]][t[2]] = val;
                    break;
                case 4:
                    appSettings[t[0]][t[1]][t[2]][t[3]] = val;
                    break;
                case 5:
                    appSettings[t[0]][t[1]][t[2]][t[3]][t[4]] = val;
                    break;
                case 6:
                    appSettings[t[0]][t[1]][t[2]][t[3]][t[4]][t[5]] = val;
                    break;
                case 7:
                    appSettings[t[0]][t[1]][t[2]][t[3]][t[4]][t[5]][t[6]] = val;
                    break;
                case 8:
                    appSettings[t[0]][t[1]][t[2]][t[3]][t[4]][t[5]][t[6]][t[7]] = val;
                    break;
                case 9:
                    appSettings[t[0]][t[1]][t[2]][t[3]][t[4]][t[5]][t[6]][t[7]][t[8]] = val;
                    break;
                case 10:
                    appSettings[t[0]][t[1]][t[2]][t[3]][t[4]][t[5]][t[6]][t[7]][t[8]][t[9]] = val;
                    break;
                default:
                    throw new IndexOutOfRangeException("JSON depth exceeds the limit of 10 levels");
            }
        }

        /// <summary>
        ///     Gets a list of all the tokens in a dynamic object
        /// </summary>
        private static IEnumerable<JToken> AllTokens(JObject obj)
        {
            var toSearch = new Stack<JToken>(obj.Children());
            while (toSearch.Count > 0)
            {
                JToken inspected = toSearch.Pop();
                yield return inspected;
                foreach (JToken child in inspected)
                {
                    toSearch.Push(child);
                }
            }
        }

        /// <summary>
        ///     Gets the path to the artifacts directory (for creating absolute paths)
        /// </summary>
        private static DirectoryInfo GetArtifactsDirectory()
        {
            string directoryPath = Directory.GetCurrentDirectory();
            var currentDirectory = new DirectoryInfo(directoryPath);

            Console.WriteLine("Current Directory:" + directoryPath);

            // walk up the directory tree until we find a folder called "Build"
            while (currentDirectory != null && currentDirectory.Name != "Build" &&
                   currentDirectory != currentDirectory.Root)
            {
                currentDirectory = currentDirectory.Parent;
            }

            // once we get to "Build", step up one more level
            if (currentDirectory != null && currentDirectory.Name == "Build")
            {
                currentDirectory = currentDirectory.Parent;
            }

            return currentDirectory;
        }

        /// <summary>
        ///     Gets the list of paths from App.config
        /// </summary>
        private static IEnumerable<string> ReadConfigList(string appSettingName)
        {
            // get the list of files from App.config
            string config = ConfigurationManager.AppSettings.Get(appSettingName);

            if (config == null)
            {
                return new string[] { };
            }

            // split the list on commas.  Stripping out extra whitespace
            return config.Split(',')
                .Select(fileName => Regex.Replace(fileName, @"\s+", string.Empty));
        }
    }
}