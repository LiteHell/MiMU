using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UpdaterCore;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace MiMU
{
    class UpdateInstance
    {
        private LauncherProfileManager launcherProfileManager;
        private DownloadUtils downloadUtils = new DownloadUtils();
        private ModPackFetcher fetcher = new ModPackFetcher(Settings.ModPackUpdateUrl);
        private MainWindow updaterWindow;
        private bool forcedUpdate;
        public void Start(MainWindow updaterWindow, bool forcedUpdate = false)
        {
            this.updaterWindow = updaterWindow;
            this.forcedUpdate = forcedUpdate;
            Thread thr = new Thread(startUpdate);
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
        }
        private void startUpdate()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; // to resolve SendFailure bug

            // get installed version
            SetMainProgress(ProgressValueTypes.Maximum, 5);
            SetMainStatus("모드팩 버전을 확인하고 있습니다.");
            SetSubStatus("설치된 버전 확인중");
            string installedVerson = Settings.InstalledModPackVersion;
            // get versions
            SetSubStatus("최신 버전 확인중");
            List<ModPackVersion> versions = new List<ModPackVersion>(fetcher.GetModPacks());
            versions.Sort();
            ModPackVersion latestVersion = versions[0];
            SetSubStatus("런쳐 프로파일 존재 여부 확인중");
            // check whether there is launcher profile
            if(!LauncherProfileManager.ProfilesExists)
            {
                MessageBox.Show("런쳐 프로파일 설정파일이 없습니다. 런쳐를 최소 한번 이상 실행시켜주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
            SetSubStatus("모드팩 업데이트 필요 여부 확인중");
            launcherProfileManager = new LauncherProfileManager();
            // check whether update is needed
            if (installedVerson == latestVersion.Identifier && !forcedUpdate)
            {
                if (Settings.MinecraftLauncherPath != "")
                {
#if !NoLauncher && DEBUG
                    Process.Start(Settings.MinecraftLauncherPath);
#endif
                    return;
                }
            }
            SetMainProgress(ProgressValueTypes.Value, 1, true);
            // check forge
            SetMainStatus("포지 설치 필요 여부을 확인하고 있습니다.");
            SetSubStatus("포지 최신버전을 확하는 중");
            List<string> forgeVersionIds = new List<string>(DetectForges(Settings.MinecraftVersion));
            if (forgeVersionIds.Count > 0)
            {
                SetSubStatus("포지 있음. 설치를 건너뜁니다...");
            }
            else
            {
                SetMainStatus("포지를 설치하고 있습니다.");
                SetSubStatus("설치된 포지 없음. 포지 설치를 시도합니다....");
                bool retry = false;
                while (forgeVersionIds.Count == 0)
                {
                    if (retry)
                        SetSubStatus("포지가 설치되지 않음. 다시 시도중...");
                    InstallForge();
                    forgeVersionIds = new List<string>(DetectForges(Settings.MinecraftVersion));
                    retry = true;
                }
            }
            SetMainProgress(ProgressValueTypes.Value, 1, true);
            string latestForgeVersionId = forgeVersionIds.Last();
            // extract modpack
            SetMainStatus("모드팩 다운로드를 시작합니다.");
            string gameDirectory = Environment.ExpandEnvironmentVariables("%AppData%\\.minecraft-" + Settings.GameDirectorySuffix);
            string modPackTempFile = $"modpack-{DateTime.UtcNow.Ticks}.zip";
            SetSubStatus($"{latestVersion.PackageUrl} => {modPackTempFile}");
            downloadUtils.DownloadToFile(latestVersion.PackageUrl, modPackTempFile);
            SetSubStatus("OK");
            SetMainProgress(ProgressValueTypes.Value, 1, true);
            SetMainStatus("모드팩 압축 해제를 시작합니다.");
            if (!Directory.Exists(gameDirectory))
                Directory.CreateDirectory(gameDirectory);
            if (!Directory.Exists(gameDirectory + "\\mods"))
                Directory.CreateDirectory(gameDirectory + "\\mods");
            using (FileStream archiveFileStream = new FileStream(modPackTempFile, FileMode.Open))
            using (ZipArchive archive = new ZipArchive(archiveFileStream, ZipArchiveMode.Read, false, Encoding.UTF8))
            {
                SetSubProgress(ProgressValueTypes.Maximum, archive.Entries.Count);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    SetSubStatus($"압축 해제 : {entry.FullName}");
                    entry.ExtractToFile(Path.Combine(gameDirectory, "mods", entry.FullName), true);
                    SetSubProgress(ProgressValueTypes.Value, 1, true);
                }
            }
            File.Delete(modPackTempFile);
            SetMainStatus("모드팩 압축 해제 완료");
            SetSubStatus("OK");
            SetMainProgress(ProgressValueTypes.Value, 1, true);
            SetSubProgress(ProgressValueTypes.Value, 0);
            // modify profile
            SetMainStatus("런쳐 프로파일 수정중");
            SetSubStatus("설정 확인중");
            if (!Settings.HadFirstSettings)
            {
                FirstSettings fsWindow = new FirstSettings();
                while (true)
                {
                    fsWindow.ShowDialog();
                    if (fsWindow.LauncherPath != "")
                    {
                        Settings.MinecraftLauncherPath = fsWindow.LauncherPath;
                        Settings.MinecraftJavaArgs = fsWindow.JavaArguments;
                        Settings.HadFirstSettings = true;
                        break;
                    }
                }
            }
            SetSubStatus("런쳐 프로파일 수정중");
            LauncherProfile profile = new LauncherProfile();
            profile.SetCreatedAt(DateTime.Now);
            profile.SetLastUsedAt(DateTime.Now);
            profile.gameDir = gameDirectory;
            profile.name = Settings.LauncherProfileName;
            profile.lastVersionId = latestForgeVersionId;
            if (Settings.MinecraftJavaArgs.Trim() != "")
                profile.javaArgs = Settings.MinecraftJavaArgs;
            launcherProfileManager.AddOrAlterProfile(Settings.LauncherProfileInternalId, profile);
            Settings.InstalledModPackVersion = latestVersion.Identifier;
            SetSubStatus("완료");
            SetMainProgress(ProgressValueTypes.Value, 1, true);
            Settings.InstalledModPackVersion = latestVersion.Identifier;
#if !NoLauncher && DEBUG
            Process.Start(Settings.MinecraftLauncherPath);
#endif
        }
        private void SetMainStatus(string newStatus)
        {
            updaterWindow.Dispatcher.Invoke(() => updaterWindow.mainStatus.Text = newStatus);
        }
        private void SetSubStatus(string newStatus)
        {
            updaterWindow.Dispatcher.Invoke(() => updaterWindow.subStatus.Text = newStatus);
        }
        private enum ProgressValueTypes
        {
            Maximum,
            Minimum,
            Value
        }
        private void SetMainProgress(ProgressValueTypes valueType, int value, bool isIncrease = false)
        {
            updaterWindow.Dispatcher.Invoke(() =>
            {
                switch (valueType)
                {
                    case ProgressValueTypes.Maximum:
                        updaterWindow.mainProgress.Maximum = value + (isIncrease ? updaterWindow.mainProgress.Maximum : 0);
                        break;
                    case ProgressValueTypes.Minimum:
                        updaterWindow.mainProgress.Minimum = value + (isIncrease ? updaterWindow.mainProgress.Minimum : 0);
                        break;
                    case ProgressValueTypes.Value:
                        updaterWindow.mainProgress.Value = value + (isIncrease ? updaterWindow.mainProgress.Value : 0);
                        break;
                }
            });
        }
        private void SetSubProgress(ProgressValueTypes valueType, int value, bool isIncrease = false)
        {
            updaterWindow.Dispatcher.Invoke(() =>
            {
                switch (valueType)
                {
                    case ProgressValueTypes.Maximum:
                        updaterWindow.subProgress.Maximum = value + (isIncrease ? updaterWindow.subProgress.Maximum : 0);
                        break;
                    case ProgressValueTypes.Minimum:
                        updaterWindow.subProgress.Minimum = value + (isIncrease ? updaterWindow.subProgress.Minimum : 0);
                        break;
                    case ProgressValueTypes.Value:
                        updaterWindow.subProgress.Value = value + (isIncrease ? updaterWindow.subProgress.Value : 0);
                        break;
                }
            });
        }
        private IEnumerable<string> DetectForges(string minecraftVersion)
        {
            List<string> forgeVersionIds = new List<string>(launcherProfileManager.GetVersionIds()
                .Where(id => id.Contains("forge" + minecraftVersion)));
            forgeVersionIds.Sort((a, b) =>
            {
                Version aVer = new Version(a.Split('-').Last());
                Version bVer = new Version(b.Split('-').Last());
                return aVer.CompareTo(bVer);
            });
            return forgeVersionIds;
        }
        private void InstallForge()
        {
            SetSubStatus("포지 최신 버전 확인중");
            ForgeFetcher forgeFetcher = new ForgeFetcher();
            int latestForgeBuildNo = forgeFetcher.GetForgeBuilds(Settings.MinecraftVersion).Max();
            ForgeBuildInfo latestForgeBuild = forgeFetcher.GetForgeInfo(latestForgeBuildNo);
            //SetSubStatus($"최신 포지 버전 정보 가져옴 : {latestForgeBuild.ForgeVersion} (Build {latestForgeBuild.BuildNo})");
            string InstallerUrl = latestForgeBuild.Installer.Url;
            string tempFilePath = $"forge-installer-{DateTime.UtcNow.Ticks}.jar";
            SetSubStatus("포지 간편설치기 다운로드중");
            downloadUtils.DownloadToFile(InstallerUrl, tempFilePath);
            Process proc = new Process();
            proc.StartInfo.FileName = "javaw.exe";
            proc.StartInfo.Arguments = "-jar " + tempFilePath;
            proc.StartInfo.UseShellExecute = false;
            SetSubStatus("포지 간편설치기를 실행했습니다. 포지 설치후 간편설치기를 종료해주세요.");
            proc.Start();
            proc.WaitForExit();
            File.Delete(tempFilePath);
            SetSubStatus("설치 완료");
        }
    }
}
