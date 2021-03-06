﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace MiMU
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartUpdater();
        }

        private void StartUpdater()
        {
            UpdaterCore.WebRequestable.GUIVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            noticeBrowser.Navigate(Settings.NoticeWebUrl);
            UpdateInstance update = new UpdateInstance();
            IEnumerable<string> args = Environment.GetCommandLineArgs().Skip(1);
            bool modPackForced = args.Contains("--forced"), forgeForced = args.Contains("--forge-forced");
#if DEBUG
            Debug.WriteLine("forced mod pack update? : " + (modPackForced ? "yes" : "no"));
            Debug.WriteLine("forced forge update? : " + (forgeForced ? "yes" : "no"));
#endif
            update.Start(this, modPackForced, forgeForced);
        }
    }
}
