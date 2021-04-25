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
        private const string acs = "ARMOURY CRATE Service";
        private const string rog = "ROGLiveServicePackage";

        public MainWindow()
        {
            InitializeComponent();
            FillData();
        }

        private async void Createlog_Button_Click(object sender, RoutedEventArgs e)
        {
            Createlog_Button.IsEnabled = false;
            await WriteLog();
            MessageBox.Show("Log wurde geschrieben und auf dem Desktop gespeichert.\nBitte senden Sie die Datei an den Support.");
            Createlog_Button.IsEnabled = true;
        }

        private void FillData()
        {
            var data = new GetData();
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
            myasus_value.Content = data.AppX("*ASUSPCAssistant*")[0];
            armoury_value.Content = data.AppX("*armoury*")[0];
            windows_value.Content = data.GetOSVersion();
            armouryservice_value.Content = data.Checkapp(acs);
            rogliveservice_value.Content = data.Checkapp(rog);
            asdcdver_value.Text = data.CheckASDC();
            ashdiver_value.Text = data.CheckASHDI();
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
        /// Submit to create logfile
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
