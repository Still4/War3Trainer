Attribute VB_Name = "modModifyUnit"
Option Explicit

Public Const sMemoryMin As Long = 1
Public Const sMemoryMax As Long = 34
Public sMemoryState(sMemoryMin To sMemoryMax) As t_MemManage
Public sMemorySelectedUnitList() As t_MemSelectedUnitList
Public nSelectedUnitList As Long

Public Sub dataInit(dataGrid As MSHFlexGrid)
    On Error Resume Next
     
    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
        With sMemoryState(i)
            .Caption = LoadResString(i * 2 + 100 - 1)
            dataGrid.AddItem .Caption, i
            .DataType = LoadResString(i * 2 + 100)
            .Address4 = 0
            .OK = False
        End With
    Next
End Sub

Public Sub goInject()
    ' 获得当前选中的单位的地址
    nSelectedUnitList = 0
    Erase sMemorySelectedUnitList
    
    Dim listHead As Long, listEnd As Long, listLength As Long
    Dim tmpAddress As Long
    tmpAddress = ReadMemLong(BroodHwnd, WAR3_ADDRESS_SELECTED_UNIT_LIST)
    Dim a2 As Long
    a2 = ReadMemInteger(BroodHwnd, tmpAddress + &H28)
    tmpAddress = ReadMemLong(BroodHwnd, tmpAddress + &H58 + 4 * a2) ' 这个a2比较玄乎
    tmpAddress = ReadMemLong(BroodHwnd, tmpAddress + &H34)
    
    listHead = ReadMemLong(BroodHwnd, tmpAddress + &H1F0)
    'listEnd = ReadMemLong(BroodHwnd, tmpAddress + &H1F4)
    listLength = ReadMemLong(BroodHwnd, tmpAddress + &H1F8)
    
    While listLength > 0
        nSelectedUnitList = nSelectedUnitList + 1
        ReDim Preserve sMemorySelectedUnitList(1 To nSelectedUnitList)
        With sMemorySelectedUnitList(nSelectedUnitList)
            .nextNode = ReadMemLong(BroodHwnd, listHead)
            '.nextNodeNot = ReadMemLong(BroodHwnd, listHead+4)
            .heroESI = ReadMemLong(BroodHwnd, listHead + 8)
            
            listHead = .nextNode
            listLength = listLength - 1
        End With
    Wend
End Sub

Function getUnitName(ByVal ESI As Long)
    getUnitName = ReadMemChar4(BroodHwnd, ESI + &H30)   ' 真是根据hyp的文章推算出来的，未找源代码
End Function

