Attribute VB_Name = "OperProc"
Option Explicit

Private Declare Function CreateToolhelp32Snapshot Lib "kernel32" (ByVal dwFlags As Long, ByVal th32ProcessID As Long) As Long
Private Declare Function Process32First Lib "kernel32" (ByVal hSnapShot As Long, lppe As PROCESSENTRY32) As Long
Private Declare Function Process32Next Lib "kernel32" (ByVal hSnapShot As Long, lppe As PROCESSENTRY32) As Long
Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long

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

Private Function Trim0(sName As String) As String
    Dim X As Integer
    X = InStr(sName, Chr$(0))
    If X > 0 Then Trim0 = Left$(sName, X - 1) Else Trim0 = sName
End Function

Private Function GetFileName(ByVal sPath As String) As String
    Dim X As Integer
    sPath = Trim0(sPath)
    X = InStrRev(sPath, "\")
    GetFileName = sPath
    If X > 0 Then
        GetFileName = Mid$(sPath, X + 1)
    End If
End Function
