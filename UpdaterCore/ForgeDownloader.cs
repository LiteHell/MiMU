using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace UpdaterCore
{
    public class ForgeFileInfo
    {
        public string Extension { get; private set; }
        public string Name { get; private set; }
        public string Checksum { get; private set; }
        public string Url { get; private set; }
        internal ForgeFileInfo(string ext, string name, string checksum, string minecraftVersion, string forgeVersion)
        {
            Extension = ext;
            Name = name;
            Checksum = checksum;
            Url = $"http://files.minecraftforge.net/maven/net/minecraftforge/forge/{minecraftVersion}-{forgeVersion}/forge-{minecraftVersion}-{forgeVersion}-{name}.{ext}";
        }
    }
    public class ForgeBuildInfo
    {
        public int BuildNo { get; private set; }
        public string ForgeVersion { get; private set; }
        public string MinecraftVersion { get; private set; }
        public ForgeFileInfo WindowsInstaller
        {
            get
            {
                foreach(ForgeFileInfo file in Files)
                {
                    if (file.Name == "installer-win" && file.Extension == "exe")
                        return file;
                }
                return null;
            }
        }
        public ForgeFileInfo Installer
        {
            get
            {
                foreach (ForgeFileInfo file in Files)
                {
                    if (file.Name == "installer" && file.Extension == "jar")
                        return file;
                }
                return null;
            }
        }
        public List<ForgeFileInfo> Files { get; private set;}
        internal ForgeBuildInfo(JObject buildInfo)
        {
            BuildNo = (int)buildInfo["build"];
            MinecraftVersion = (string)buildInfo["mcversion"];
            ForgeVersion = (string)buildInfo["version"];
            Files = new List<ForgeFileInfo>();
            foreach (JArray fileInfo in buildInfo["files"] as JArray)
            {
                Files.Add(new ForgeFileInfo((string)fileInfo[0], (string)fileInfo[1], (string)fileInfo[2], MinecraftVersion, ForgeVersion));
            }
        }
    }
    public class ForgeFetcher : WebRequestable
    {
        private const string versionsJsonUrl = "http://files.minecraftforge.net/maven/net/minecraftforge/forge/json";
        private JObject versionsJson;
        public ForgeFetcher()
        {
            versionsJson = readVersionsJson();
        }
        private JObject readVersionsJson()
        {
            using (HttpWebResponse wres = CreateGetRequest(versionsJsonUrl, "application/json"))
            using (Stream str = wres.GetResponseStream())
            using (StreamReader sre = new StreamReader(str))
            {
                JsonTextReader jsonReader = new JsonTextReader(sre);
                return JObject.Load(jsonReader);
            }
        }
        public IEnumerable<int> GetForgeBuilds(string minecraftVersion)
        {
            JArray buildsToken = versionsJson["mcversion"][minecraftVersion] as JArray;
            List<int> builds = new List<int>(buildsToken.Select(token => (int)token));
            builds.Sort();
            return builds;
        }
        public ForgeBuildInfo GetForgeInfo(int buildNo)
        {
            JObject buildInfo = versionsJson["number"][buildNo.ToString()] as JObject;
            return new ForgeBuildInfo(buildInfo);
        }
    }
}
