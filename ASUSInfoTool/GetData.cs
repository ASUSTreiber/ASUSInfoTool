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
    class GetData
    {
        private const string localMachine = "HKEY_LOCAL_MACHINE";

        public ArrayList AppX(string args)
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
                if (appx.Count > 0)
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

        /// <summary>
        /// Get OS Buildname like 20h2 and get Buildnumber and PatchLevel from Registry
        /// </summary>
        public string GetOSVersion()
        {
            const string subkey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";
            const string keyName = GetData.localMachine + "\\" + subkey;

            var OSVersion = Registry.GetValue(keyName, "DisplayVersion", Registry.GetValue(keyName, "ReleaseID", null));
            var CurrentBuild = Registry.GetValue(keyName, "CurrentBuild", null);
            var PatchLevel = Registry.GetValue(keyName, "UBR", null);
            if (OSVersion != null)
            {
                return OSVersion + " - " + CurrentBuild + "." + PatchLevel;
            }
            else
            {
                return "error";
            }
        }

        public string Checkapp(string app)
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
        /// Check if ASUS OS
        /// </summary>
        public string CheckASDC()
        {
            string windir = Environment.GetEnvironmentVariable("windir");
            string AsDCDVer = windir + "\\AsDCDVer.txt";
            string asdcd = File.Exists(AsDCDVer) ? File.ReadLines(@AsDCDVer).First() : "Not ASUS OS";
            return asdcd;
        }
        public string CheckASHDI()
        {
            string windir = Environment.GetEnvironmentVariable("windir");
            string AsHDIVer = windir + "\\AsHDIVer.txt";
            string ashdi = File.Exists(AsHDIVer) ? File.ReadLines(@AsHDIVer).First() : "Not ASUS OS";
            return ashdi;
        }
    }
}
