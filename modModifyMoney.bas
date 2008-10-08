Attribute VB_Name = "modModifyMoney"
Option Explicit

' 警告：本文件的DataType定义是这样的：
' 0 - int
' 1 - int x10

Public Const sMoneyMemoryMin As Long = 1
Public Const sMoneyMemoryMax As Long = 40
Public sMoneyMemoryState(sMoneyMemoryMin To sMoneyMemoryMax) As t_MemManage

Public Sub dataInit(dataGrid As MSHFlexGrid)
    On Error Resume Next

    Dim i As Integer
    For i = sMoneyMemoryMin To sMoneyMemoryMax Step 4
        sMoneyMemoryState(i + 0).Caption = "P" & Trim(Str(i \ 4 + 1)) & " - 金"
        sMoneyMemoryState(i + 1).Caption = "P" & Trim(Str(i \ 4 + 1)) & " - 木"
        sMoneyMemoryState(i + 2).Caption = "P" & Trim(Str(i \ 4 + 1)) & " - 最大人口"
        sMoneyMemoryState(i + 3).Caption = "P" & Trim(Str(i \ 4 + 1)) & " - 当前人口"
        
        sMoneyMemoryState(i + 0).DataType = 1
        sMoneyMemoryState(i + 1).DataType = 1
        sMoneyMemoryState(i + 2).DataType = 0
        sMoneyMemoryState(i + 3).DataType = 0
        
        sMoneyMemoryState(i + 0).Address4 = 0
        sMoneyMemoryState(i + 1).Address4 = 0
        sMoneyMemoryState(i + 2).Address4 = 0
        sMoneyMemoryState(i + 3).Address4 = 0
        
        sMoneyMemoryState(i + 0).OK = False
        sMoneyMemoryState(i + 1).OK = False
        sMoneyMemoryState(i + 2).OK = False
        sMoneyMemoryState(i + 3).OK = False
        
        dataGrid.AddItem sMoneyMemoryState(i + 0).Caption, i
        dataGrid.AddItem sMoneyMemoryState(i + 1).Caption, i + 1
        dataGrid.AddItem sMoneyMemoryState(i + 2).Caption, i + 2
        dataGrid.AddItem sMoneyMemoryState(i + 3).Caption, i + 3
    Next
End Sub

Public Sub goInject()
    Dim i As Long, Upper As Long
    
    modWar3Common.initGameMemory
    Upper = modWar3Common.readFromGameMemory(1)
    Upper = Upper And &HFFFF0000
    
    sMoneyMemoryState(1 + 0 * 4).Address4 = Upper + &H190&
    sMoneyMemoryState(1 + 1 * 4).Address4 = Upper + &H1410&
    sMoneyMemoryState(1 + 2 * 4).Address4 = Upper + &H26A0&
    sMoneyMemoryState(1 + 3 * 4).Address4 = Upper + &H3920&
    sMoneyMemoryState(1 + 4 * 4).Address4 = Upper + &H4BB0&
    sMoneyMemoryState(1 + 5 * 4).Address4 = Upper + &H5E30&
    sMoneyMemoryState(1 + 6 * 4).Address4 = Upper + &H70C0&
    sMoneyMemoryState(1 + 7 * 4).Address4 = Upper + &H8350&
    sMoneyMemoryState(1 + 8 * 4).Address4 = Upper + &H95D0&
    sMoneyMemoryState(1 + 9 * 4).Address4 = Upper + &HA860&
    
    For i = sMoneyMemoryMin To sMoneyMemoryMax Step 4
        sMoneyMemoryState(i + 1).Address4 = sMoneyMemoryState(i).Address4 + &H80&
        sMoneyMemoryState(i + 2).Address4 = sMoneyMemoryState(i).Address4 + &H180&
        sMoneyMemoryState(i + 3).Address4 = sMoneyMemoryState(i).Address4 + &H200&
        
        sMoneyMemoryState(i + 0).OK = True
        sMoneyMemoryState(i + 1).OK = True
        sMoneyMemoryState(i + 2).OK = True
        sMoneyMemoryState(i + 3).OK = True
    Next
        
End Sub

Public Sub fillData(dataGrid As MSHFlexGrid)
    Dim vVal As Long
    Dim i As Integer
   
    For i = sMoneyMemoryMin To sMoneyMemoryMax
    With sMoneyMemoryState(i)
        If .OK Then
            vVal = ReadMemLong(BroodHwnd, .Address4)
            If .DataType = 1 Then vVal = vVal / 10
            
            dataGrid.TextMatrix(i, 1) = Trim(Str(vVal))
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

