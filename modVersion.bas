Attribute VB_Name = "modVersion"
Option Explicit

Public iFileName          As String

Public iFileVersion       As String
Public iProductVersion    As String
Public iFlags             As String
Public iOS                As String
Public iFileType          As String
Public iSubType           As String

' -------------------------
' File version declarations

Private Type VS_VERSION
   dwSignature As Long
   dwStrucVersion As Long         '  e.g. 0x00000042 = "0.42"
   dwFileVersionMS As Long        '  e.g. 0x00030075 = "3.75"
   dwFileVersionLS As Long        '  e.g. 0x00000031 = "0.31"
   dwProductVersionMS As Long     '  e.g. 0x00030010 = "3.10"
   dwProductVersionLS As Long     '  e.g. 0x00000031 = "0.31"
   dwFileFlagsMask As Long        '  = 0x3F for version "0.42"
   dwFileFlags As Long            '  e.g. VFF_DEBUG Or VFF_PRERELEASE
   dwFileOS As Long               '  e.g. VOS_DOS_WINDOWS16
   dwFileType As Long             '  e.g. VFT_DRIVER
   dwFileSubtype As Long          '  e.g. VFT2_DRV_KEYBOARD
   dwFileDateMS As Long           '  e.g. 0
   dwFileDateLS As Long           '  e.g. 0
End Type

#If Win32 Then
   Private Declare Function GetFileVersionInfoSize Lib "version.dll" Alias "GetFileVersionInfoSizeA" (ByVal lptstrFilename As String, lpdwHandle As Long) As Long
   Private Declare Function GetFileVersionInfo Lib "version.dll" Alias "GetFileVersionInfoA" (ByVal lptstrFilename As String, ByVal dwHandle As Long, ByVal dwLen As Long, lpData As Byte) As Long
   Private Declare Function VerLanguageName Lib "version.dll" Alias "VerLanguageNameA" (ByVal wLang As Long, ByVal szLang As String, ByVal nSize As Long) As Long
   Private Declare Function VerQueryValue Lib "version.dll" Alias "VerQueryValueA" (pBlock As Byte, ByVal lpSubBlock As String, lplpBuffer As Long, puLen As Long) As Long
      
   Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" ( _
      stvDest As Any, _
      stvSource As Any, _
      ByVal cbCopy As Long)
#Else
   Private Declare Function GetFileVersionInfo% Lib "ver.dll" (ByVal lpszFileName$, ByVal Handle&, ByVal cbBuf&, lpvData As Byte)
   Private Declare Function GetFileVersionInfoSize% Lib "ver.dll" (ByVal lpszFileName$, lpdwHandle&)
   Private Declare Function VerLanguageName% Lib "ver.dll" (ByVal Lang%, ByVal lpszLang$, ByVal cbLang%)
   Private Declare Function VerQueryValue% Lib "ver.dll" (lpvBlock As Byte, ByVal SubBlock$, lpBuffer&, lpcb%)
   
   Private Declare Sub CopyMemory Lib "Kernel" Alias "hmemcpy" ( _
      stvDest As Any, _
      stvSource As Any, _
      ByVal cbCopy As Long)
#End If

' **** VS_VERSION.dwFileFlags ****
Private Const VS_FF_DEBUG = &H1
Private Const VS_FF_PRERELEASE = &H2
Private Const VS_FF_PATCHED = &H4
Private Const VS_FF_PRIVATEBUILD = &H8
Private Const VS_FF_INFOINFERRED = &H10
Private Const VS_FF_SPECIALBUILD = &H20

' **** VS_VERSION.dwFileOS ****
Private Const VOS_UNKNOWN = &H0
Private Const VOS_DOS = &H10000
Private Const VOS_OS216 = &H20000
Private Const VOS_OS232 = &H30000
Private Const VOS_NT = &H40000


Private Const VOS__BASE = &H0
Private Const VOS__WINDOWS16 = &H1
Private Const VOS__PM16 = &H2
Private Const VOS__PM32 = &H3
Private Const VOS__WINDOWS32 = &H4

Private Const VOS_DOS_WINDOWS16 = &H10001
Private Const VOS_DOS_WINDOWS32 = &H10004
Private Const VOS_OS216_PM16 = &H20002
Private Const VOS_OS232_PM32 = &H30003
Private Const VOS_NT_WINDOWS32 = &H40004

' **** VS_VERSION.dwFileType ****
Private Const VFT_UNKNOWN = &H0
Private Const VFT_APP = &H1
Private Const VFT_DLL = &H2
Private Const VFT_DRV = &H3
Private Const VFT_FONT = &H4
Private Const VFT_VXD = &H5
Private Const VFT_STATIC_LIB = &H7

' **** VS_VERSION.dwFileSubtype for VFT_WINDOWS_DRV ****
Private Const VFT2_UNKNOWN = &H0
Private Const VFT2_DRV_PRINTER = &H1
Private Const VFT2_DRV_KEYBOARD = &H2
Private Const VFT2_DRV_LANGUAGE = &H3
Private Const VFT2_DRV_DISPLAY = &H4
Private Const VFT2_DRV_MOUSE = &H5
Private Const VFT2_DRV_NETWORK = &H6
Private Const VFT2_DRV_SYSTEM = &H7
Private Const VFT2_DRV_INSTALLABLE = &H8
Private Const VFT2_DRV_SOUND = &H9
Private Const VFT2_DRV_COMM = &HA

