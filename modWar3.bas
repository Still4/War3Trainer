Attribute VB_Name = "modWar3"
Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)
Private Declare Function GetTickCount Lib "kernel32" () As Long

Public BroodHwnd As Long

Public Type t_MemInfo
    Address1 As Long        ' 源地址
    Mem0() As Byte          ' 源代码
    Mem1() As Byte          ' 修改-调转
    Address2 As Long        ' 转移-地址
    Mem2() As Byte          ' 转移-代码
    Address3 As Long        ' 寄存器的备份
    Address3Patch As Long   ' 待修改的地址与寄存器的偏移量
    
    Index As Long           ' DataGrid的Index
    Name As String          ' 这个修改的内容
    DataType As VbVarType   ' 3-Long 4-Single
End Type

Public Type t_MemManage
    Address1 As Long
    ok As Boolean
End Type

Public sMemory(1 To 12) As t_MemInfo
Public sMemoryState(1 To 12) As t_MemManage
Public Const sMemoryMin As Long = 1
Public Const sMemoryMax As Long = 12

' Patch 2007.1.31 - 处理智力的特殊代码
' Begin
Public tickStart1 As Long
' End

Public Sub dataInit(dataGrid As MSHFlexGrid)
    On Error Resume Next

    Dim i As Integer, j As Integer
    For i = sMemoryMin To sMemoryMax
        j = j + 1
        With sMemory(i)
            'dataGrid.TextMatrix(j, 0) = LoadResString(Trim(Str(j)) & "00")
            dataGrid.AddItem LoadResString(Trim(Str(j)) & "00"), j
            .Address1 = Val("&H" & LoadResString(Trim(Str(j)) & "01"))
            .Address2 = Val("&H" & LoadResString(Trim(Str(j)) & "02"))
            .Address3 = Val("&H" & LoadResString(Trim(Str(j)) & "03"))
            .Address3Patch = Val("&H" & LoadResString(Trim(Str(j)) & "04"))
            .DataType = Val(LoadResString(Trim(Str(j)) & "05"))
            .Mem0 = LoadResData(j * 100 + 0, "CUSTOM")
            .Mem1 = LoadResData(j * 100 + 1, "CUSTOM")
            .Mem2 = LoadResData(j * 100 + 2, "CUSTOM")
            .Index = j
        End With
    Next
End Sub

Public Sub goInject()
    Dim i As Integer
    Dim var1(0 To 3) As Byte
    For i = sMemoryMin To sMemoryMax
        var1(0) = 0
        var1(1) = 0
        var1(2) = 0
        var1(3) = 0
        
        If sMemory(i).Address1 <> 0 Then
            EditMem BroodHwnd, sMemory(i).Address2, sMemory(i).Mem2
            EditMem BroodHwnd, sMemory(i).Address1, sMemory(i).Mem1
            EditMem BroodHwnd, sMemory(i).Address3, var1
        End If
        sMemoryState(i).ok = False
        sMemoryState(i).Address1 = 0
    Next
End Sub

Public Sub findHero(dataGrid As MSHFlexGrid)
    Dim vMem(0 To 3) As Byte
    Dim vVal As Long
    Dim vRetLng As Long, vRetFloat As Single
    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
        If sMemoryState(i).ok Then
            ' 恢复正常代码
            'EditMem BroodHwnd, sMemory(i).Address1, sMemory(i).Mem0
        Else
            ' 看写了数据吗？
            With sMemory(i)
                ReadMem BroodHwnd, .Address3, vMem
                CopyMemory ByVal VarPtr(vVal), vMem(0), 4
                If vMem(0) = 0 _
                And vMem(1) = 0 _
                And vMem(2) = 0 _
                And vMem(3) = 0 Then
                    ' 还没有搞定
                Else
                    ' 读取
                    ReadMem BroodHwnd, vVal + .Address3Patch, vMem
                    If .DataType = vbLong Then
                        CopyMemory ByVal VarPtr(vRetLng), vMem(0), 4
                        dataGrid.TextMatrix(.Index, 1) = Trim(Str(vRetLng))
                    ElseIf .DataType = vbSingle Then
                        CopyMemory ByVal VarPtr(vRetFloat), vMem(0), 4
                        dataGrid.TextMatrix(.Index, 1) = Trim(Str(vRetFloat))
                    End If
                    ' 记录、恢复正常代码
                    'sMemoryState(i).ok = True
                    'sMemoryState(i).Address1 = vVal
                    'EditMem BroodHwnd, sMemory(i).Address1, sMemory(i).Mem0
                    
                    ' Patch 2007.1.31 - 处理智力的特殊代码 ----------------
                    ' Begin
                    If i = 3 Then
                        ' “敏捷”刚刚找到
                        tickStart1 = GetTickCount
                    End If
                    If i = 4 Then  ' 当前是"智力"
                        If sMemoryState(3).ok And GetTickCount - tickStart1 > 3000 Then  ' “敏捷”已经找到&&已经超时3秒
                            ' 记录、恢复正常代码
                            sMemoryState(i).ok = True
                            sMemoryState(i).Address1 = vVal
                            EditMem BroodHwnd, sMemory(i).Address1, sMemory(i).Mem0
                        End If
                    Else           ' 普通情况
                        ' 记录、恢复正常代码
                        sMemoryState(i).ok = True
                        sMemoryState(i).Address1 = vVal
                        If sMemory(i).Address1 <> 0 Then
                            EditMem BroodHwnd, sMemory(i).Address1, sMemory(i).Mem0
                        End If
                    End If
                    ' Patch End -------------------------------------------
                End If
            End With
        End If
    Next
End Sub

Public Sub editHero(dataGrid As MSHFlexGrid)
    Dim vMem(0 To 3) As Byte
    Dim vVal As Long
    Dim vStr As String
    Dim vRetLng As Long, vRetFloat As Single

    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
    With sMemory(i)
        'ReadMem BroodHwnd, .Address3, vMem
        'CopyMemory ByVal VarPtr(vVal), vMem(0), 4
        vVal = sMemoryState(i).Address1
        
        vStr = dataGrid.TextMatrix(.Index, 2)
        If Trim(vStr) <> "" And vVal <> 0 Then
        ' 改!
            dataGrid.TextMatrix(.Index, 1) = dataGrid.TextMatrix(.Index, 2)
            dataGrid.TextMatrix(.Index, 2) = ""
            If .DataType = vbLong Then
                vRetLng = Val(vStr)
                CopyMemory vMem(0), ByVal VarPtr(vRetLng), 4
            ElseIf .DataType = vbSingle Then
                vRetFloat = Val(vStr)
                CopyMemory vMem(0), ByVal VarPtr(vRetFloat), 4
            End If
            EditMem BroodHwnd, vVal + .Address3Patch, vMem
        End If
    End With
    Next
End Sub

' 解除修改的代码
Sub AntiRefresh()
    Dim vMem(0 To 3) As Byte
    Dim vVal As Long
    Dim vRetLng As Long, vRetFloat As Single
    Dim i As Integer
    For i = sMemoryMin To sMemoryMax
        sMemoryState(i).ok = True
        ' 恢复正常代码
        If sMemory(i).Address1 <> 0 Then
            EditMem BroodHwnd, sMemory(i).Address1, sMemory(i).Mem0
        End If
    Next
End Sub
