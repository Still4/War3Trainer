Attribute VB_Name = "OperMem"
Option Explicit

Public Declare Function OpenProcess Lib "Kernel32.dll" (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long
Public Declare Function ReadProcessMemory Lib "Kernel32.dll" (ByVal hProcess As Long, ByVal lpBaseAddress As Long, lpBuffer As Any, ByVal nSize As Long, lpNumberOfBytesRead As Long) As Long
Public Declare Function WriteProcessMemory Lib "Kernel32.dll" (ByVal hProcess As Long, ByVal lpBaseAddress As Long, lpBuffer As Any, ByVal nSize As Long, lpNumberOfBytesWritten As Any) As Long

Public Type MEMORY_BASIC_INFORMATION
    BaseAddress As Long
    AllocationBase As Long
    AllocattionProtect As Long
    RegionSize As Long
    State As Long
    Protect As Long
    Type As Long
End Type
Public Declare Function VirtualQueryEx Lib "Kernel32.dll" (ByVal hProcess As Long, ByVal lpAddress As Long, info As MEMORY_BASIC_INFORMATION, ByVal dwLength As Long) As Long
Public Declare Function CloseHandle Lib "Kernel32.dll" (ByVal Handle As Long) As Long

Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)

Public Const READ_CONTROL = &H20000
Public Const SYNCHRONIZE = &H100000
Public Const STANDARD_RIGHTS_ALL = &H1F0000
Public Const STANDARD_RIGHTS_EXECUTE = (READ_CONTROL)
Public Const STANDARD_RIGHTS_READ = (READ_CONTROL)
Public Const STANDARD_RIGHTS_REQUIRED = &HF0000
Public Const STANDARD_RIGHTS_WRITE = (READ_CONTROL)
Public Const PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED Or SYNCHRONIZE Or &HFFF
Public Const PROCESS_TERMINATE = &H1
Public Const PROCESS_VM_OPERATION = &H8
Public Const PROCESS_VM_READ = &H10
Public Const PROCESS_VM_WRITE = &H20
Public Const PROCESS_QUERY_INFORMATION = &H400

'权限提升
Private Declare Function GetCurrentProcess Lib "kernel32" () As Long
Private Declare Function LookupPrivilegeValue Lib "advapi32.dll" Alias "LookupPrivilegeValueA" (ByVal lpSystemName As String, ByVal lpName As String, lpLuid As LUID) As Long
Private Declare Function AdjustTokenPrivileges Lib "advapi32.dll" (ByVal TokenHandle As Long, ByVal DisableAllPrivileges As Long, NewState As TOKEN_PRIVILEGES, ByVal BufferLength As Long, PreviousState As TOKEN_PRIVILEGES, ReturnLength As Long) As Long
Private Declare Function OpenProcessToken Lib "advapi32.dll" (ByVal ProcessHandle As Long, ByVal DesiredAccess As Long, TokenHandle As Long) As Long

Private Const TOKEN_ASSIGN_PRIMARY = &H1
Private Const TOKEN_DUPLICATE = (&H2)
Private Const TOKEN_IMPERSONATE = (&H4)
Private Const TOKEN_QUERY = (&H8)
Private Const TOKEN_QUERY_SOURCE = (&H10)
Private Const TOKEN_ADJUST_PRIVILEGES = (&H20)
Private Const TOKEN_ADJUST_GROUPS = (&H40)
Private Const TOKEN_ADJUST_DEFAULT = (&H80)
Private Const TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED Or TOKEN_ASSIGN_PRIMARY Or _
                                TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or _
                                TOKEN_ADJUST_PRIVILEGES Or TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT)
Private Const SE_PRIVILEGE_ENABLED = &H2
Private Const ANYSIZE_ARRAY = 1

Private Type LUID
    lowpart As Long
    highpart As Long
End Type

Private Type LUID_AND_ATTRIBUTES
    pLuid As LUID
    Attributes As Long
End Type

Private Type TOKEN_PRIVILEGES
    PrivilegeCount As Long
    Privileges(ANYSIZE_ARRAY) As LUID_AND_ATTRIBUTES
End Type

'获取占用内存
Private Declare Function GetProcessMemoryInfo Lib "PSAPI.DLL" (ByVal hProcess As Long, ppsmemCounters As PROCESS_MEMORY_COUNTERS, ByVal cb As Long) As Long
Private Type PROCESS_MEMORY_COUNTERS
    cb As Long
    PageFaultCount As Long
    PeakWorkingSetSize As Long
    workingSetSize As Long
    QuotaPeakPagedPoolUsage As Long
    QuotaPagedPoolUsage As Long
    QuotaPeakNonPagedPoolUsage As Long
    QuotaNonPagedPoolUsage As Long
    PagefileUsage As Long
    PeakPagefileUsage As Long
End Type

Private FirstRWAddress As Long
Private Const BLOCKSIZE = 4096&

'提升权限为高
Public Function ToKen() As Boolean
    Dim hdlProcessHandle As Long
    Dim hdlTokenHandle As Long
    Dim tmpLuid As LUID
    Dim tkp As TOKEN_PRIVILEGES
    Dim tkpNewButIgnored As TOKEN_PRIVILEGES
    Dim lBufferNeeded As Long
    Dim lP As Long
    hdlProcessHandle = GetCurrentProcess()
    lP = OpenProcessToken(hdlProcessHandle, TOKEN_ALL_ACCESS, hdlTokenHandle)
    lP = LookupPrivilegeValue("", "SeDebugPrivilege", tmpLuid)
    tkp.PrivilegeCount = 1
    tkp.Privileges(0).pLuid = tmpLuid
    tkp.Privileges(0).Attributes = SE_PRIVILEGE_ENABLED
    lP = AdjustTokenPrivileges(hdlTokenHandle, False, tkp, Len(tkpNewButIgnored), tkpNewButIgnored, lBufferNeeded)
    ToKen = lP
