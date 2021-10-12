
Some examples how to use the C# classes Win32RDS & Win32Printing
Pieter De Ridder
github.com/suglasp

----------------------
I am by no means a Professional or Daily C# Programmer, but a System Engineer with Programming skills.
That uses C# or .Net technology to automate some stuff or to write system/low level tools.
If some of the code to a daily C# dev 'looks old skool'. You know why. :)
----------------------

// These two classes are part of a tool i wrote to map printers stored from within a Database.
// This code has proven to be reliable code, since this code ran on more than 10000 enterprise workstations (Windows XP-10) and hundreds of RDS servers (Windows Server OS flavors) since 2012.
// During the time, i updated the classes often to reflect modern OS flavors of Windows.


// --- include Win32 API binding namespace ---
using using MyAPIBinding.Win32;


// --- Win32 Terminal Session status ---
// Example : Win32RDS Static class

//create and fetch connection status
Win32RDS rdsClient = new Win32RDS();

// get session ID
Console.WriteLine(rdsClient.RDSUserSessionId.ToString());
Console.WriteLine(rdsClient.RDSSessionState.ToString());
Console.WriteLine(rdsClient.RDSUserIdleTime.ToString());

//check console session (local to keyboard/mouse) or RDP
If (rdsClient.RDSUserSessionId <= 1)
{
  Console.WriteLine(Convert.ToChar(016) + "We are in a Console session (id = " + rdsClient.RDSUserSessionId.ToString() + " / idle time =  " + lastIdle.ToString() + "ms)");
} Else {
  Console.WriteLine(Convert.ToChar(016) + "We are in a RDP session (init state = " + rdsClient.RDSSessionState.ToString() + ")");
}

// update status and fetch it
rdsClient.UpdateSessionState();
Console.WriteLine(rdsClient.RDSSessionState.ToString());

if (((rdsClient.RDSSessionState.Equals("WTSCONNECTED")) || (rdsClient.RDSSessionState.Equals("WTSACTIVE")) || (rdsClient.RDSSessionState.Equals("WTSIDLE"))) && (lastState.Equals("WTSDISCONNECTED")))
	Console.WriteLine("You are reconnected to a session");
}






// --- Win32 Printing --
// Example : Win32Printing Static class
//
// note : this code is tested to run on Windows XP, Windows 7, Windows 8.x, Windows 10, Server 2008 R2, Server 2012R2, Server 2016 and Server 2019
// C# has a class to handle printers. But i found some bug in the framework when mapping printers in combination with bogus registry settings under the current user profile.
// I created my own class to map and unmap network printers over the SMB protocol using the Win32 API.
//

// set default printer
Win32Printing.MakeDefaultPrinter("\\printerserver.mylab.local\printerDell");

// map a network printer from a Windows Print Server (or Samba) + optionally, add a delay between printer mappings (so the underlying OS has some time to map the network printer).
Win32Printing.MapPrinter("\\printerserver.mylab.local\printerKyocera", true);

// unmap a network printer
Win32Printing.UnMapPrinter(lp);


// windows 10 can override the Default Printer according to the printer the user uses most often.
// We can query this setting and also disable or enable it.
bool w10autoPrinter = Win32Printing.GetModernWindowsUserDefaultPrinterFeature();
Win32Printing.SetModernWindowsUserDefaultPrinterFeature(false);
Win32Printing.SetModernWindowsUserDefaultPrinterFeature(true);

