using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ASUSInfoTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string acs = "ARMOURY CRATE Service";
        private const string rog = "ROGLiveServicePackage";
        private const string atk = "ATK Package";

        public MainWindow()
        {
            InitializeComponent();
            FillData();
        }

        private async void Createlog_Button_Click(object sender, RoutedEventArgs e)
        {
            await WriteLog();
        }

        private void FillData()
        {
            var data = new GetData();
            model_value.Content = data.GetBaseboard()[0];
            if (data.GetBios()[0].ToString() == "System Serial Number")
            {
                serial_value.Content = data.GetBaseboard()[1];
            }
            else
            { 
                serial_value.Content = data.GetBios()[0];
            }
            bios_value.Content = data.GetBios()[1];
            myasus_value.Content = data.AppX("*ASUSPCAssistant*")[0];
            armoury_value.Content = data.AppX("*armoury*")[0];
            windows_value.Content = data.GetOSVersion();
            armouryservice_value.Content = data.Checkapp(acs);
            rogliveservice_value.Content = data.Checkapp(rog);
            asdcdver_value.Text = data.CheckASDC();
            ashdiver_value.Text = data.CheckASHDI();
        }

        /// <summary>
        /// Submit to create logfile
        /// </summary>
        public async Task WriteLog()
        {

            Createlog_Button.Content = "Bitte Warten...";
            Createlog_Button.IsEnabled = false;
            await Task.Run(() =>
            {
                var data = new GetData();
                string serial;
                if (data.GetBios()[0].ToString() == "System Serial Number")
                {
                    serial = data.GetBaseboard()[1].ToString();
                }
                else
                {
                    serial = data.GetBios()[0].ToString();
                }
                serial = Regex.Replace(serial, @"\s", "");
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
                using StreamWriter file = new(desktop + "\\ASUS-LOG-" + serial + ".txt", append: false);
                file.WriteLineAsync("═══════════════════════════════════════════════════════════════════════");
                file.WriteLineAsync("Model  : " + data.GetBaseboard()[0].ToString());
                file.WriteLineAsync("Serial : " + serial);
                file.WriteLineAsync("Bios   : " + data.GetBios()[1].ToString());
                file.WriteLineAsync("OSVer  : " + data.GetOSVersion());
                foreach (var gpu in data.GetVGACard())
                {
                    string[] gpudata = gpu.ToString().Split(',');
                    file.WriteLineAsync("GPU    : " + gpudata[0]);
                    file.WriteLineAsync("Driver : " + gpudata[1]);
                }
                file.WriteLineAsync("ASUS LOG═══════════════════════════════════════════════════════════════");
                file.WriteLineAsync("ASDCD : " + data.CheckASDC());
                file.WriteLineAsync("ASHDI : " + data.CheckASHDI());
                file.WriteLineAsync("ASUS APPs══════════════════════════════════════════════════════════════");
                file.WriteLineAsync("MyASUS : " + data.AppX("*ASUSPCAssistant*")[0]);
                file.WriteLineAsync("ScreenXpert  : " + data.AppX("*ScreenPadMaster*")[0]);
                file.WriteLineAsync("ArmouryCrate : " + data.AppX("*armoury*")[0]);
                file.WriteLineAsync("AURA Creator : " + data.AppX("*AURACreator*")[0]);
                file.WriteLineAsync("KeyboardHotkeys : " + data.AppX("*ASUSKeyboardHotkeys*")[0]);
                file.WriteLineAsync("HealthCharging  : " + data.AppX("*ASUSBatteryHealthCharging*")[0]);
                file.WriteLineAsync("ASUS Services══════════════════════════════════════════════════════════");
                file.WriteLineAsync("ArmouryCrate Service : " + data.Checkapp(acs));
                file.WriteLineAsync("ROG Live Service : " + data.Checkapp(rog));
                file.WriteLineAsync("ATK Package (OSD) : " + data.Checkapp(atk));
                file.WriteLineAsync("ASUS Drivers═══════════════════════════════════════════════════════════");
                file.WriteLineAsync("Keyboard Hotkeys (ATK) : " + data.GetDriver("ATK Package")[0].ToString());
                file.WriteLineAsync("System Control Interface : " + data.GetDriver("ASUS System Control Interface")[0].ToString());
                file.WriteLineAsync("Precision Touchpad : " + data.GetDriver("ASUS Precision Touchpad")[0].ToString());
                file.WriteLineAsync("Number Pad : " + data.GetDriver("ASUS Number Pad")[0].ToString());
                file.WriteLineAsync("ScreenXpert Interface  : " + data.GetDriver("ASUS ScreenXpert Interface")[0].ToString());
                file.WriteLineAsync("Wireless Radio Control : " + data.GetDriver("ASUS Wireless Radio Control")[0].ToString());
            });
            MessageBox.Show("Log wurde geschrieben und auf dem Desktop gespeichert.\nBitte senden Sie die Datei an den Support.");
            Createlog_Button.Content = "LOG Datei erstellen";
            Createlog_Button.IsEnabled = true;
        }
}
}
