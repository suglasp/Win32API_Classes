
/*
 * Pieter De Ridder
 * github.com/suglasp
 * 
 * Windows API RDS definitions
 * 
 * Initial creation date: 01-02-2014
 * Change Date: 26-01-2017 (updated header comments on 12/10/2021)
 * 
 * Re-used class from other .NET Project utility.
 * 
 */


using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace MyAPIBinding.Win32
{

    #region RDS Win32 Enums  
    
    /// <summary>
    /// WTS_CONNECTSTATE_CLASS
    /// </summary>
    public enum WTS_CONNECTSTATE_CLASS { 
          WTSActive = 0,
          WTSConnected,
          WTSConnectQuery,
          WTSShadow,
          WTSDisconnected,
          WTSIdle,
          WTSListen,
          WTSReset,
          WTSDown,
          WTSInit
    }    

    /// <summary>
    /// WTS_INFO_CLASS
    /// https://msdn.microsoft.com/en-us/library/aa383861%28v=vs.85%29.aspx
    /// </summary>
    public enum WTS_INFO_CLASS
    {
        //WTSInitialProgram         = 0,
        //WTSApplicationName        = 1,
        //WTSWorkingDirectory       = 2,
        //WTSOEMId                  = 3,
        WTSSessionId              = 4,
        WTSUserName               = 5,
        WTSWinStationName         = 6,
        WTSDomainName             = 7,
        WTSConnectState           = 8,
        //WTSClientBuildNumber      = 9,
        WTSClientName             = 10,
        //WTSClientDirectory        = 11,
        //WTSClientProductId        = 12,
        //WTSClientHardwareId       = 13,
        //WTSClientAddress          = 14,
        //WTSClientDisplay          = 15,
        //WTSClientProtocolType     = 16,
        //WTSIdleTime               = 17,
        //WTSLogonTime              = 18,
        //WTSIncomingBytes          = 19,
        //WTSOutgoingBytes          = 20,
        //WTSIncomingFrames         = 21,
        //WTSOutgoingFrames         = 22,
        //WTSClientInfo             = 23,
        //WTSSessionInfo            = 24,
        //WTSSessionInfoEx          = 25,
        //WTSConfigInfo             = 26,
        //WTSValidationInfo         = 27,
        //WTSSessionAddressV4       = 28,
        //WTSIsRemoteSession        = 29
    }

    /*
    /// <summary>
    /// these are constants in the Wtsapi32.h
    /// we converted them to a enum of type uint
    /// </summary>
    public enum WTS_SESSIONSTATE : uint
    {
        WTS_SESSIONSTATE_LOCK = 0,
        WTS_SESSIONSTATE_UNLOCK = 1,
        WTS_SESSIONSTATE_UNKNOWN = 4294967295
    }
    */

    #endregion

    #region RDS Win32 Helper class
    public sealed class Win32RDS
    {      

        #region Constants
        public const int WTS_CURRENT_SESSION = -1;  //Windows API Constant that defines the "current RDP Session"
        #endregion

        #region Win32 API calls
        [DllImport("Wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out string ppBuffer, out uint pBytesReturned);
                
        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern Int32 WTSEnumerateSessions(IntPtr hServer, [MarshalAs(UnmanagedType.U4)] Int32 Reserved, [MarshalAs(UnmanagedType.U4)] Int32 Version, ref IntPtr ppSessionInfo, [MarshalAs(UnmanagedType.U4)] ref Int32 pCount);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] String pServerName);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("Wtsapi32.dll")]
        static extern bool WTSQuerySessionInformation(System.IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);
        
        [DllImport("user32.dll")]
        static extern Boolean GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/aa383864%28v=vs.85%29.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WTS_SESSION_INFO
        {
            public Int32 SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public Int32 dwTime;
        }

        /*
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct LARGE_INTEGER
        {
            [FieldOffset(0)]
            public Int64 QuadPart;
            [FieldOffset(0)]
            public UInt32 LowPart;
            [FieldOffset(4)]
            public Int32 HighPart;
        }
        */

        /*
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/ee621017%28v=vs.85%29.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WTSINFOEX
        {
            public uint Level;
            public WTSINFOEX_LEVEL1 Data;
        }
        */

        /*
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/ee621019%28v=vs.85%29.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WTSINFOEX_LEVEL1
        { 
          public uint SessionId;
          public WTS_CONNECTSTATE_CLASS SessionState;
          public WTS_SESSIONSTATE SessionFlags;
          public String WinStationName;
          public String UserName;
          public String DomainName;
          public LARGE_INTEGER LogonTime;
          public LARGE_INTEGER ConnectTime;
          public LARGE_INTEGER DisconnectTime;
          public LARGE_INTEGER LastInputTime;
          public LARGE_INTEGER CurrentTime;
          public uint IncomingBytes;
          public uint OutgoingBytes;
          public uint IncomingFrames;
          public uint OutgoingFrames;
          public uint IncomingCompressedBytes;
          public uint OutgoingCompressedBytes;
        }
         */
        #endregion

        #region Private vars
        private string _username = string.Empty;   //RDP Session user name
        private int _usersessionid = -1;      //-1 is our value for "invalid". If the value is 0, then we have the default Windows OS "console" session on a host computer.
        private string _clientname = string.Empty;   //RDP session hostname (remote computer)
        private string _winstationname = string.Empty;  //Same
        private uint _sessionid = UInt32.MaxValue;   //this is not the same as the RDS User session ID. It's an idenfier that Windows OS creates.
        private string _domainname = string.Empty;    //Client Domain name
        private string _sessionState = string.Empty;  //Client session status
        private uint _idleTime = 0;   //Client session idle time
        #endregion

        #region static vars
        private readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;   //Handle (pointer)
        #endregion

        #region Constructors
        public Win32RDS()
        {
            this.QueryRDS();  //init Query RDS Server Host
        }
        #endregion

        #region Getters & setters
        public string RDSUsername
        {
            get { return this._username; }
            private set { this._username = value.ToLowerInvariant(); }
        }

        public int RDSUserSessionId
        {
            get { return this._usersessionid; }
            private set { this._usersessionid = value; }
        }

        public string RDSClientname
        {
            get { return this._clientname; }
            private set { this._clientname = value.ToUpperInvariant(); }
        }

        public string RDSDomainname
        {
            get { return this._domainname; }
            private set { this._domainname = value.ToUpperInvariant(); }
        }

        public string RDSWinStationname
        {
            get { return this._winstationname; }
            private set { this._winstationname = value.ToUpperInvariant(); }
        }

        public uint RDSSessionId
        {
            get { return this._sessionid; }
            private set { this._sessionid = value; }
        }

        public string RDSSessionState
        {
            get { return this._sessionState; }
            private set { this._sessionState = value.ToUpperInvariant(); }
        }

        public uint RDSUserIdleTime
        {
            get { return this._idleTime; }
            private set { this._idleTime = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Filter routine for WTS_INFO_CLASS. Only a few needed values we need are getting filtered.
        /// </summary>
        private void WriteValues(string wtsNameItem, string wtsValue)
        {
            switch (wtsNameItem)
            {
                case "WTSUserName":
                    this.RDSUsername = wtsValue;   
                    break;
                case "WTSClientName":
                    this.RDSClientname = wtsValue; //in case we are local logged on, value should be ""
                    break;
                case "WTSDomainName":
                    this.RDSDomainname = wtsValue; //AD or LDAP Domain name
                    break;
                case "WTSWinStationName":
                    this.RDSWinStationname = wtsValue; //in case we are local logged on, value should be "CONSOLE"
                    break;
                case "WTSSessionId":
                    uint id;

                    if (UInt32.TryParse(wtsValue, out id))
                    {
                        this.RDSSessionId = id;
                    }                    
                    break;
                case "WTSConnectState":
                    this.RDSSessionState = wtsValue;
                    this.UpdateSessionState();
                    break;
                default:
                    //Do nothing, leave the cow alone
                    break;
            }
        }

        /// <summary>
        /// Open connection to a RDS
        /// </summary>
        private IntPtr OpenServer(string hostname)
        {
            return WTSOpenServer(hostname);
        }

        /// <summary>
        /// Close connection from a RDS
        /// </summary>
        private void CloseServer(IntPtr ServerHandle)
        {
            WTSCloseServer(ServerHandle);
        }

        /// <summary>
        /// Console idle time (local machine only!!!)
        /// </summary>
        /// <returns>time in ms</returns>
        public static uint GetIdleTime()
        { 
            LASTINPUTINFO LastInput = new LASTINPUTINFO();

            uint idleTime = 0;
            LastInput.cbSize = (uint)Marshal.SizeOf(LastInput);
            LastInput.dwTime = 0;

            if (GetLastInputInfo(ref LastInput))
            {
                idleTime = (uint)(System.Environment.TickCount - LastInput.dwTime);
            }

            return idleTime;
        }

        /// <summary>
        /// Get the username from a RDP session ID
        /// </summary>
        public static string GetUserName(int sessionId, IntPtr server)
        {
            IntPtr buffer = IntPtr.Zero;
            uint count = 0;
            string userName = string.Empty;

            try
            {
                WTSQuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out count);
                userName = Marshal.PtrToStringAnsi(buffer).ToLowerInvariant().Trim();
            }
            finally
            {
                WTSFreeMemory(buffer);
            }

            return userName;
        }

        /// <summary>
        /// Build a list with all RDP user names and there session ID's from RDS server
        /// </summary>
        /*public Dictionary<string, int> GetUserSessionDictionary(IntPtr server, List<int> sessions)
        {
            Dictionary<string, int> userSession = new Dictionary<string, int>();

            foreach (var sessionId in sessions)
            {
                string uName = GetUserName(sessionId, server);

                if (!string.IsNullOrWhiteSpace(uName))
                {
                    userSession.Add(uName.ToLowerInvariant(), sessionId);
                }
            }
            return userSession;
        }*/

        /// <summary>
        /// Build a list with all RDP user names and there session ID's from RDS server
        /// </summary>
        public Dictionary<int, string> GetUserSessionDictionary(IntPtr server, List<int> sessions)
        {
            Dictionary<int, string> userSession = new Dictionary<int, string>();

            foreach (var sessionId in sessions)
            {
                string uName = GetUserName(sessionId, server);

                if (!string.IsNullOrWhiteSpace(uName))
                {
                    userSession.Add(sessionId, uName.ToLowerInvariant());
                }
            }
            return userSession;
        }

        /// <summary>
        /// Build a list with all known user ID's from RDS server
        /// </summary>
        public List<int> GetSessionIDs(IntPtr server)
        {
            List<int> sessionIds = new List<int>();
            IntPtr buffer = IntPtr.Zero;
            int count = 0;
            int retval = WTSEnumerateSessions(server, 0, 1, ref buffer, ref count);
            int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            Int64 current = (int)buffer;

            if (retval != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += dataSize;
                    sessionIds.Add(si.SessionID);
                }

                WTSFreeMemory(buffer);
            }

            return sessionIds;
        }

        /// <summary>
        /// Get a session's current state (connected, disconnected, idle, ...)
        /// </summary>
        private WTS_CONNECTSTATE_CLASS GetSessionState(int sessionId, IntPtr server)
        {
            WTS_CONNECTSTATE_CLASS state = WTS_CONNECTSTATE_CLASS.WTSInit;  // per default, we assume the session is down.

            IntPtr buffer = IntPtr.Zero;
            int count = 0;
            int retval = WTSEnumerateSessions(server, 0, 1, ref buffer, ref count);
            int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            Int64 current = (int)buffer;

            if (retval != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += dataSize;

                    if (si.SessionID == sessionId)
                    {
                        state = si.State;
                    }
                }

                WTSFreeMemory(buffer);
            }

            return state;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Filter from a list of session ID's on the RDS server, get the current user's session ID.
        /// </summary>
        private void GetCurrentUserSessionId()
        {
            this.RDSUserSessionId = -1;

            /*
            IntPtr server = this.OpenServer(Environment.MachineName);  //connect to localmachine name (the RDS server)

            List<int> sessionidlist = this.GetSessionIDs(server);  //get all session ID's on server
            //Dictionary<string, int> userlist = this.GetUserSessionDictionary(server, sessionidlist);   //get all associated user names from session ID's
            Dictionary<int, string> userlist = this.GetUserSessionDictionary(server, sessionidlist);

            this.CloseServer(server);  //close server connection
            */
            
            Dictionary<int, string> userlist = GetAllUserSessionIds();

            if (userlist != null)
            {
                //search current user session ID from our list based on the username we already have in cache.
                foreach (KeyValuePair<int, string> rdsUserInfo in userlist)
                {
                    if (rdsUserInfo.Value.ToLowerInvariant().Equals(this.RDSUsername.ToLowerInvariant()))
                    {
                        this.RDSUserSessionId = rdsUserInfo.Key;  //store session ID it once in memory place holder
                    }
                }
            }            
        }


        /// <summary>
        /// Get all current user session ID's.
        /// </summary>
        public Dictionary<int, string> GetAllUserSessionIds()
        {
            this.RDSUserSessionId = -1;
            Dictionary<int, string> userlist = null;

            IntPtr server = this.OpenServer(Environment.MachineName);  //connect to localmachine name (the RDS server)

            List<int> sessionidlist = this.GetSessionIDs(server);  //get all session ID's on server            
            userlist = this.GetUserSessionDictionary(server, sessionidlist);

            this.CloseServer(server);  //close server connection

            //usage 
            /*userlist = GetAllUserSessionIds()
            foreach (KeyValuePair<int, string> rdsUserInfo in userlist)
            {
                ...
            }*/

            return userlist;
        }

        /// <summary>
        /// Update session status for current user session ID
        /// </summary>
        public void UpdateSessionState()
        {
            WTS_CONNECTSTATE_CLASS sessionState = WTS_CONNECTSTATE_CLASS.WTSInit;  //Default to Down state

            try
            {
                IntPtr server = this.OpenServer(Environment.MachineName);  //connect to localmachine name (= the RDS server)

                sessionState = GetSessionState(this.RDSUserSessionId, server);    //Fetch our session ID state
                
                this.CloseServer(server);     //Close server connection
            }
            catch (Exception ex)
            { 
                //throw in garbadge
            }

            this.RDSSessionState = sessionState.ToString();  //update class status variable

            
        }

        /// <summary>
        /// Update (localhost) session status for current user logged on to Console
        /// </summary>
        public void UpdateSessionIdleTime()
        {
            this.RDSUserIdleTime = GetIdleTime(); //update idle time
        }

        /// <summary>
        /// Query the session
        /// </summary>
        public void QueryRDS()
        {
            String wtsValue = string.Empty;
            uint byteCount;

            foreach (WTS_INFO_CLASS wtsItem in Enum.GetValues(typeof(WTS_INFO_CLASS)))
            {
                WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, wtsItem, out wtsValue, out byteCount);

                WriteValues(wtsItem.ToString(), wtsValue);
            }

            //fetch current user SessionID (RDP Session ID) once and store it in memory for fast caching.
            this.GetCurrentUserSessionId();
        }
        #endregion
        
    }
    #endregion

}
