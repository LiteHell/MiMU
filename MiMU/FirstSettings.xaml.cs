using System;
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
using System.Windows.Shapes;

namespace MiMU
{
    /// <summary>
    /// FirstSettings.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FirstSettings : Window
    {
        public FirstSettings()
        {
            InitializeComponent();
        }
        private string launcherPath;
        public string LauncherPath { get { return launcherPath; } set { launcherPath = value; if (value.Trim().Length != 0) SpecifyLauncher.Content = "(런쳐 선택됨)"; } }
        public string JavaArguments { get { return JavaArgs.Text; } set { JavaArgs.Text = value; } }
        private void SpecifyLauncher_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "*.exe|*.exe";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "마인크래프트 런쳐 선택";
            openFileDialog.ShowDialog();
            launcherPath = openFileDialog.FileName;
            SpecifyLauncher.Content = "(런쳐 선택됨)";
        }
    }
}
