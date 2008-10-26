Attribute VB_Name = "OperProc"
Option Explicit

Private Declare Function CreateToolhelp32Snapshot Lib "kernel32" (ByVal dwFlags As Long, ByVal th32ProcessID As Long) As Long
Private Declare Function Process32First Lib "kernel32" (ByVal hSnapShot As Long, lppe As PROCESSENTRY32) As Long
Private Declare Function Process32Next Lib "kernel32" (ByVal hSnapShot As Long, lppe As PROCESSENTRY32) As Long
Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
Public Declare Function EnumProcessModules Lib "PSAPI.DLL" ( _
   ByVal hProcess As Long, _
   hModule As Long, _
   ByVal cb As Long, _
   cbNeeded As Long _
) As Long
Public Declare Function GetModuleFileNameEx Lib "PSAPI.DLL" Alias "GetModuleFileNameExA" ( _
   ByVal hProcess As Long, _
   ByVal hModule As Long, _
   ByVal lpFileName As String, _
   ByVal nSize As Long _
) As Long
Public Declare Function GetModuleBaseName Lib "PSAPI.DLL" Alias "GetModuleBaseNameA" ( _
   ByVal hProcess As Long, _
   ByVal hModule As Long, _
   ByVal lpBaseName As String, _
   ByVal nSize As Long _
) As Long

Const MAX_PATH = 260
Const TH32CS_SNAPPROCESS = &H2

Private Type PROCESSENTRY32
    dwSize As Long
    cntUsage As Long
    th32ProcessID As Long
    th32DefaultHeapID As Long
    th32ModuleID As Long
    cntThreads As Long
    th32ParentProcessID As Long
    pcPriClassBase As Long
    dwFlags As Long
    szExeFile As String * MAX_PATH
End Type

Public Function GetBroodHwnd() As Long
    Dim procEntry As PROCESSENTRY32
    Dim hSnapShot As Long, lret As Long
    procEntry.dwSize = LenB(procEntry)
    
    hSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0&)
    If hSnapShot = -1 Then
        Exit Function
    End If
    
    lret = Process32First(hSnapShot, procEntry)
    If lret = 0 Then
        GoTo ExitLine
    Else
        If GetFileName(procEntry.szExeFile) = "WAR3.EXE" Then GetBroodHwnd = procEntry.th32ProcessID: GoTo ExitLine
    End If
    
    Do
        lret = Process32Next(hSnapShot, procEntry)
        If lret = 0 Then
            GoTo ExitLine
        Else
        If UCase(GetFileName(procEntry.szExeFile)) = "WAR3.EXE" Then GetBroodHwnd = procEntry.th32ProcessID: GoTo ExitLine
            DoEvents
        End If
    Loop
 
ExitLine:
    CloseHandle hSnapShot
End Function

Public Function getFullPath(ByVal hProcess As Long) As String
    Dim Ret As Long
    Dim hEXE As Long
    Dim cbNeeded As Long
    Dim ProcessID As Long
    
    ProcessID = hProcess
    hProcess = OpenProcess( _
        PROCESS_QUERY_INFORMATION Or PROCESS_VM_READ, _
        0&, _
        ProcessID)
    Ret = EnumProcessModules(hProcess, hEXE, 4&, cbNeeded)
    
    If hEXE <> 0 Then
       'getFullPath = String$(MAX_PATH, 0)
       'Ret = GetModuleBaseName(hProcess, hEXE, getFullPath, MAX_PATH)
       'getFullPath = Trim0(getFullPath)
       
       getFullPath = String$(MAX_PATH, 0)
       Ret = GetModuleFileNameEx(hProcess, hEXE, getFullPath, MAX_PATH)
       getFullPath = Trim0(getFullPath)
    End If
    CloseHandle (hProcess)
End Function

Private Function Trim0(sName As String) As String
    Dim x As Integer
    x = InStr(sName, Chr$(0))
    If x > 0 Then Trim0 = Left$(sName, x - 1) Else Trim0 = sName
End Function

Private Function GetFileName(ByVal sPath As String) As String
    Dim x As Integer
    sPath = Trim0(sPath)
    x = InStrRev(sPath, "\")
    GetFileName = sPath
    If x > 0 Then
        GetFileName = Mid$(sPath, x + 1)
    End If
End Function
