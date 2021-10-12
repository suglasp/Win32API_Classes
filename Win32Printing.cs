
/*
 * Pieter De Ridder
 * github.com/suglasp
 * 
 * Windows API Printer Spooler definitions
 * 
 * Initial creation date: 18-03-2014
 * Change Date: 28-10-2016 (updated header comments on 12/10/2021)
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using Microsoft.Win32;

//using MyCode.Globals;

namespace MyAPIBinding.Win32
{


    /// <summary>
    /// Windows OS Version
    /// </summary>
    public enum OSWindowsVersion {
        OSVersionLegacy = 0,           // Old Client OS like 95, 98, ME, 2000
        OSVersionWindowsXP,
        OSVersionWindowsVista,
        OSVersionWindows7,
        OSVersionWindows8,
        OSVersionWindows81,
        OSVersionWindows10,
        OSVersionWindowsLegacyServer,   // Windows Server 2003, 2008, 2012 flavors
        OSVersionWindowsModernServer    // Windows Server 2016 flavor
    }

    /// <summary>
    /// Windows Printing API and related settings
    /// </summary>
    public static class Win32Printing
    {

        #region Win32 API calls
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true, EntryPoint="AddPrinterConnection")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddPrinterConnection(string pName);
       
/*
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "AddPrinterConnection")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int AddPrinterConnection(string pName);
*/

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeletePrinterConnection(string printerName);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDefaultPrinter(string printerName);
        
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDefaultPrinter(string pszBuffer, ref int size);
        #endregion

        #region Static Methods
        /// <summary>
        /// Create network connected printer
        /// <summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static int MapPrinter(string hostname, bool delay = false)
        {
            int success = -1;

            try
            {
                unsafe
                {
                    AddPrinterConnection(hostname);

#if DEBUG
                    MainProgram.WriteConsole("AddPrinterConnection call [" + hostname + "]");
#endif
                }

                if (delay)  //only when true
                {
                    //sanity checks PrinterMappingDelay
                    if (Global.PrinterMappingDelay < SybaseConfig.PRINTERMAPPINGDELAY_MIN_TRESHOLD)
                    {
                        Global.PrinterMappingDelay = SybaseConfig.PRINTERMAPPINGDELAY_MIN_TRESHOLD;
                    }

                    if (Global.PrinterMappingDelay > SybaseConfig.PRINTERMAPPINGDELAY_MAX_TRESHOLD)
                    {
                        Global.PrinterMappingDelay = SybaseConfig.PRINTERMAPPINGDELAY_MAX_TRESHOLD;
                    }

                    //If we first alive check (ping) a printer host, then we cause a natural delay of mapping printers.
                    //If we skip alive check, we forcefully delay our printer mapping.
                    System.Threading.Thread.Sleep(Global.PrinterMappingDelay);
                }

                success = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            }
            catch (Exception ex)
            {
                //only dump exception to log file
                if (Global.FileLogger != null)
                {
                    Global.FileLogger.WriteLog(ex.ToString());
                    //MainProgram.WriteConsole(ex.ToString());
                }

                throw;
            }

            return success;
        }
        

        /*
        /// <summary>
        /// Create network connected printer
        /// <summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static int MapPrinter(string hostname, bool delay = false)
        {
            int success = -1;          

            try
            {
                unsafe
                {
                    success = AddPrinterConnection(hostname);
#if DEBUG
                    MainProgram.WriteConsole("AddPrinterConnection call");
#endif
                }

                if (delay)  //only when true
                {
                    //sanity checks PrinterMappingDelay
                    if (Global.PrinterMappingDelay < Global.PRINTERMAPPINGDELAY_MIN_TRESHOLD)
                    {
                        Global.PrinterMappingDelay = Global.PRINTERMAPPINGDELAY_MIN_TRESHOLD;
                    }

                    if (Global.PrinterMappingDelay > Global.PRINTERMAPPINGDELAY_MAX_TRESHOLD)
                    {
                        Global.PrinterMappingDelay = Global.PRINTERMAPPINGDELAY_MAX_TRESHOLD;
                    }
                    
                    //If we first alive check (ping) a printer host, then we cause a natural delay of mapping printers.
                    //If we skip alive check, we forcefully delay our printer mapping.
                    System.Threading.Thread.Sleep(Global.PrinterMappingDelay); 
                }
            }
            catch (Exception ex) {
                //only dump exception to log file
                if (Global.FileLogger != null)
                {
                    Global.FileLogger.WriteLog(ex.ToString());
                    //MainProgram.WriteConsole(ex.ToString());
                }

                throw;
            }

            return success;
        }
        */

        /// <summary>
        /// Remove network connected printer
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static bool UnMapPrinter(string hostname)
        {
            bool status = false;

            try
            {
                unsafe {
                    status = DeletePrinterConnection(hostname);

#if DEBUG
            MainProgram.WriteConsole("DeletePrinterConnection call");
#endif
                }
            } catch (Exception ex) {
                //only dump exception to log file
                if (Global.FileLogger != null)
                {
                    Global.FileLogger.WriteLog(ex.ToString());
                    //MainProgram.WriteConsole(ex.ToString());
                }

                throw;
            }

            return status;
        }

        /// <summary>
        /// Get default user printer
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static bool GetDefaultPrinter(string hostname)
        {
            bool status = false;

            try
            {
                unsafe
                {
                    string pszBuffer = string.Empty;
                    int pcchBuffer = hostname.Length;

                    /*
                    if (GetDefaultPrinter(pszBuffer, ref pcchBuffer))
                    {
                        return null;
                    }    
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
                    {
                    }
                    if (lastWin32Error == ERROR_FILE_NOT_FOUND)
                    {
                    }
                    */
                    status = GetDefaultPrinter(pszBuffer, ref pcchBuffer);

#if DEBUG
                    MainProgram.WriteConsole("-- GetDefaultPrinter call");
#endif
                }
            }
            catch (Exception ex)
            {
                //only dump exception to log file
                if (Global.FileLogger != null)
                {
                    Global.FileLogger.WriteLog(ex.ToString());
                    //MainProgram.WriteConsole(ex.ToString());
                }

                throw;
            }

            return status;
        }

        /// <summary>
        /// Set default user printer
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static bool MakeDefaultPrinter(string hostname)
        {
            bool status = false;

            try { 
                unsafe
                {
                    status = SetDefaultPrinter(hostname);

#if DEBUG
                    MainProgram.WriteConsole("-- SetDefaultPrinter call");
#endif
                }
            } catch (Exception ex) {
                //only dump exception to log file
                if (Global.FileLogger != null)
                {
                    Global.FileLogger.WriteLog(ex.ToString());
                    //MainProgram.WriteConsole(ex.ToString());
                }

                throw;
            }

            return status;
        }


        /// <summary>
        /// Request if Windows 10 or Windows 2016 have for the current user "Let Windows Manage Default Printer" enabled or not
        /// </summary>
        /// <returns>true or false</returns>
        public static bool GetModernWindowsUserDefaultPrinterFeature() {
            /*
             * [HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows]
             * "LegacyDefaultPrinterMode"=dword:00000001   = Disabled
             * "LegacyDefaultPrinterMode"=dword:00000000   = Enabled
             */

#if DEBUG
            MainProgram.WriteConsole("-- GetModernWindowsUserDefaultPrinterFeature call");
#endif

            bool winOSDefaultPrinterFeatureEnabled = false;
            
            if ((Global.GetOSVersion() == OSWindowsVersion.OSVersionWindows10) || (Global.GetOSVersion() == OSWindowsVersion.OSVersionWindowsModernServer)) {
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows");
                
                    string osDefaultPrintingFeatureState = key.GetValue("LegacyDefaultPrinterMode").ToString();

                    if (osDefaultPrintingFeatureState != null)
                    {
                        winOSDefaultPrinterFeatureEnabled = (osDefaultPrintingFeatureState == "1") ? false : true;
                    }                
                }
                catch {
                   //reg key does not exist!
                }
            }

            return winOSDefaultPrinterFeatureEnabled;
        }


        /// <summary>
        /// Enable or disable for the current user "Let Windows Manage Default Printer" feature.
        /// Available in Windows 10 since CB/CBB 1511.
        /// </summary>
        /// <param name="WindowsControlPrintingEnabled"></param>
        public static bool SetModernWindowsUserDefaultPrinterFeature(bool WindowsControlPrintingEnabled = false)
        {
            /*
             * [HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows]
             * "LegacyDefaultPrinterMode"=dword:00000001   = Disabled
             * "LegacyDefaultPrinterMode"=dword:00000000   = Enabled
             */

#if DEBUG
            MainProgram.WriteConsole("-- SetModernWindowsUserDefaultPrinterFeature call");
#endif

            bool bSuccessApplied = false;

            if ((Global.GetOSVersion() == OSWindowsVersion.OSVersionWindows10) || (Global.GetOSVersion() == OSWindowsVersion.OSVersionWindowsModernServer))
            {
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows", true); //open in writable mode!

                    if (WindowsControlPrintingEnabled)
                    {
                        key.SetValue("LegacyDefaultPrinterMode", 0, RegistryValueKind.DWord);  // Enable feature                        
                    }
                    else
                    {
                        key.SetValue("LegacyDefaultPrinterMode", 1, RegistryValueKind.DWord);  // Disable feature
                    }

                    bSuccessApplied = true;
                }
                catch {

                }

            }

            return bSuccessApplied;
        }
        #endregion

    }
}
