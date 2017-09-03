using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;

namespace MiMU
{
    public static class Settings
    {
        private static IniData userIniFile;
        private static IniData serverIniFile;
        private static FileIniDataParser iniFileParser = new FileIniDataParser();
        private static string serverSettingsPath = Path.Combine(Environment.CurrentDirectory, "server.ini");
        private static string userSettingsPath = Path.Combine(Environment.CurrentDirectory, "user.ini");
        static Settings()
        {
            if (File.Exists(serverSettingsPath))
                serverIniFile = iniFileParser.ReadFile(serverSettingsPath, Encoding.UTF8);
            else
                serverIniFile = new IniData();
            if (File.Exists(userSettingsPath))
                userIniFile = iniFileParser.ReadFile(userSettingsPath, Encoding.UTF8);
            else
                userIniFile = new IniData();
        }
        private static void writeUserSetting(string section, string keyName, string keyValue)
        {
            userIniFile[section][keyName] = keyValue;
            iniFileParser.WriteFile(userSettingsPath, userIniFile, Encoding.UTF8);
        }
        private static string readSetting(string section, string keyName, string defaultValue, bool isUser)
        {
            return (isUser ? userIniFile[section][keyName] : serverIniFile[section][keyName]) ?? defaultValue;
        }
        private static void writeUserSetting(string keyName, string keyValue)
        {
            writeUserSetting("User", keyName, keyValue);
        }
        private static string readUserSetting(string keyName, string defaultValue)
        {
            return readSetting("User", keyName, defaultValue, true);
        }
        private static string readServerSetting(string keyName, string defaultValue)
        {
            return readSetting("Server", keyName, defaultValue, false);
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
