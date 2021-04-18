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
using System.Linq;
using Microsoft.Win32;

namespace ASUSInfoTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GetBios();
            GetBaseboard();
            GetOSVersion();
            GetVGACard();
            CheckOS();
        }
        /// <summary>
        /// Get Model and Serialnumber of System
        /// </summary>
        private void GetBaseboard()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("win32_baseboard");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                model_value.Content = provider["Product"];
                serial_value.Content = provider["SerialNumber"];
            }
        }

        /// <summary>
        /// Get Bios Version
        /// </summary>
        private void GetBios()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("win32_bios");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                string SMBIOSBIOSVersion = Convert.ToString(provider["SMBIOSBIOSVersion"]);

                bios_value.Content = provider["SMBIOSBIOSVersion"];
            }
        }

        /// <summary>
        /// Get OS Buildname like 20h2 and get Buildnumber and PatchLevel from Registry
        /// </summary>
        private void GetOSVersion()
        {
            const string localMachine = "HKEY_LOCAL_MACHINE";
            const string subkey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";
            const string keyName = localMachine + "\\" + subkey;

            var OSVersion = Registry.GetValue(keyName, "DisplayVersion", "null");
            var CurrentBuild = Registry.GetValue(keyName, "CurrentBuild", "null");
            var PatchLevel = Registry.GetValue(keyName, "UBR", "null");
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
        private void GetVGACard()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("win32_videocontroller");
            var providers = wmi.GetInstances();

            //foreach (var provider in providers)
            //{
            //    asdcdver_value.Content = provider["Name"];
            //    ashdiver_value.Content = provider["DriverVersion"];
            //}

        }

        /// <summary>
        /// Check if ASUS OS
        /// </summary>
        private void CheckOS()
        {
            string AsDCDVer = @"C:\Windows\AsDCDVer.txt";
            string AsHDIVer = @"C:\Windows\AsHDIVer.txt";
            var asdcd = (File.Exists(AsDCDVer) ? File.ReadLines(AsDCDVer).First() : "Not ASUS OS");
            var ashdi = (File.Exists(AsHDIVer) ? File.ReadLines(AsHDIVer).First() : "Not ASUS OS");
            asdcdver_value.Content = asdcd;
            ashdiver_value.Content = ashdi;
        }

    }
}
