Attribute VB_Name = "modWar3Common"
Option Explicit

'public Declare Function GetTickCount Lib "kernel32" () As Long

Public BroodHwnd As Long    ' 游戏句柄

Public Type t_MemSelectedUnitList
    nextNode As Long
    nextNodeNot As Long ' = Not nextNode
    heroESI As Long     ' 正是我们需要的东西
End Type

Public Type t_MemManage
    Caption As String       ' 这个修改了什么
    DataType As VbVarType   ' 3-Long 4-Single 17-Char*4
    Address4 As Long        ' 目标地址
    OK As Boolean           ' 是否允许修改
End Type

Public pGameMemory_ThisGame As Long         ' 游戏全局      [6FAA4178]
Public pGameMemory_ThisGameMemory As Long   ' 游戏全局内存  [ThisGame + 0xC]

Public pUnit_ThisUnit As Long               ' 当前单位      ESI
Public pUnit_UnitAttributes As Long         '               [ThisUnit + 1E4]
Public pUnit_HeroAttributes As Long         '               [ThisUnit + 1EC]

Public WAR3_ADDRESS_THIS_GAME As Long
Public WAR3_ADDRESS_SELECTED_UNIT_LIST As Long
Public WAR3_FUNCTION_MOVESPEED As Long

Public Sub initGameMemory()
    pGameMemory_ThisGame = OperMem.ReadMemLong(BroodHwnd, WAR3_ADDRESS_THIS_GAME)
    If pGameMemory_ThisGame <> 0 Then
        pGameMemory_ThisGameMemory = OperMem.ReadMemLong(BroodHwnd, pGameMemory_ThisGame + &HC)
    Else
        pGameMemory_ThisGameMemory = 0
        pUnit_ThisUnit = 0
        pUnit_UnitAttributes = 0
        pUnit_HeroAttributes = 0
    End If
End Sub

' 所谓的<内存提取算法0>
Public Function readFromGameMemory(ByVal nIndex As Long) As Long
    If nIndex < 0 Then
        MsgBox "BUG in <readFromGameMemory>：nIndex最高位为1。"
        readFromGameMemory = 0
        Exit Function
    End If
    If pGameMemory_ThisGameMemory <> 0 Then
        readFromGameMemory = OperMem.ReadMemLong(BroodHwnd, pGameMemory_ThisGameMemory + nIndex * 8 + 4)
    Else
        readFromGameMemory = 0
    End If
End Function

' 所谓的<内存提取算法1>，用于智力
Public Function getGameVar1(ByVal nIndex As Long) As Long
    getGameVar1 = readFromGameMemory(nIndex)
    getGameVar1 = getGameVar1 + &H78
End Function

' 所谓的<内存提取算法2>，用于移动速度
Public Function getGameVar2(ByVal nIndex As Long)
    getGameVar2 = readFromGameMemory(nIndex)
    If OperMem.ReadMemLong(BroodHwnd, getGameVar2 + &H20) = 0 Then
        getGameVar2 = OperMem.ReadMemLong(BroodHwnd, getGameVar2 + &H54)
    Else
        MsgBox "BUG in <内存提取算法2>：sub_6F468A20()所获地址 + 20所指区域不为零。"
        ' 如果真的发生了，重新抄袭sub_6F468A20
        ' 断点下在6F0776F6
        getGameVar2 = 0
    End If
End Function

Public Function getWar3Version() As String
    Dim Version As String
    Dim subVersion As String
    
    modVersion.iFileName = getFullPath(BroodHwnd)
    modVersion.Update
    
    Version = modVersion.iProductVersion
    getWar3Version = "游戏版本：" & Version & vbCrLf & "评价："
        
    Select Case Version
    Case "1.20.4.6074"
        subVersion = "1.20e"
        getWar3Version = getWar3Version & "该版本被完全支持"
    Case "1.21.0.6263"
        subVersion = "1.21"
        getWar3Version = getWar3Version & "该版本被完全支持"
    Case "1.22.0.6328"
        subVersion = "1.22"
        getWar3Version = getWar3Version & "该版本被完全支持"
    Case Else
        subVersion = Left(Version, 4)
        Select Case subVersion
        Case "1.20"
            getWar3Version = getWar3Version & "目前按照1.20.4.6074兼容运行，部分功能可能无法使用"
            subVersion = "1.20e"
        Case "1.21"
            getWar3Version = getWar3Version & "目前按照1.20.4.6074兼容运行，部分功能可能无法使用"
        Case "1.22"
            getWar3Version = getWar3Version & "目前按照1.22.0.6328兼容运行，部分功能可能无法使用"
        Case Else
            getWar3Version = getWar3Version & "不可识别的版本。目前按照1.20.4.6074运行，功能可能无法使用"
            subVersion = "1.20e"
        End Select
    End Select
    
    Select Case subVersion
    Case "1.20e"
        WAR3_ADDRESS_THIS_GAME = &H6F87C744
        WAR3_ADDRESS_SELECTED_UNIT_LIST = &H6F8722BC
        WAR3_FUNCTION_MOVESPEED = &H6F55BDF0
    Case "1.21"
        WAR3_ADDRESS_THIS_GAME = &H6F87D7BC
        WAR3_ADDRESS_SELECTED_UNIT_LIST = &H6F873334
        WAR3_FUNCTION_MOVESPEED = &H6F55FE80
    Case "1.22"
        WAR3_ADDRESS_THIS_GAME = &H6FAA4178
        WAR3_ADDRESS_SELECTED_UNIT_LIST = &H6FAA2FFC
        WAR3_FUNCTION_MOVESPEED = &H6F201190
    End Select
End Function
