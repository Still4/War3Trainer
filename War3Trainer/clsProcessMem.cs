using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   // DllImport

namespace WindowsApi
{
    /************************************************************************/
    /* Token                                                                */
    /************************************************************************/
    using LUID = Int64;
    public class ProcessToken
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public int Attributes;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
            [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            UInt32 Zero,
            IntPtr Null1,
            IntPtr Null2);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            int DesiredAccess,
            ref IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(
            string lpSystemName,
            string lpName,
            ref LUID lpLuid);
        [DllImport("kernel32.dll")]
        internal static extern Int32 CloseHandle(IntPtr hObject);

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_PRIVILEGE_NAMETEXT = "SeDebugPrivilege";

        //////////////////////////////////////////////////////////////////////////
        public static bool SetPrivilege()
        {
            TOKEN_PRIVILEGES tmpKP = new TOKEN_PRIVILEGES{
                                         PrivilegeCount = 1,
                                         Privileges = new LUID_AND_ATTRIBUTES[1]
                                         {
                                             new LUID_AND_ATTRIBUTES
                                             {
                                                Luid = 0,
                                                Attributes = SE_PRIVILEGE_ENABLED
                                             }                                             
                                         }
                                     };
            bool retVal;
            IntPtr hdlProcessHandle = IntPtr.Zero;
            IntPtr hdlTokenHandle = IntPtr.Zero;
            try
            {
                hdlProcessHandle = GetCurrentProcess();
                retVal = OpenProcessToken(hdlProcessHandle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hdlTokenHandle);
                retVal = LookupPrivilegeValue(null, SE_PRIVILEGE_NAMETEXT, ref tmpKP.Privileges[0].Luid);
                retVal = AdjustTokenPrivileges(
                    hdlTokenHandle,
                    false,
                    ref tmpKP,
                    0,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
            finally
            {
                CloseHandle(hdlProcessHandle);
                CloseHandle(hdlTokenHandle);
            }   
            return retVal;
        }
    }

    /************************************************************************/
    /* Memory of extern process                                             */
    /************************************************************************/
    class BadProcessIdException
        : ApplicationException
    {

    }

    public class clsProcessMemory
        : IDisposable
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        internal static extern Int32  ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
                                                        UInt32 nSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        internal static extern Int32  WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
                                                         UInt32 nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        internal static extern Int32  CloseHandle(IntPtr hObject);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            None                = 0,
            Terminate           = 0x00000001,
            CreateThread        = 0x00000002,
            VMOperation         = 0x00000008,
            VMRead              = 0x00000010,
            VMWrite             = 0x00000020,
            DupHandle           = 0x00000040,
            SetInformation      = 0x00000200,
            QueryInformation    = 0x00000400,
            Synchronize         = 0x00100000,
            All                 = 0x001F0FFF
        }

        //////////////////////////////////////////////////////////////////////////
        public readonly IntPtr hProcess = IntPtr.Zero;
        private bool isDisposed = false;

        public clsProcessMemory(int ProcessID)
        {
            System.Diagnostics.Process MyProc;
            try
            {
                MyProc = System.Diagnostics.Process.GetProcessById(ProcessID);
            }
            catch
            {
                throw new BadProcessIdException();
            }
            if (MyProc.HandleCount > 0)
                hProcess = OpenProcess(
                    (uint)ProcessAccessFlags.All,
                    false,
                    ProcessID);
            else
                throw new BadProcessIdException();
        }

        ~clsProcessMemory()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (isDisposing)
                {
                    // TODO: Free managed resources here
                }
                CloseHandle(hProcess);
            }
        }

        public byte[] ReadBytes(IntPtr BaseAddress, UInt32 Size, out IntPtr BytesRead)
        {
            byte[] Buffer = new byte[Size];
            if (hProcess == IntPtr.Zero)
                BytesRead = IntPtr.Zero;
            else
                ReadProcessMemory(hProcess, BaseAddress, Buffer, Size, out BytesRead);
            return Buffer;
        }

        public void WriteBytes(byte[] Buffer, IntPtr BaseAddress, UInt32 Size, out IntPtr BytesWriten)
        {
            if (hProcess == IntPtr.Zero)
                BytesWriten = IntPtr.Zero;
            else
                WriteProcessMemory(hProcess, BaseAddress, Buffer, Size, out BytesWriten);
        }

        //////////////////////////////////////////////////////////////////////////
        public float ReadFloat(IntPtr BaseAddress)
        {
            IntPtr BytesRead;
            byte[] Buffer = ReadBytes(BaseAddress, sizeof(float), out BytesRead);
            return BitConverter.ToSingle(Buffer, 0);
        }

        public void WriteFloat(IntPtr BaseAddress, float Value)
        {
            IntPtr BytesWriten;
            WriteBytes(
                BitConverter.GetBytes(Value),
                BaseAddress, sizeof(float), out BytesWriten);
        }

        public Int32 ReadInt32(IntPtr BaseAddress)
        {
            IntPtr BytesRead;
            byte[] Buffer = ReadBytes(BaseAddress, sizeof(Int32), out BytesRead);
            return BitConverter.ToInt32(Buffer, 0);
        }

        public void WriteInt32(IntPtr BaseAddress, Int32 Value)
        {
            IntPtr BytesWriten;
            WriteBytes(
                BitConverter.GetBytes(Value),
                BaseAddress, sizeof(Int32), out BytesWriten);
        }

        public UInt32 ReadUInt32(IntPtr BaseAddress)
        {
            IntPtr BytesRead;
            byte[] Buffer = ReadBytes(BaseAddress, sizeof(Int32), out BytesRead);
            return BitConverter.ToUInt32(Buffer, 0);
        }

        public void WriteUInt32(IntPtr BaseAddress, UInt32 Value)
        {
            IntPtr BytesWriten;
            WriteBytes(
                BitConverter.GetBytes(Value),
                BaseAddress, sizeof(UInt32), out BytesWriten);
        }

        public UInt16 ReadUInt16(IntPtr BaseAddress)
        {
            IntPtr BytesRead;
            byte[] Buffer = ReadBytes(BaseAddress, sizeof(Int16), out BytesRead);
            return BitConverter.ToUInt16(Buffer, 0);
        }

        public void WriteUInt16(IntPtr BaseAddress, UInt16 Value)
        {
            IntPtr BytesWriten;
            WriteBytes(
                BitConverter.GetBytes(Value),
                BaseAddress, sizeof(UInt16), out BytesWriten);
        }

        public string ReadChar4(IntPtr BaseAddress)
        {
            IntPtr BytesRead;
            byte[] Buffer = ReadBytes(BaseAddress, 4, out BytesRead);
            return Encoding.ASCII.GetString(
                new byte[]
                {
                    Buffer[3],
                    Buffer[2],
                    Buffer[1],
                    Buffer[0]
                });
        }

        public void WriteChar4(IntPtr BaseAddress, string Value)
        {
            IntPtr BytesWriten;
            if (Value.Length < 4)
                Value += new string('\0', 4 - Value.Length);
            byte[] Buffer = Encoding.ASCII.GetBytes(Value);
            WriteBytes(
                new byte[]
                {
                    Buffer[3],
                    Buffer[2],
                    Buffer[1],
                    Buffer[0]
                },
                BaseAddress, 4, out BytesWriten);
        }
    }
}
