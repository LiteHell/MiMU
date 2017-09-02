using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace MiMU
{
    public static class Settings
    {
        // from http://wow54321.tistory.com/5
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        
        private static string settingsPath = Path.Combine(Environment.CurrentDirectory, "mimu.ini");
        private static void writeSetting(string section, string keyName, string keyValue)
        {
            WritePrivateProfileString(section, keyName, keyValue, settingsPath);
        }
        private static string readSetting(string section, string keyName, string defaultValue)
        {
            StringBuilder strBuilder = new StringBuilder();
            GetPrivateProfileString(section, keyName, defaultValue, strBuilder, 256, settingsPath);
            return strBuilder.ToString();
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
