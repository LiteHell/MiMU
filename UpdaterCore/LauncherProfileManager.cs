using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UpdaterCore
{
    public struct ForgeVersionId
    {
        public string MinecraftVersion { get; private set; }
        public string ForgeVersion { get; private set; }
        public string VersionId { get; private set; }
        internal ForgeVersionId(string m, string f, string v)
        {
            MinecraftVersion = m;
            ForgeVersion = f;
            VersionId = v;
        }
    }
    public class LauncherProfileManager
    {
        private static string profilesJsonPath = Environment.ExpandEnvironmentVariables("%AppData%\\.minecraft\\launcher_profiles.json");
        private JObject launcherProfiles;
        public static bool ProfilesExists
        {
            get
            {
                return File.Exists(profilesJsonPath);
            }
        }
        private void readProfiles()
        {
            using (FileStream fstr = new FileStream(profilesJsonPath, FileMode.Open))
            using (StreamReader sre = new StreamReader(fstr))
            {
                launcherProfiles = JObject.Load(new JsonTextReader(sre));
            }
        }
        public LauncherProfileManager()
        {
            readProfiles();
        }
        // this method don't return vanilla
        public IEnumerable<string> GetVersionIds()
        {
            DirectoryInfo dinfo = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%AppData%\\.minecraft\\versions"));
            foreach (DirectoryInfo subdir in dinfo.GetDirectories())
            {
                if (subdir.GetFiles("*.json").Length == 0) continue;
                FileInfo finfo = subdir.GetFiles("*.json")[0];
                JObject obj = JObject.Parse(File.ReadAllText(finfo.FullName));
                yield return (string)obj["id"];
            }
        }
        public void AddOrAlterProfile(string profileId, LauncherProfile newProfile)
        {
            readProfiles();
            JObject jobj = new JObject();
            jobj["type"] = "custom";
            if (newProfile.name != "") jobj["name"] = newProfile.name;
            if (newProfile.created != "") jobj["created"] = newProfile.created;
            if (newProfile.lastUsed != "") jobj["lastUsed"] = newProfile.lastUsed;
            if (newProfile.lastVersionId != "") jobj["lastVersionId"] = newProfile.lastVersionId;
            if (newProfile.gameDir != "") jobj["gameDir"] = newProfile.gameDir;
            if (newProfile.javaArgs != "") jobj["javaArgs"] = newProfile.javaArgs;
            launcherProfiles["profiles"][profileId] = jobj;
            using (FileStream fstr = new FileStream(profilesJsonPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fstr, new UTF8Encoding(false))) // BOM 있으면 100% 오류남
            {
                sw.Write(launcherProfiles.ToString());
            }
        }
    }
    public class LauncherProfile
    {
        private string _gameDir;
        public string name { get; set; }
        public string created { get; private set; } = "";
        public string lastUsed { get; private set; } = "";
        public string lastVersionId { get; set; } = "";
        public string gameDir
        {
            get { return _gameDir; }
            set
            {
                System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex("^([a-zA-Z]):\\\\", System.Text.RegularExpressions.RegexOptions.Compiled);
                _gameDir = pattern.Replace(value, "$1:/");
            }
        }
        public string javaArgs { get; set; } = "";
        public void SetCreatedAt(DateTime time)
        {
            created = time.ToUniversalTime().ToString("o");
        }
        public void SetLastUsedAt(DateTime time)
        {
            lastUsed = time.ToUniversalTime().ToString("o");
        }
    }
}