End Function

'获取占用内存大小
Public Function GetMemorySize(ByVal lppid As Long) As Long
    Dim tPMC As PROCESS_MEMORY_COUNTERS
    Dim pHandle As Long ' 储存进程句柄
    pHandle = OpenProcess(PROCESS_ALL_ACCESS, False, lppid)
    GetProcessMemoryInfo pHandle, tPMC, Len(tPMC)
    CloseHandle pHandle
    GetMemorySize = tPMC.workingSetSize
End Function

Sub EditMem(ByVal hProcess As Long, ByVal FirstAddress As Long, St() As Byte)
    If FirstAddress < &H300000 Then Exit Sub
    Dim ProcessID As Long
    Dim r As Long
    Dim temp As Long
    Dim nSize As Integer
    
    ProcessID = hProcess
    hProcess = OpenProcess(PROCESS_ALL_ACCESS Or PROCESS_TERMINATE Or PROCESS_VM_OPERATION Or _
                           PROCESS_VM_READ Or PROCESS_VM_WRITE, False, ProcessID)
    If hProcess = 0 Then
       'MsgBox "OpenProcess Fail."
       Call frmMain.reScanGame
       Exit Sub
    End If
    
    nSize = UBound(St) - LBound(St) + 1
    r = WriteProcessMemory(hProcess, FirstAddress, St(LBound(St)), nSize, temp)
    If r = 0 Then
        'MsgBox "WriteProcessMemory Fail."
    End If
    CloseHandle hProcess
 
End Sub

Sub ReadMem(ByVal hProcess As Long, ByVal FirstAddress As Long, St() As Byte)
    If FirstAddress < &H300000 Then Exit Sub
    Dim ProcessID As Long
    Dim r As Long
    Dim temp As Long
    Dim nSize As Integer
    
    ProcessID = hProcess
    hProcess = OpenProcess(PROCESS_ALL_ACCESS, 0&, ProcessID)
    If hProcess = 0 Then
       'MsgBox "OpenProcess Fail."
       Call frmMain.reScanGame
       Exit Sub
    End If
    
    nSize = UBound(St) - LBound(St) + 1
    r = ReadProcessMemory(hProcess, FirstAddress, St(LBound(St)), nSize, temp)
    If r = 0 Then
        'MsgBox "ReadProcessMemory Fail."
    End If
    CloseHandle hProcess
End Sub

Public Function ReadMemInteger(ByVal hProcess As Long, ByVal FirstAddress As Long) As Long
    Dim Buffer(1 To 4) As Byte

    ReadMem hProcess, FirstAddress, Buffer
    Buffer(3) = 0
    Buffer(4) = 0
    CopyMemory ReadMemInteger, Buffer(1), 4
End Function

Public Function ReadMemLong(ByVal hProcess As Long, ByVal FirstAddress As Long) As Long
    Dim Buffer(1 To 4) As Byte

    ReadMem hProcess, FirstAddress, Buffer
    CopyMemory ReadMemLong, Buffer(1), 4
End Function

Public Function ReadMemFloat(ByVal hProcess As Long, ByVal FirstAddress As Long) As Single
    Dim Buffer(1 To 4) As Byte

    ReadMem hProcess, FirstAddress, Buffer
    CopyMemory ReadMemFloat, Buffer(1), 4
End Function

Public Function ReadMemChar4(ByVal hProcess As Long, ByVal FirstAddress As Long) As String
    Dim Buffer(1 To 4) As Byte
    Dim BufferInt As Long
    BufferInt = ReadMemLong(hProcess, FirstAddress)
    Buffer(1) = (BufferInt \ &H1000000) And &HFF
    Buffer(2) = (BufferInt \ &H10000) And &HFF
    Buffer(3) = (BufferInt \ &H100) And &HFF
    Buffer(4) = BufferInt And &HFF
    ReadMemChar4 = StrConv(Buffer, vbUnicode)
End Function

Public Sub WriteMemLong(ByVal hProcess As Long, ByVal FirstAddress As Long, ByVal Value As Long)
    Dim Buffer(1 To 4) As Byte
    CopyMemory Buffer(1), Value, 4

    EditMem hProcess, FirstAddress, Buffer
End Sub

Public Sub WriteMemFloat(ByVal hProcess As Long, ByVal FirstAddress As Long, ByVal Value As Single)
    Dim Buffer(1 To 4) As Byte
    CopyMemory Buffer(1), Value, 4

    EditMem hProcess, FirstAddress, Buffer
End Sub

Public Sub WriteMemChar4(ByVal hProcess As Long, ByVal FirstAddress As Long, ByVal Value As String)
    Dim Buffer1(1 To 4) As Byte
    Dim Buffer2() As Byte
    Buffer2 = StrConv(Value, vbFromUnicode)
    ReDim Preserve Buffer2(0 To 3)
    Buffer1(1) = Buffer2(0)
    Buffer1(2) = Buffer2(1)
    Buffer1(3) = Buffer2(2)
    Buffer1(4) = Buffer2(3)

    Dim BufferInt As Long
    BufferInt = &H1000000 * Buffer1(1) _
            + &H10000 * Buffer1(2) _
            + &H100& * Buffer1(3) _
            + Buffer1(4)
    
    WriteMemLong hProcess, FirstAddress, BufferInt
End Sub

