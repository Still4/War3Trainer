using System;
using System.Text;
using System.Runtime.InteropServices;   // DllImport

namespace WindowsApi
{
    /************************************************************************/
    /* Memory of extern process                                             */
    /************************************************************************/
    public class BadProcessIdException
        : ApplicationException
    {
        public readonly int ProcessId;

        public BadProcessIdException(int processId)
        {
            ProcessId = processId;
        }
    }

    public class ProcessMemory
        : IDisposable
    {
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
            
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern Int32  ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
                                                            UInt32 nSize, out IntPtr lpNumberOfBytesRead);
            
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
                                                            UInt32 nSize, out IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool CloseHandle(IntPtr hObject);

            [Flags]
            internal enum ProcessAccessFlags : uint
            {
                None = 0,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VMOperation = 0x00000008,
                VMRead = 0x00000010,
                VMWrite = 0x00000020,
                DupHandle = 0x00000040,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                Synchronize = 0x00100000,
                All = 0x001F0FFF
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private readonly IntPtr processHandle = IntPtr.Zero;
        private bool isDisposed;

        public ProcessMemory(int processId)
        {
            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.GetProcessById(processId);
            }
            catch
            {
                throw new BadProcessIdException(processId);
            }

            if (process.HandleCount > 0)
                processHandle = NativeMethods.OpenProcess(
                    (uint)NativeMethods.ProcessAccessFlags.All,
                    false,
                    processId);
            else
                throw new BadProcessIdException(processId);
        }

        ~ProcessMemory()
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
                    // Free managed resources here
                }

                // Free unmanaged resources here
                NativeMethods.CloseHandle(processHandle);
            }
        }

        public byte[] ReadBytes(IntPtr baseAddress, UInt32 size, out IntPtr bytesRead)
        {
            byte[] Buffer = new byte[size];
            if (processHandle == IntPtr.Zero)
                bytesRead = IntPtr.Zero;
            else
                NativeMethods.ReadProcessMemory(processHandle, baseAddress, Buffer, size, out bytesRead);
            return Buffer;
        }

        public void WriteBytes(byte[] buffer, IntPtr baseAddress, UInt32 size, out IntPtr bytesWriten)
        {
            if (processHandle == IntPtr.Zero)
                bytesWriten = IntPtr.Zero;
            else
                NativeMethods.WriteProcessMemory(processHandle, baseAddress, buffer, size, out bytesWriten);
        }

        //////////////////////////////////////////////////////////////////////////
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public float ReadFloat(IntPtr baseAddress)
        {
            IntPtr bytesRead;
            byte[] Buffer = ReadBytes(baseAddress, sizeof(float), out bytesRead);
            return BitConverter.ToSingle(Buffer, 0);
        }

        public void WriteFloat(IntPtr baseAddress, float value)
        {
            IntPtr bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(float), out bytesWriten);
        }

        public Int32 ReadInt32(IntPtr baseAddress)
        {
            IntPtr bytesRead;
            byte[] Buffer = ReadBytes(baseAddress, sizeof(Int32), out bytesRead);
            return BitConverter.ToInt32(Buffer, 0);
        }

        public void WriteInt32(IntPtr baseAddress, Int32 value)
        {
            IntPtr bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(Int32), out bytesWriten);
        }

        public UInt32 ReadUInt32(IntPtr baseAddress)
        {
            IntPtr bytesRead;
            byte[] Buffer = ReadBytes(baseAddress, sizeof(Int32), out bytesRead);
            return BitConverter.ToUInt32(Buffer, 0);
        }

        public void WriteUInt32(IntPtr baseAddress, UInt32 value)
        {
            IntPtr bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(UInt32), out bytesWriten);
        }

        public UInt16 ReadUInt16(IntPtr baseAddress)
        {
            IntPtr bytesRead;
            byte[] Buffer = ReadBytes(baseAddress, sizeof(Int16), out bytesRead);
            return BitConverter.ToUInt16(Buffer, 0);
        }

        public void WriteUInt16(IntPtr baseAddress, UInt16 value)
        {
            IntPtr bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(UInt16), out bytesWriten);
        }

        public string ReadChar4(IntPtr baseAddress)
        {
            IntPtr bytesRead;
            byte[] buffer = ReadBytes(baseAddress, 4, out bytesRead);
            return Encoding.ASCII.GetString(
                new byte[]
                {
                    buffer[3],
                    buffer[2],
                    buffer[1],
                    buffer[0]
                });
        }

        public void WriteChar4(IntPtr baseAddress, string value)
        {
            IntPtr bytesWriten;
            if (value.Length < 4)
                value += new string('\0', 4 - value.Length);
            byte[] Buffer = Encoding.ASCII.GetBytes(value);
            WriteBytes(
                new byte[]
                {
                    Buffer[3],
                    Buffer[2],
                    Buffer[1],
                    Buffer[0]
                },
                baseAddress, 4, out bytesWriten);
        }
    }
}
