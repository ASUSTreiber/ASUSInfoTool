using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace ASUSInfoTool
{
    class GetData
    {
        private const string localMachine = "HKEY_LOCAL_MACHINE";

        public ArrayList AppX(string args)
        {
            using PowerShell PowerShellInst = PowerShell.Create();
            string criteria = args;
            var appx = new ArrayList();
            PowerShellInst.AddScript("Get-AppxPackage " + criteria);
            Collection<PSObject> PSOutput = PowerShellInst.Invoke();
            foreach (PSObject obj in PSOutput)
            {
                if (obj != null)
                {
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

        public string CheckappWow(string app)
        {
            string subkey = "SOFTWARE\\WOW6432Node\\ASUS\\";
            string keyName = localMachine + "\\" + subkey + app;

            var Version = Registry.GetValue(keyName, "Version", null);
            if (Version != null)
            {
                return (string)Version;
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

        /// <summary>
        /// Get Model and Serialnumber of System
        /// </summary>
        public ArrayList GetBaseboard()
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
        public ArrayList GetBios()
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
        public ArrayList GetVGACard()
        {
            System.Management.ManagementClass wmi = new("win32_videocontroller");
            var providers = wmi.GetInstances();

            var myAL = new ArrayList();
            foreach (var provider in providers)
            {
                myAL.Add(provider["Name"] + "," + provider["DriverVersion"] + "," + provider["PNPDeviceID"]);
            }
            return myAL;
        }

        /// <summary>
        /// Get Driver Data
        /// </summary>
        public ArrayList GetDriver(string args)
        {
            string driver = args;
            var device = new ArrayList();
            using PowerShell PowerShellInst = PowerShell.Create();
            PowerShellInst.AddScript("Get-WmiObject Win32_PnPSignedDriver | where {$_.DeviceName -like \"*" + driver + "*\"}");
            Collection<PSObject> PSOutput = PowerShellInst.Invoke();
            foreach (PSObject obj in PSOutput)
            {
                device.Add(obj.Properties["DriverVersion"].Value.ToString()
                           + " - "
                           + obj.Properties["DeviceID"].Value.ToString());
            }
            if (device.Count > 0)
            {
                return device;
            }
            else
            {
                device.Add("Not Installed");
                return device;
            }
        }
    }
}
