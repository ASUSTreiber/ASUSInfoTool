using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Threading;
using System.Management.Automation;

namespace ASUSInfoTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string localMachine = "HKEY_LOCAL_MACHINE";
        //private const string[] AppSearch = new string[]("ASUSPCAssistant", "ScreenPadMaster", "Armoury", "ASUSKeyboardHotkeys", "ASUSBatteryHealthCharging");

        public MainWindow()
        {
            InitializeComponent();
            FillData();
            GetOSVersion();
            CheckOS();
            CheckApps();
        }

        private async void Createlog_Button_Click(object sender, RoutedEventArgs e)
        {
            Createlog_Button.IsEnabled = false;
            await WriteLog();
            MessageBox.Show("Log wurde geschrieben und auf dem Desktop gespeichert.\nBitte senden Sie die Datei an den Support.");
            Createlog_Button.IsEnabled = true;
        }

        private ArrayList AppX(string args)
        {
            using (PowerShell PowerShellInst = PowerShell.Create())
            {
                string criteria = args;
                var appx = new ArrayList();
                PowerShellInst.AddScript("Get-AppxPackage " + criteria);
                Collection<PSObject> PSOutput = PowerShellInst.Invoke();
                foreach (PSObject obj in PSOutput)
                {
                    if (obj != null)
                    {
                        //appx.Add(obj.Properties["Name"].Value.ToString() + " - "));
                        appx.Add(obj.Properties["Version"].Value.ToString());
                    }
                }
                if(appx.Count > 0)
                {
                    return appx;
                }
                else
                {
                    appx.Add("Not Installed");
                    return appx;
                }
                
            }
        }

        private void FillData()
        {
            model_value.Content = GetBaseboard()[0];
            if (GetBios()[0].ToString() == "System Serial Number")
            {
                serial_value.Content = GetBaseboard()[1];
            }
            else
            { 
                serial_value.Content = GetBios()[0];
            }
            bios_value.Content = GetBios()[1];
            myasus_value.Content = AppX("*ASUSPCAssistant*")[0];
            armoury_value.Content = AppX("*armoury*")[0];
        }

        /// <summary>
        /// Get Model and Serialnumber of System
        /// </summary>
        private ArrayList GetBaseboard()
        {
            System.Management.ManagementClass wmi = new("win32_baseboard");
            var providers = wmi.GetInstances();
            var product = new ArrayList();
            foreach (var provider in providers)
            {
                product.Add(provider["Product"].ToString());
                product.Add(provider["SerialNumber"].ToString());
            }
            return product;
        }

        /// <summary>
        /// Get Bios Version
        /// </summary>
        private ArrayList GetBios()
        {
            System.Management.ManagementClass wmi = new("win32_bios");
            var providers = wmi.GetInstances();
            var bios = new ArrayList();
            foreach (var provider in providers)
            {
                bios.Add(provider["SerialNumber"].ToString());
                bios.Add(provider["SMBIOSBIOSVersion"].ToString());
            }
            return bios;
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
        public async Task WriteLog()
        {
            string serial;
            if (GetBios()[0].ToString() == "System Serial Number")
            {
                serial = GetBaseboard()[1].ToString();
            }
            else
            {
                serial = GetBios()[0].ToString();
            }
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
            using StreamWriter file = new(desktop + "\\LOG-" + serial + ".txt", append: true);
            await file.WriteLineAsync("====================================================");
            await file.WriteLineAsync("Model: " + GetBaseboard()[0].ToString());
            await file.WriteLineAsync("Serial: " + serial);
            await file.WriteLineAsync("Biosversion: " + GetBios()[1].ToString());
            await file.WriteLineAsync("====================================================");
        }
}
}