' 填充全部Address4
Sub unitCaleAllAdress()
    ' 基本的地址
    pUnit_UnitAttributes = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H1E4)
    pUnit_HeroAttributes = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H1EC)
    modWar3Common.initGameMemory
    
    Dim tmpAddress As Long
    ' 2 - HPMax
    If pUnit_ThisUnit > 0 Then
        tmpAddress = pUnit_ThisUnit + &H98 + &H8
        tmpAddress = ReadMemLong(BroodHwnd, tmpAddress)
        sMemoryState(2).Address4 = modWar3Common.readFromGameMemory(tmpAddress) + &H84
        sMemoryState(2).OK = True
    Else
        sMemoryState(2).OK = False
    End If
    
    ' 1 - HP
    If sMemoryState(2).OK Then
        sMemoryState(1).Address4 = sMemoryState(2).Address4 - &HC
        sMemoryState(1).OK = True
    Else
        sMemoryState(1).OK = False
    End If
        
    ' 4 - MPMax
    If pUnit_ThisUnit > 0 Then
        tmpAddress = pUnit_ThisUnit + &H98 + &H28
        tmpAddress = ReadMemLong(BroodHwnd, tmpAddress)
        sMemoryState(4).Address4 = modWar3Common.readFromGameMemory(tmpAddress) + &H84
        sMemoryState(4).OK = True
    Else
        sMemoryState(4).OK = False
    End If
        
    ' 3 - MP
    If sMemoryState(4).OK Then
        sMemoryState(3).Address4 = sMemoryState(4).Address4 - &HC
        sMemoryState(3).OK = True
    Else
        sMemoryState(3).OK = False
    End If
    
    ' 5 - Exp
    If pUnit_HeroAttributes > 0 Then
        sMemoryState(5).Address4 = pUnit_HeroAttributes + &H8C
        sMemoryState(5).OK = True
    Else
        sMemoryState(5).OK = False
    End If
    
    ' 6 - Power
    If pUnit_HeroAttributes > 0 Then
        sMemoryState(6).Address4 = pUnit_HeroAttributes + &H94
        sMemoryState(6).OK = True
    Else
        sMemoryState(6).OK = False
    End If
        
    ' 7 - Agility
    If pUnit_HeroAttributes > 0 Then
        sMemoryState(7).Address4 = pUnit_HeroAttributes + &HA8
        sMemoryState(7).OK = True
    Else
        sMemoryState(7).OK = False
    End If
        
    ' 9 - ROF
    If pUnit_UnitAttributes > 0 Then
        sMemoryState(9).Address4 = pUnit_UnitAttributes + &H1B0
        sMemoryState(9).OK = True
    Else
        sMemoryState(9).OK = False
    End If
    
    ' 10 - Attack-Base
    If pUnit_UnitAttributes > 0 Then
        sMemoryState(10).Address4 = pUnit_UnitAttributes + &HA0
        sMemoryState(10).OK = True
    Else
        sMemoryState(10).OK = False
    End If
    
    ' 11 - Attack-Base
    If pUnit_UnitAttributes > 0 Then
        sMemoryState(11).Address4 = pUnit_UnitAttributes + &H94
        sMemoryState(11).OK = True
    Else
        sMemoryState(11).OK = False
    End If
    
    ' 12 - Attack-Base
    If pUnit_UnitAttributes > 0 Then
        sMemoryState(12).Address4 = pUnit_UnitAttributes + &H88
        sMemoryState(12).OK = True
    Else
        sMemoryState(12).OK = False
    End If
    
    ' 13 - Attack-Base
    If pUnit_UnitAttributes > 0 Then
        sMemoryState(13).Address4 = pUnit_UnitAttributes + &HF4
        sMemoryState(13).OK = True
    Else
        sMemoryState(13).OK = False
    End If
    
    ' 15 - Armour
    If pUnit_ThisUnit > 0 Then
        sMemoryState(15).Address4 = pUnit_ThisUnit + &HE0
        sMemoryState(15).OK = True
    Else
        sMemoryState(15).OK = False
    End If
    
    ' 16 - Armour Type
    If pUnit_ThisUnit > 0 Then
        sMemoryState(16).Address4 = pUnit_ThisUnit + &HE4
        sMemoryState(16).OK = True
    Else
        sMemoryState(16).OK = False
    End If
    
    ' 8 - Intellect
    If pUnit_HeroAttributes > 0 Then
        tmpAddress = ReadMemLong(BroodHwnd, pUnit_HeroAttributes + &H7C + 2 * 4)
        sMemoryState(8).Address4 = modWar3Common.getGameVar1(tmpAddress)
        sMemoryState(8).OK = True
    Else
        sMemoryState(8).OK = False
    End If
    
    ' 14 - Move Speed
    If pUnit_ThisUnit > 0 Then
        Dim tmp1 As Long, tmp2 As Long, tmp3 As Long
        tmpAddress = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H1D8)
        tmpAddress = modWar3Common.getGameVar2(tmpAddress)
        tmp1 = ReadMemLong(BroodHwnd, tmpAddress)
        tmp2 = ReadMemLong(BroodHwnd, tmpAddress + &H24)
        tmp3 = ReadMemLong(BroodHwnd, tmpAddress + &H28)
        Do While True
            If ReadMemLong(BroodHwnd, tmp1 + &H2D4) = WAR3_FUNCTION_MOVESPEED Then
                sMemoryState(14).Address4 = tmpAddress + &H78
                sMemoryState(14).OK = True
                Exit Do
            End If
            If tmp2 = -1 Or tmp3 = -1 Then Exit Do
            tmpAddress = ReadMemLong(BroodHwnd, tmpAddress + &H24)
            tmpAddress = modWar3Common.getGameVar2(tmpAddress)
            tmp1 = ReadMemLong(BroodHwnd, tmpAddress)
            tmp2 = ReadMemLong(BroodHwnd, tmpAddress + &H24)
            tmp3 = ReadMemLong(BroodHwnd, tmpAddress + &H28)
        Loop
    Else
        sMemoryState(14).OK = False
    End If
    
    ' 17 - Coordinate X
    If pUnit_ThisUnit > 0 Then
        tmpAddress = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H164 + 8)
        sMemoryState(17).Address4 = modWar3Common.readFromGameMemory(tmpAddress) + &H78
        sMemoryState(17).OK = True
    Else
        sMemoryState(17).OK = False
    End If
    
    ' 18 - Coordinate Y
    If sMemoryState(17).OK Then
        sMemoryState(18).Address4 = sMemoryState(17).Address4 + 4
        sMemoryState(18).OK = True
    Else
        sMemoryState(18).OK = False
    End If
    
    ' 19 - Capability Name, Level, Hero Level
    ' hyp提供，未找源代码
    If pUnit_HeroAttributes > 0 Then
        sMemoryState(19).Address4 = pUnit_HeroAttributes + &HF4
        sMemoryState(20).Address4 = pUnit_HeroAttributes + &HF8
        sMemoryState(21).Address4 = pUnit_HeroAttributes + &HFC
        sMemoryState(22).Address4 = pUnit_HeroAttributes + &H100
        sMemoryState(23).Address4 = pUnit_HeroAttributes + &H104
        
        sMemoryState(24).Address4 = pUnit_HeroAttributes + &H10C
        sMemoryState(25).Address4 = pUnit_HeroAttributes + &H110
        sMemoryState(26).Address4 = pUnit_HeroAttributes + &H114
        sMemoryState(27).Address4 = pUnit_HeroAttributes + &H118
        sMemoryState(28).Address4 = pUnit_HeroAttributes + &H11C
        
        sMemoryState(29).Address4 = pUnit_HeroAttributes + &H124
        sMemoryState(30).Address4 = pUnit_HeroAttributes + &H128
        sMemoryState(31).Address4 = pUnit_HeroAttributes + &H12C
        sMemoryState(32).Address4 = pUnit_HeroAttributes + &H130
        sMemoryState(33).Address4 = pUnit_HeroAttributes + &H134
        
        sMemoryState(19).OK = True
        sMemoryState(20).OK = True
        sMemoryState(21).OK = True
        sMemoryState(22).OK = True
        sMemoryState(23).OK = True
        
        sMemoryState(24).OK = True
        sMemoryState(25).OK = True
        sMemoryState(26).OK = True
        sMemoryState(27).OK = True
        sMemoryState(28).OK = True
        
        sMemoryState(29).OK = True
        sMemoryState(30).OK = True
        sMemoryState(31).OK = True
        sMemoryState(32).OK = True
        sMemoryState(33).OK = True
    Else
        sMemoryState(19).OK = False
        sMemoryState(20).OK = False
        sMemoryState(21).OK = False
        sMemoryState(22).OK = False
        sMemoryState(23).OK = False
        
        sMemoryState(24).OK = False
        sMemoryState(25).OK = False
        sMemoryState(26).OK = False
        sMemoryState(27).OK = False
        sMemoryState(28).OK = False
        
        sMemoryState(29).OK = False
        sMemoryState(30).OK = False
        sMemoryState(31).OK = False
        sMemoryState(32).OK = False
        sMemoryState(33).OK = False
    End If
    
    ' 31 - Capability Point
    ' hyp提供，未找源代码
    If pUnit_HeroAttributes > 0 Then
        sMemoryState(34).Address4 = pUnit_HeroAttributes + &H90
        sMemoryState(34).OK = True
    Else
        sMemoryState(34).OK = False
    End If
    ' 结束
