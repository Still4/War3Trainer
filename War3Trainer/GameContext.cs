using System;
using System.Diagnostics;   // Process, FileVersionInfo

namespace War3Trainer
{
    // Exceptions
    public class UnkonwnGameVersionExpection
        : ApplicationException
    {
        public readonly int ProcessId;
        public readonly string FileVersion;

        public UnkonwnGameVersionExpection(
            int processId,
            string fileVersion)
        {
            ProcessId = processId;
            FileVersion = fileVersion;
        }
    }

    // This is the context of War3Trainer
    class GameContext
    {
        public readonly int ProcessId;      // PID
        public readonly string ProcessVersion;

        public readonly UInt32 War3AddressThisGame;
        public readonly UInt32 War3AddressSelectedUnitList;
        public readonly UInt32 War3AddressMoveSpeed;

        public readonly UInt32 War3OffsetUnitAttributes;
        public readonly UInt32 War3OffsetHeroAttributes;
        public readonly UInt32 War3OffsetGoodsList;
        public readonly UInt32 War3OffsetMoveSpeed;

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

        // Unknown game version will cause ctor() failure
        public GameContext(Process gameProcess, string moduleName)
        {
            // Get PID
            try
            {
                this.ProcessId = gameProcess.Id;
            }
            catch
            {
                throw new WindowsApi.BadProcessIdException(0);
            }

            // Find module
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
            uint baseAddress = (uint)mainModule.BaseAddress;

            // Decide addresses according to version
            switch (ProcessVersion)
            {
                case "1.20.4.6074":
                    War3AddressThisGame         = baseAddress + 0x87C744;
                    War3AddressSelectedUnitList = baseAddress + 0x8722BC;
                    War3AddressMoveSpeed        = baseAddress + 0x55BDF0;
                    break;
                case "1.21.0.6263":
                    War3AddressThisGame         = baseAddress + 0x87D7BC;
                    War3AddressSelectedUnitList = baseAddress + 0x873334;
                    War3AddressMoveSpeed        = baseAddress + 0x55FE80;
                    break;
                case "1.21.1.6300":
                    War3AddressThisGame         = baseAddress + 0x87D7BC;
                    War3AddressSelectedUnitList = baseAddress + 0x873334;
                    War3AddressMoveSpeed        = baseAddress + 0x55fEA0;
                    break;
                case "1.22.0.6328":
                    War3AddressThisGame         = baseAddress + 0xAA4178;
                    War3AddressSelectedUnitList = baseAddress + 0xAA2FFC;
                    War3AddressMoveSpeed        = baseAddress + 0x201190;
                    break;
                case "1.23.0.6352":
                    War3AddressThisGame         = baseAddress + 0xABCFC8;
                    War3AddressSelectedUnitList = baseAddress + 0xABBE4C;
                    War3AddressMoveSpeed        = baseAddress + 0x2026D0;
                    break;
                case "1.24.0.6372":
                case "1.24.1.6374":
                case "1.24.2.6378":
                case "1.24.3.6384":
                    War3AddressThisGame         = baseAddress + 0xACE5E0;
                    War3AddressSelectedUnitList = baseAddress + 0xACD44C;
                    War3AddressMoveSpeed        = baseAddress + 0x202780;
                    break;
                case "1.24.4.6387":
                    War3AddressThisGame         = baseAddress + 0xACE5E0;
                    War3AddressSelectedUnitList = baseAddress + 0xACD44C;
                    War3AddressMoveSpeed        = baseAddress + 0x2027E0;
                    break;
                default:
                    throw new UnkonwnGameVersionExpection(
                        this.ProcessId,
                        ProcessVersion);
            }

            // Decide offsets according to version
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