' **** VS_VERSION.dwFileSubtype for VFT_WINDOWS_FONT ****
Private Const VFT2_FONT_RASTER = &H1
Private Const VFT2_FONT_VECTOR = &H2
Private Const VFT2_FONT_TRUETYPE = &H3

Public Sub Update()
    Dim x          As VS_VERSION
    Dim BufSize    As Long
    Dim r          As Long
    Dim dwHandle   As Long
    
    Dim InfoAddr   As Long
    Dim InfoLen    As Long
    
    Dim lpvData()  As Byte

   ' Clear the values
   iFileVersion = ""
   iProductVersion = ""
   iFlags = ""
   iOS = ""
   iFileType = ""
   
   ' Get Version Info
   BufSize = GetFileVersionInfoSize(iFileName, dwHandle)
   
   If BufSize = 0 Then
      Exit Sub
   End If
   
   ReDim lpvData(BufSize + 1)
   r = GetFileVersionInfo(iFileName, dwHandle, BufSize, lpvData(0))
   r = VerQueryValue(lpvData(0), "\", InfoAddr, InfoLen)
   
   If r = 0 Then
      Exit Sub
   End If
   
   CopyMemory x, ByVal InfoAddr, InfoLen

   ' Determine File Version number
   iFileVersion = LTrim(Str(HiWord(x.dwFileVersionMS))) + "."
   iFileVersion = iFileVersion + LTrim(Str(LoWord(x.dwFileVersionMS))) + "."
   iFileVersion = iFileVersion + LTrim(Str(HiWord(x.dwFileVersionLS))) + "."
   iFileVersion = iFileVersion + LTrim(Str(LoWord(x.dwFileVersionLS)))

   ' Determine Product Version number
   iProductVersion = LTrim(Str(HiWord(x.dwFileVersionMS))) + "."
   iProductVersion = iProductVersion + LTrim(Str(LoWord(x.dwProductVersionMS))) + "."
   iProductVersion = iProductVersion + LTrim(Str(HiWord(x.dwProductVersionLS))) + "."
   iProductVersion = iProductVersion + LTrim(Str(LoWord(x.dwProductVersionLS)))

   ' Determine Boolean attributes of File
   If x.dwFileFlags And VS_FF_DEBUG Then iFlags = "Debug"
   If x.dwFileFlags And VS_FF_PRERELEASE Then iFlags = iFlags + "Pre release"
   If x.dwFileFlags And VS_FF_PATCHED Then iFlags = iFlags + "Patched"
   If x.dwFileFlags And VS_FF_PRIVATEBUILD Then iFlags = iFlags + "Private build"

   If x.dwFileFlags And VS_FF_INFOINFERRED Then iFlags = iFlags + "Info"
   If x.dwFileFlags And VS_FF_DEBUG Then iFlags = iFlags + "Special"

   If x.dwFileFlags And &HFF00 Then iFlags = iFlags + "Unknown"

   ' Determine OS for which file was designed
   Select Case x.dwFileOS
      Case VOS_DOS_WINDOWS16
         iOS = "DOS-Win16"
      Case VOS_DOS_WINDOWS32
         iOS = "DOS-Win32"
      Case VOS_OS216_PM16
         iOS = "OS/2-16 PM-16"
      Case VOS_OS232_PM32
         iOS = "OS/2-32 PM-32"
      Case VOS_NT_WINDOWS32
         iOS = "NT-Win32"
      Case Else
         iOS = "Unknown"
   End Select

   ' Determine Type and SubType of File
   Select Case x.dwFileType
      Case VFT_APP
         iFileType = "App"
      Case VFT_DLL
         iFileType = "DLL"
      Case VFT_DRV
         iFileType = "Driver"
         Select Case x.dwFileSubtype
            Case VFT2_DRV_PRINTER
               iSubType = "Printer drv"
            Case VFT2_DRV_KEYBOARD
               iSubType = "Keyboard drv"
            Case VFT2_DRV_LANGUAGE
               iSubType = "Language drv"
            Case VFT2_DRV_DISPLAY
               iSubType = "Display drv"
            Case VFT2_DRV_MOUSE
               iSubType = "Mouse drv"
            Case VFT2_DRV_NETWORK
               iSubType = "Network drv"
            Case VFT2_DRV_SYSTEM
               iSubType = "System drv"
            Case VFT2_DRV_INSTALLABLE
               iSubType = "Installable"
            Case VFT2_DRV_SOUND
               iSubType = "Sound drv"
            Case VFT2_DRV_COMM
               iSubType = "Comm drv"
            Case VFT2_UNKNOWN
               iSubType = "Unknown"
         End Select
      Case VFT_FONT
         iFileType = "Font"
         Select Case x.dwFileSubtype
            Case VFT2_FONT_RASTER
               iSubType = "Raster Font"
            Case VFT2_FONT_VECTOR
               iSubType = "Vector Font"
            Case VFT2_FONT_TRUETYPE
               iSubType = "TrueType Font"
         End Select
      Case VFT_VXD
         iFileType = "VxD"
      Case VFT_STATIC_LIB
         iFileType = "Lib"
      Case Else
         iFileType = "Unknown"
   End Select

End Sub

Private Function LoWord(ByVal x As Long) As Integer
   LoWord = x And &HFFFF&
End Function

Private Function HiWord(ByVal x As Long) As Integer
   HiWord = x \ &HFFFF&
End Function