End Sub

Public Sub getUnitValues(dataGrid As MSHFlexGrid)
    If pUnit_ThisUnit = 0 Or pGameMemory_ThisGame = 0 Then Exit Sub
    
    Dim vRetLng As Long, vRetFloat As Single, vChar4 As String
    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
    With sMemoryState(i)
        If .OK Then
            If .DataType = vbLong Then
                vRetLng = ReadMemLong(BroodHwnd, .Address4)
                dataGrid.TextMatrix(i, 1) = Trim(Str(vRetLng))
            ElseIf .DataType = vbSingle Then
                vRetFloat = ReadMemFloat(BroodHwnd, .Address4)
                dataGrid.TextMatrix(i, 1) = Trim(Str(vRetFloat))
            ElseIf .DataType = vbByte Then
                vChar4 = ReadMemChar4(BroodHwnd, .Address4)
                dataGrid.TextMatrix(i, 1) = vChar4
            End If
        Else
            dataGrid.TextMatrix(i, 1) = "?"
        End If
    End With
    Next
End Sub

Public Sub editData(dataGrid As MSHFlexGrid)
    Dim vVal As Long
    Dim vStr As String

    Dim i As Integer
    For i = sMoneyMemoryMin To sMoneyMemoryMax
    With sMoneyMemoryState(i)
        If .OK Then
            vStr = dataGrid.TextMatrix(i, 2)
            If Trim(vStr) <> "" Then
                vVal = Val(vStr)
                dataGrid.TextMatrix(i, 1) = Trim(Str(vVal))
                dataGrid.TextMatrix(i, 2) = ""
                If .DataType = 1 Then vVal = vVal * 10
                OperMem.WriteMemLong BroodHwnd, .Address4, vVal
            End If
        End If
    End With
    Next
End Sub

Public Sub putUnitValues(dataGrid As MSHFlexGrid)
    Dim vStr As String
    Dim vInt As Long, vFloat As Single, vChar4 As String
    
    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
    With sMemoryState(i)
        If .OK Then
            vStr = dataGrid.TextMatrix(i, 2)
            If Trim(vStr) <> "" Then
                If .DataType = vbLong Then
                    vInt = Val(vStr)
                    OperMem.WriteMemLong BroodHwnd, .Address4, vInt
                    dataGrid.TextMatrix(i, 1) = Trim(Str(vInt))
                ElseIf .DataType = vbSingle Then
                    vFloat = Val(vStr)
                    OperMem.WriteMemFloat BroodHwnd, .Address4, vFloat
                    dataGrid.TextMatrix(i, 1) = Trim(Str(vFloat))
                ElseIf .DataType = vbByte Then
                    vChar4 = Trim(vStr)
                    vChar4 = Left(vChar4, 4)
                    If Len(vChar4) < 4 Then vChar4 = vChar4 & Space(4 - Len(vChar4))
                    OperMem.WriteMemChar4 BroodHwnd, .Address4, vChar4
                    dataGrid.TextMatrix(i, 1) = vChar4
                End If
                dataGrid.TextMatrix(i, 2) = ""
            End If
        End If
    End With
    Next
End Sub

