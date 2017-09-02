using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;

namespace MiMU
{
    public static class Settings
    {
        private static IniData iniFile;
        private static FileIniDataParser iniFileParser = new FileIniDataParser();
        private static string settingsPath = Path.Combine(Environment.CurrentDirectory, "mimu.ini");
        static Settings()
        {
            iniFile = iniFileParser.ReadFile(settingsPath, Encoding.UTF8);
        }
        private static void writeSetting(string section, string keyName, string keyValue)
        {
            iniFile[section][keyName] = keyValue;
            iniFileParser.WriteFile(settingsPath, iniFile, Encoding.UTF8);
        }
        private static string readSetting(string section, string keyName, string defaultValue)
        {
            return iniFile[section][keyName] ?? defaultValue;
        }
        private static void writeUserSetting(string keyName, string keyValue)
        {
            writeSetting("User", keyName, keyValue);
        }
        private static string readUserSetting(string keyName, string defaultValue)
        {
            return readSetting("User", keyName, defaultValue);
        }
        private static string readServerSetting(string keyName, string defaultValue)
        {
            return readSetting("Server", keyName, defaultValue);
        }
        // User Settings
        public static string MinecraftJavaArgs { get { return readUserSetting("MinecraftJavaArgs", ""); } set { writeUserSetting("MinecraftJavaArgs", value); } }
        public static string InstalledModPackVersion { get { return readUserSetting("InstalledModPackVersion", ""); } set { writeUserSetting("InstalledModPackVersion", value); } }
        public static string MinecraftLauncherPath { get { return Environment.ExpandEnvironmentVariables(readUserSetting("MinecraftLauncherPath", "")); } set { writeUserSetting("MinecraftLauncherPath", value); } }
        public static bool HadFirstSettings { get { return readUserSetting("HadFirstSettings", "") == "yes"; } set { writeUserSetting("HadFirstSettings", value ? "yes" : ""); } }
        // Server Settings
        public static string ModPackUpdateUrl { get { return readServerSetting("ModPackUpdateUrl", ""); } }
        public static string GameDirectorySuffix { get { return readServerSetting("GameDirectorySuffix", ""); } }
        public static string LauncherProfileName { get { return readServerSetting("LauncherProfileName", ""); } }
        public static string LauncherProfileInternalId { get { return readServerSetting("LauncherProfileInternalId", ""); } }
        public static string MinecraftVersion { get { return readServerSetting("MinecraftVersion", ""); } }
        public static string NoticeWebUrl { get { return readServerSetting("NoticeWebUrl", "https://mcupdate.tumblr.com/"); } }
    }
}
