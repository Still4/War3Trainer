using System;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace WindowsApi
{
    /************************************************************************/
    /* Exceptions                                                           */
    /************************************************************************/
    #region Exception

    [Serializable]
    public class BadProcessIdException
        : Exception, ISerializable
    {
        public int ProcessId { get; private set; }

        public BadProcessIdException()
            : base("Process ID can not be feteched or incorrect.")
        {
            
        }

        public BadProcessIdException(string message)
            : base(message)
        {
            
        }
        
        public BadProcessIdException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }

        public BadProcessIdException(int processId)
        {
            ProcessId = processId;
        }

        public BadProcessIdException(string message, int processId) 
            : base(message)
        {
            ProcessId = processId;
        }

        public BadProcessIdException(string message, int processId, Exception innerException)
            : base(message, innerException)
        {
            ProcessId = processId;
        }

        protected BadProcessIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ProcessId = info.GetInt32("ProcessId");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("ProcessId", ProcessId, typeof(int));
        }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (ProcessId != 0)
                {
                    return message
                        + Environment.NewLine
                        + string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "ProcessId = {0}",
                            ProcessId);
                }
                return message;
            }
        }
    }

    #endregion

    /************************************************************************/
    /* Memory of extern process                                             */
    /************************************************************************/
    public class ProcessMemory
        : IDisposable
    {
        #region Windows API

        [SuppressUnmanagedCodeSecurity()]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern SafeProcessHandle OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
            
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool ReadProcessMemory(SafeProcessHandle hProcess, IntPtr lpBaseAddress, [In, Out] byte[] lpBuffer,
                                                          UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);
            
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool WriteProcessMemory(SafeProcessHandle hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
                                                           UIntPtr nSize, out UIntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
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

        #endregion

        #region Process handle

        [SuppressUnmanagedCodeSecurity]
        [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
        private sealed class SafeProcessHandle
            : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            internal static SafeProcessHandle InvalidHandle = new SafeProcessHandle(IntPtr.Zero);

            internal SafeProcessHandle() : base(true)
            {
            }

            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            internal SafeProcessHandle(IntPtr handle) : base(true)
            {
                base.SetHandle(handle);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            internal void InitialSetHandle(IntPtr initialValue)
            {
                base.handle = initialValue;
            }

            protected override bool ReleaseHandle()
            {
                return NativeMethods.CloseHandle(base.handle);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////
        private readonly int _processId;
        private readonly SafeProcessHandle _processHandle;

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public ProcessMemory(int processId)
        {
            // Save paramter
            _processId = processId;
            
            // Open handle
            _processHandle = NativeMethods.OpenProcess(
                (uint)NativeMethods.ProcessAccessFlags.All,
                false,
                _processId);
            if (_processHandle.IsInvalid)
                throw new BadProcessIdException(_processId);
        }

        private bool _isDisposed;

        #region IDisposable

        ~ProcessMemory()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (isDisposing)
                {
                    // Free managed resources here
                    if (_processHandle != null && !_processHandle.IsInvalid)
                    {
                        // Free the handle
                        _processHandle.Dispose();
                    }
                }

                // Free unmanaged resources here
            }
        }

        #endregion

        #region Core R/W

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public byte[] ReadBytes(IntPtr baseAddress, int size, out int bytesRead)
        {
            byte[] buffer = new byte[size];
            UIntPtr apiSize = (UIntPtr)size, apiBytesRead;
            if (!NativeMethods.ReadProcessMemory(
                _processHandle,
                baseAddress,
                buffer,
                apiSize,
                out apiBytesRead))
            {
                throw new BadProcessIdException(_processId);
            }
            bytesRead = (int)apiBytesRead;
            return buffer;
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void WriteBytes(byte[] buffer, IntPtr baseAddress, int size, out int bytesWriten)
        {
            UIntPtr apiSize = (UIntPtr)size, apiBytesWriten;
            if (!NativeMethods.WriteProcessMemory(
                _processHandle,
                baseAddress,
                buffer,
                apiSize,
                out apiBytesWriten))
            {
                throw new BadProcessIdException(_processId);
            }
            bytesWriten = (int)apiBytesWriten;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////
        // Access helper
        #region int/float/string R/W

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public float ReadFloat(IntPtr baseAddress)
        {
            int bytesRead;
            byte[] buffer = ReadBytes(baseAddress, sizeof(float), out bytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void WriteFloat(IntPtr baseAddress, float value)
        {
            int bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(float), out bytesWriten);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public Int32 ReadInt32(IntPtr baseAddress)
        {
            int bytesRead;
            byte[] buffer = ReadBytes(baseAddress, sizeof(Int32), out bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void WriteInt32(IntPtr baseAddress, Int32 value)
        {
            int bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(Int32), out bytesWriten);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public UInt32 ReadUInt32(IntPtr baseAddress)
        {
            int bytesRead;
            byte[] buffer = ReadBytes(baseAddress, sizeof(Int32), out bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void WriteUInt32(IntPtr baseAddress, UInt32 value)
        {
            int bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(UInt32), out bytesWriten);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public UInt16 ReadUInt16(IntPtr baseAddress)
        {
            int bytesRead;
            byte[] buffer = ReadBytes(baseAddress, sizeof(Int16), out bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void WriteUInt16(IntPtr baseAddress, UInt16 value)
        {
            int bytesWriten;
            WriteBytes(
                BitConverter.GetBytes(value),
                baseAddress, sizeof(UInt16), out bytesWriten);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public string ReadChar4(IntPtr baseAddress)
        {
            int bytesRead;
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

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void WriteChar4(IntPtr baseAddress, string value)
        {
            int bytesWriten;
            if (value.Length < 4)
                value += new string('\0', 4 - value.Length);
            byte[] buffer = Encoding.ASCII.GetBytes(value);
            WriteBytes(
                new byte[]
                {
                    buffer[3],
                    buffer[2],
                    buffer[1],
                    buffer[0]
                },
                baseAddress, 4, out bytesWriten);
        }

        #endregion
    }
}
