using System;
using System.Diagnostics;   // Process, FileVersionInfo

namespace War3Trainer
{
    /************************************************************************/
    /* Exceptions                                                           */
    /************************************************************************/
    #region Exception

    [Serializable]
    public class UnkonwnGameVersionExpection
        : Exception, System.Runtime.Serialization.ISerializable
    {
        public int ProcessId      { get; private set; }
        public string GameVersion { get; private set; }

        public UnkonwnGameVersionExpection()
            : base("Game version can not be recognized.")
        {
            
        }

        public UnkonwnGameVersionExpection(string message)
            : base(message)
        {
            
        }
        
        public UnkonwnGameVersionExpection(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }

        public UnkonwnGameVersionExpection(int processId, string gameVersion)
        {
            ProcessId = processId;
            GameVersion = gameVersion;
        }

        public UnkonwnGameVersionExpection(string message, int processId, string gameVersion) 
            : base(message)
        {
            ProcessId = processId;
            GameVersion = gameVersion;
        }

        public UnkonwnGameVersionExpection(string message, int processId, string gameVersion, Exception innerException)
            : base(message, innerException)
        {
            ProcessId = processId;
            GameVersion = gameVersion;
        }

        protected UnkonwnGameVersionExpection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            ProcessId = info.GetInt32("ProcessId");
            GameVersion = info.GetString("GameVersion");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("ProcessId", ProcessId, typeof(int));
            info.AddValue("GameVersion", GameVersion, typeof(string));
        }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (ProcessId != 0)
                {
                    return message
                        + string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "{0}ProcessId = {1}{0}GameVersion = {2}",
                            Environment.NewLine,
                            ProcessId,
                            GameVersion);
                }
                return message;
            }
        }
    }

    #endregion

    /************************************************************************/
    /* Context of War3Trainer                                               */
    /************************************************************************/
    class GameContext
    {
        public int ProcessId                      { get; private set; }
        public string ProcessVersion              { get; private set; }

        public UInt32 War3AddressThisGame         { get; private set; }
        public UInt32 War3AddressSelectedUnitList { get; private set; }
        public UInt32 War3AddressMoveSpeed        { get; private set; }

        public UInt32 War3OffsetUnitAttributes    { get; private set; }
        public UInt32 War3OffsetHeroAttributes    { get; private set; }
        public UInt32 War3OffsetGoodsList         { get; private set; }
        public UInt32 War3OffsetMoveSpeed         { get; private set; }

        private uint _moduleAddress;

        // Get a context if the game is running and recognized.
        // Returns null if not running.
        // Raise exception if game version is not recognized.
        public static GameContext FindGameRunning(string processName, string moduleName)
        {
            Process[] processesByName = Process.GetProcessesByName(processName);
            if (processesByName.Length > 0)
            {
                return new GameContext(processesByName[0], moduleName);
            }
            return null;
        }

        public GameContext(Process gameProcess, string moduleName)
        {
            // Get PID
            GetProcessInfo(gameProcess);

            // Find module
            GetModuleInfo(gameProcess, moduleName);

            // Decide addresses according to version
            GetGameAddress();

            // Decide offsets according to version
            GetGameOffset();
        }

        private void GetProcessInfo(Process gameProcess)
        {
            try
            {
                this.ProcessId = gameProcess.Id;
            }
            catch
            {
                throw new WindowsApi.BadProcessIdException(0);
            }
        }

        private void GetModuleInfo(Process gameProcess, string moduleName)
        {
            ProcessModule mainModule = null;
            foreach (ProcessModule module in gameProcess.Modules)
            {
                // Find module
                if (module.ModuleName.ToLower() != moduleName.ToLower())
                    continue;

                // Get version
                // Example: "1, 22, 0, 6328"
                string fileVersion = module.FileVersionInfo.FileVersion;
                if (fileVersion == null)
                    continue;
                this.ProcessVersion = fileVersion.Replace(", ", ".");

                // Save
                mainModule = module;
                break;
            }
            if (mainModule == null)
            {
                // Main module not found
                throw new WindowsApi.BadProcessIdException(this.ProcessId);
            }

            // Base address of module
            _moduleAddress = (uint)mainModule.BaseAddress;
        }

        private void GetGameAddress()
        {
            switch (ProcessVersion)
            {
                case "1.20.4.6074":
                    War3AddressThisGame         = _moduleAddress + 0x87C744;
                    War3AddressSelectedUnitList = _moduleAddress + 0x8722BC;
                    War3AddressMoveSpeed        = _moduleAddress + 0x55BDF0;
                    break;
                case "1.21.0.6263":
                    War3AddressThisGame         = _moduleAddress + 0x87D7BC;
                    War3AddressSelectedUnitList = _moduleAddress + 0x873334;
                    War3AddressMoveSpeed        = _moduleAddress + 0x55FE80;
                    break;
                case "1.21.1.6300":
                    War3AddressThisGame         = _moduleAddress + 0x87D7BC;
                    War3AddressSelectedUnitList = _moduleAddress + 0x873334;
                    War3AddressMoveSpeed        = _moduleAddress + 0x55fEA0;
                    break;
                case "1.22.0.6328":
                    War3AddressThisGame         = _moduleAddress + 0xAA4178;
                    War3AddressSelectedUnitList = _moduleAddress + 0xAA2FFC;
                    War3AddressMoveSpeed        = _moduleAddress + 0x201190;
                    break;
                case "1.23.0.6352":
                    War3AddressThisGame         = _moduleAddress + 0xABCFC8;
                    War3AddressSelectedUnitList = _moduleAddress + 0xABBE4C;
                    War3AddressMoveSpeed        = _moduleAddress + 0x2026D0;
                    break;
                case "1.24.0.6372":
                case "1.24.1.6374":
                case "1.24.2.6378":
                case "1.24.3.6384":
                    War3AddressThisGame         = _moduleAddress + 0xACE5E0;
                    War3AddressSelectedUnitList = _moduleAddress + 0xACD44C;
                    War3AddressMoveSpeed        = _moduleAddress + 0x202780;
                    break;
                case "1.24.4.6387":
                    War3AddressThisGame         = _moduleAddress + 0xACE5E0;
                    War3AddressSelectedUnitList = _moduleAddress + 0xACD44C;
                    War3AddressMoveSpeed        = _moduleAddress + 0x2027E0;
                    break;
                default:
                    throw new UnkonwnGameVersionExpection(
                        this.ProcessId,
                        ProcessVersion);
            }
        }

        private void GetGameOffset()
        {
            switch (ProcessVersion)
            {
                case "1.20.4.6074":
                case "1.21.0.6263":
                case "1.21.1.6300":
                case "1.22.0.6328":
                case "1.23.0.6352":
                case "1.24.0.6372":
                case "1.24.1.6374":
                case "1.24.2.6378":
                    War3OffsetUnitAttributes = 0x1E4;
                    War3OffsetHeroAttributes = 0x1EC;
                    War3OffsetGoodsList      = 0x1F4;
                    War3OffsetMoveSpeed      = 0x1D8;
                    break;
                case "1.24.3.6384":
                case "1.24.4.6387":
                    War3OffsetUnitAttributes = 0x1E8;
                    War3OffsetHeroAttributes = 0x1F0;
                    War3OffsetGoodsList      = 0x1F8;
                    War3OffsetMoveSpeed      = 0x1DC;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false, "Immposible to run here");
                    break;
            }
        }
    }
}
