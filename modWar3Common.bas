Attribute VB_Name = "modWar3Common"
Option Explicit

'public Declare Function GetTickCount Lib "kernel32" () As Long

Public BroodHwnd As Long    ' 游戏句柄

Public Type t_MemInject
    Address1 As Long        ' 源-地址
    Mem0() As Byte          ' 源-内容
    Mem1() As Byte          ' 跳转-内容
    
    Address2 As Long        ' 转移-地址
    Mem2() As Byte          ' 转移-内容
    
    Address3 As Long        ' 寄存器备份-地址
End Type

Public Type t_MemManage
    Caption As String       ' 这个修改了什么
    DataType As VbVarType   ' 3-Long 4-Single
    Address4 As Long        ' 目标地址
    OK As Boolean           ' 是否允许修改
End Type

Public pGameMemory_ThisGame As Long         ' 游戏全局      [6FAA4178]
Public pGameMemory_ThisGameMemory As Long   ' 游戏全局内存  [ThisGame + 0xC]

Public pUnit_ThisUnit As Long               ' 当前单位      ESI
Public pUnit_UnitAttributes As Long         '               [ThisUnit + 1E4]
Public pUnit_HeroAttributes As Long         '               [ThisUnit + 1EC]

Public Const WAR3_ADDRESS_THIS_GAME As Long = &H6FAA4178

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

