Attribute VB_Name = "modModifyUnit"
Option Explicit

Public Const sMemoryMin As Long = 1
Public Const sMemoryMax As Long = 16
Public sMemory(1 To 1) As t_MemInject
Public sMemoryState(1 To 16) As t_MemManage

Public Sub dataInit(dataGrid As MSHFlexGrid)
    On Error Resume Next
 
    With sMemory(1)
        .Address1 = Val("&H" & LoadResString(1))
        .Address2 = Val("&H" & LoadResString(2))
        .Address3 = Val("&H" & LoadResString(3))
        .Mem0 = LoadResData(101, "CUSTOM")
        .Mem1 = LoadResData(102, "CUSTOM")
        .Mem2 = LoadResData(103, "CUSTOM")
    End With
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
    With sMemory(1)
        EditMem BroodHwnd, .Address2, .Mem2
        EditMem BroodHwnd, .Address1, .Mem1
        WriteMemLong BroodHwnd, .Address3, 0
    End With
    
    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
        sMemoryState(i).OK = False
        sMemoryState(i).Address4 = 0
    Next
End Sub

' 检测注入是否结束
Public Function detectInject() As Boolean
    Dim ret As Long
    With sMemory(1)
        ret = ReadMemLong(BroodHwnd, .Address3)
        If ret = 0 Then
            detectInject = False
        Else
            detectInject = True
            pUnit_ThisUnit = ret
            unitCaleAllAdress
        End If
    End With
End Function

' 解除修改的代码
Sub AntiRefresh()
    With sMemory(1)
        EditMem BroodHwnd, .Address1, .Mem0
    End With
End Sub

' 填充全部Address4
Sub unitCaleAllAdress()
    ' 基本的地址
    pUnit_UnitAttributes = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H1E4)
    pUnit_HeroAttributes = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H1EC)
    modWar3Common.initGameMemory
    
    Dim tmpAddress As Long
    ' 2 - HPMax
    tmpAddress = pUnit_ThisUnit + &H98 + &H8
    tmpAddress = ReadMemLong(BroodHwnd, tmpAddress)
    sMemoryState(2).Address4 = modWar3Common.readFromGameMemory(tmpAddress) + &H84
    sMemoryState(2).OK = True
    
    ' 1 - HP
    sMemoryState(1).Address4 = sMemoryState(2).Address4 - &HC
    sMemoryState(1).OK = True
        
    ' 4 - MPMax
    tmpAddress = pUnit_ThisUnit + &H98 + &H28
    tmpAddress = ReadMemLong(BroodHwnd, tmpAddress)
    sMemoryState(4).Address4 = modWar3Common.readFromGameMemory(tmpAddress) + &H84
    sMemoryState(4).OK = True
    
    ' 3 - MP
    sMemoryState(3).Address4 = sMemoryState(4).Address4 - &HC
    sMemoryState(3).OK = True
    
    ' 5 - Exp
    sMemoryState(5).Address4 = pUnit_HeroAttributes + &H8C
    sMemoryState(5).OK = True
    
    ' 6 - Power
    sMemoryState(6).Address4 = pUnit_HeroAttributes + &H94
    sMemoryState(6).OK = True
    
    ' 7 - Agility
    sMemoryState(7).Address4 = pUnit_HeroAttributes + &HA8
    sMemoryState(7).OK = True
    
    ' 9 - ROF
    sMemoryState(9).Address4 = pUnit_UnitAttributes + &H1B0
    sMemoryState(9).OK = True
    
    ' 10 - Attack-Base
    sMemoryState(10).Address4 = pUnit_UnitAttributes + &HA0
    sMemoryState(10).OK = True
    
    ' 11 - Attack-Base
    sMemoryState(11).Address4 = pUnit_UnitAttributes + &H94
    sMemoryState(11).OK = True
    
    ' 12 - Attack-Base
    sMemoryState(12).Address4 = pUnit_UnitAttributes + &H88
    sMemoryState(12).OK = True
    
    ' 13 - Attack-Base
    sMemoryState(13).Address4 = pUnit_UnitAttributes + &HF4
    sMemoryState(13).OK = True
    
    ' 15 - Armour
    sMemoryState(15).Address4 = pUnit_ThisUnit + &HE0
    sMemoryState(15).OK = True
        
    ' 16 - Armour Type
    sMemoryState(16).Address4 = pUnit_ThisUnit + &HE4
    sMemoryState(16).OK = True
    
    ' 8 - Intellect
    tmpAddress = ReadMemLong(BroodHwnd, pUnit_HeroAttributes + &H7C + 2 * 4)
    sMemoryState(8).Address4 = modWar3Common.getGameVar1(tmpAddress)
    sMemoryState(8).OK = True
    
    ' 14 - Move Speed
    Dim tmp1 As Long, tmp2 As Long, tmp3 As Long
    tmpAddress = ReadMemLong(BroodHwnd, pUnit_ThisUnit + &H1D8)
    tmpAddress = modWar3Common.getGameVar2(tmpAddress)
    tmp1 = ReadMemLong(BroodHwnd, tmpAddress)
    tmp2 = ReadMemLong(BroodHwnd, tmpAddress + &H24)
    tmp3 = ReadMemLong(BroodHwnd, tmpAddress + &H28)
    Do While True
        If ReadMemLong(BroodHwnd, tmp1 + &H2D4) = &H6F201190 Then
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
    
    ' 结束
End Sub

Public Sub getUnitValues(dataGrid As MSHFlexGrid)
    If pUnit_ThisUnit = 0 Or pGameMemory_ThisGame = 0 Then Exit Sub
    
    Dim vRetLng As Long, vRetFloat As Single
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
            End If
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
    Dim vInt As Long, vFloat As Single
    
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
                End If
                dataGrid.TextMatrix(i, 2) = ""
            End If
        End If
    End With
    Next
End Sub

