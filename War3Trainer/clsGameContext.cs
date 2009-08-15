using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;   // Process, FileVersionInfo

namespace War3Trainer
{
    // Exceptions
    class UnkonwnGameVersionExpection
        : ApplicationException
    {
        private int _ProcessId;
        private string _FileVersion;

        public UnkonwnGameVersionExpection(
            int ProcessId,
            string FileVersion)
        {
            _ProcessId = ProcessId;
            _FileVersion = FileVersion;
        }

        public string FileVersion
        {
            get
            {
                return _FileVersion;
            }
        }

        public int ProcessId
        {
            get
            {
                return _ProcessId;
            }
        }
    }

    // This is the context of clsWar3Trainer
    class clsGameContext
    {
        public readonly int    ProcessID;      // PID
        public readonly string ProcessVersion;

        public readonly UInt32 War3AddressThisGame;
        public readonly UInt32 War3AddressSelectedUnitList;
        public readonly UInt32 War3AddressMoveSpeed;

        // Get a context if the game is running and recognized.
        // Returns null if not running.
        // Raise exception if game version is not recognized.
        public static clsGameContext FindGameRunning(string ProcessName)
        {
            foreach (Process MyProc in Process.GetProcesses())
            {
                if (MyProc.ProcessName.ToLower() == ProcessName.ToLower() &&
                    MyProc.HandleCount > 0)
                    return new clsGameContext(MyProc);
            }
            return null;
        }

        // Unknown game version will cause ctor() failure
        public clsGameContext(Process GameProcess)
        {
            this.ProcessID = GameProcess.Id;
            ProcessModuleCollection myProcessModuleCollection = GameProcess.Modules;

            if (myProcessModuleCollection.Count > 0)
            {
                FileVersionInfo myFileInfo = myProcessModuleCollection[0].FileVersionInfo;
                ProcessVersion = myFileInfo.FileVersion.Replace(", ", "."); // ProcessVersion.FileVersion sample: "1, 22, 0, 6328"
                switch (ProcessVersion)
                {
                    case "1.20.4.6074":
                        War3AddressThisGame         = 0x6F87C744;
                        War3AddressSelectedUnitList = 0x6F8722BC;
                        War3AddressMoveSpeed        = 0x6F55BDF0;
                        break;
                    case "1.21.0.6263":
                        War3AddressThisGame         = 0x6F87D7BC;
                        War3AddressSelectedUnitList = 0x6F873334;
                        War3AddressMoveSpeed        = 0x6F55FE80;
                        break;
                    case "1.21.1.6300":
                        War3AddressThisGame         = 0x6F87D7BC;
                        War3AddressSelectedUnitList = 0x6F873334;
                        War3AddressMoveSpeed        = 0x6F55fEA0;
                        break;
                    case "1.22.0.6328":
                        War3AddressThisGame         = 0x6FAA4178;
                        War3AddressSelectedUnitList = 0x6FAA2FFC;
                        War3AddressMoveSpeed        = 0x6F201190;
                        break;
                    case "1.23.0.6352":
                        War3AddressThisGame         = 0x6FABCFC8;
                        War3AddressSelectedUnitList = 0x6FABBE4C;
                        War3AddressMoveSpeed        = 0x6F2026D0;
                        break;
                    case "1.24.0.6372":
                        War3AddressThisGame         = 0x6FACE5E0;
                        War3AddressSelectedUnitList = 0x6FACD44C;
                        War3AddressMoveSpeed        = 0x6F202780;
                        break;
                    default:
                        throw new UnkonwnGameVersionExpection(
                            this.ProcessID,
                            ProcessVersion);
                }
            }
            else
                throw new WindowsApi.BadProcessIdException();
        }
    }

    static class War3Common
    {
        // Game memory init
        public static void GetGameMemory(
            clsGameContext GameContext,
            ref NewChildrenEventArgs GameAddress)
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(GameContext.ProcessID))
            {
                GameAddress.pThisGame = Mem.ReadUInt32((IntPtr)GameContext.War3AddressThisGame);
                if (GameAddress.pThisGame != 0)
                {
                    GameAddress.pThisGameMemory = Mem.ReadUInt32((IntPtr)unchecked(GameAddress.pThisGame + 0xC));
                    if (GameAddress.pThisGameMemory == 0xFFFFFFFF)
                        GameAddress.pThisGameMemory = 0;
                }

                if (GameAddress.pThisGame == 0
                    || GameAddress.pThisGameMemory == 0)
                {
                    GameAddress.pThisGameMemory = 0;
                    GameAddress.pThisUnit = 0;
                    GameAddress.pUnitAttributes = 0;
                    GameAddress.pHeroAttributes = 0;
                }
            }
        }

        // Game memory extract algorithm 0 
        public static UInt32 ReadFromGameMemory(
            WindowsApi.clsProcessMemory Mem, clsGameContext GameContext, NewChildrenEventArgs GameAddress,
            Int32 nIndex)
        {
            System.Diagnostics.Debug.Assert(nIndex >= 0);

            if (GameAddress.pThisGameMemory == 0)
                return 0;

            return Mem.ReadUInt32((IntPtr)unchecked(GameAddress.pThisGameMemory + nIndex * 8 + 4));
        }

        // Game memory extract algorithm 1
        // Used in Intelligence
        public static UInt32 ReadGameValue1(
            WindowsApi.clsProcessMemory Mem, clsGameContext GameContext, NewChildrenEventArgs GameAddress,
            Int32 nIndex)
        {
            if (GameAddress.pThisGameMemory == 0)
                return 0;

            return unchecked(0x78 + ReadFromGameMemory(
                Mem, GameContext, GameAddress,
                nIndex));
        }

        // Game memory extract algorithm 2
        // Used in MoveSpeed
        public static UInt32 ReadGameValue2(
            WindowsApi.clsProcessMemory Mem, clsGameContext GameContext, NewChildrenEventArgs GameAddress,
            Int32 nIndex)
        {
            if (GameAddress.pThisGameMemory == 0)
                return 0;

            UInt32 tmpValue = ReadFromGameMemory(
                Mem, GameContext, GameAddress,
                nIndex);
            if (0 == Mem.ReadUInt32((IntPtr)unchecked(tmpValue + 0x20)))
            {
                return Mem.ReadUInt32((IntPtr)unchecked(tmpValue + 0x54));
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(
                    "Bug detected in ReadGameValue2().",
                    "Bug check");
                // Copy sub_6F468A20() again, set breakpoint at 6F0776F6
                return 0;
            }
        }
    }
}

// How to get (WindowsApi.clsProcessMemory Mem):
// using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(GameContext.ProcessID))
