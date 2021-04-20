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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.Runtime;
using System.IO;
using System.Collections;
using Microsoft.Win32;

namespace ASUSInfoTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string localMachine = "HKEY_LOCAL_MACHINE";

        public MainWindow()
        {
            InitializeComponent();

            GetBios();
            GetBaseboard();
            GetOSVersion();
            CheckOS();
            CheckApps();
        }
        /// <summary>
        /// Get Model and Serialnumber of System
        /// </summary>
        private void GetBaseboard()
        {
            System.Management.ManagementClass wmi = new("win32_baseboard");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                model_value.Content = provider["Product"];

            }
        }

        /// <summary>
        /// Get Bios Version
        /// </summary>
        private void GetBios()
        {
            System.Management.ManagementClass wmi = new("win32_bios");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                serial_value.Content = provider["SerialNumber"];
                bios_value.Content = provider["SMBIOSBIOSVersion"];
            }
        }

        /// <summary>
        /// Get OS Buildname like 20h2 and get Buildnumber and PatchLevel from Registry
        /// </summary>
        private void GetOSVersion()
        {
            const string subkey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";
            const string keyName = MainWindow.localMachine + "\\" + subkey;

            var OSVersion = Registry.GetValue(keyName, "DisplayVersion", Registry.GetValue(keyName, "ReleaseID", null));
            var CurrentBuild = Registry.GetValue(keyName, "CurrentBuild", null);
            var PatchLevel = Registry.GetValue(keyName, "UBR", null);
            if (OSVersion != null)
            {
                windows_value.Content = OSVersion + " - " + CurrentBuild + "." + PatchLevel;
            }
            else
            {
                windows_value.Content = "error";
            }
        }

        /// <summary>
        /// Get VGA Card Name and DriverVersion
        /// </summary>
        public static void GetVGACard()
        {
            System.Management.ManagementClass wmi = new("win32_videocontroller");
            var providers = wmi.GetInstances();

            var myAL = new ArrayList();
            foreach (var provider in providers)
            {
                myAL.Add(provider["Name"]);
                myAL.Add(provider["DriverVersion"]);
            }
        }

        /// <summary>
        /// Check if ASUS OS
        /// </summary>
        private void CheckOS()
        {
            string windir = Environment.GetEnvironmentVariable("windir");
            string AsDCDVer = windir + "\\AsDCDVer.txt";
            string AsHDIVer = windir + "\\AsHDIVer.txt";
            string asdcd = File.Exists(AsDCDVer) ? File.ReadLines(@AsDCDVer).First() : "Not ASUS OS";
            string ashdi = File.Exists(AsHDIVer) ? File.ReadLines(@AsHDIVer).First() : "Not ASUS OS";
            asdcdver_value.Text = asdcd;
            ashdiver_value.Text = ashdi;
        }

        /// <summary>
        /// Check Desktop Apps
        /// </summary>
        /// 
        private void CheckApps()
        { 
            armouryservice_value.Content = Checkapp("ARMOURY CRATE Service");
            rogliveservice_value.Content = Checkapp("ROGLiveServicePackage");
        }

        public static string Checkapp(string app)
        {
            string subkey = "SOFTWARE\\ASUS\\";
            string keyName = localMachine + "\\" + subkey + app;

            var DisplayVersion = Registry.GetValue(keyName, "DisplayVersion", null);
            if (DisplayVersion != null)
            {
                return (string)DisplayVersion;
            }
            else
            {
                return "Not installed";
            }
        }
        /// <summary>
        /// Check UWP Apps
        /// </summary>
        ///private voide CheckUWPApp()
        ///{
        ///
        ///}

        /// <summary>
        /// Submit to create logfile
        /// </summary>

        /// <summary>
        /// Write to file
        /// </summary>
        public static async Task ExampleAsync()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
            using StreamWriter file = new(desktop + "\\LOG-SN.txt", append: true);
            await file.WriteLineAsync("Fourth line");
        }
}
}
