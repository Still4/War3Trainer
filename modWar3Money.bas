Attribute VB_Name = "modWar3Money"
' 警告：本文件的DataType定义是这样的：
' 0 - int
' 1 - int x10

Public sMoneyMemory(1 To 40) As t_MemInfo
Public sMoneyMemoryState As t_MemManage
Public Const sMoneyMemoryMin As Long = 1
Public Const sMoneyMemoryMax As Long = 40

Public Sub dataInit(dataGrid As MSHFlexGrid)
    On Error Resume Next

    Dim i As Integer, j As Integer
    sMoneyMemory(1).Address3Patch = &H190
    sMoneyMemory(5).Address3Patch = &H1410
    sMoneyMemory(9).Address3Patch = &H26A0
    sMoneyMemory(13).Address3Patch = &H3920
    sMoneyMemory(17).Address3Patch = &H4BB0
    sMoneyMemory(21).Address3Patch = &H5E30
    sMoneyMemory(25).Address3Patch = &H70C0
    sMoneyMemory(29).Address3Patch = &H18350 - &H10000  ' 无语，VB里面默认是Integer
    sMoneyMemory(33).Address3Patch = &H195D0 - &H10000
    sMoneyMemory(37).Address3Patch = &H1A860 - &H10000
    For i = sMoneyMemoryMin To sMoneyMemoryMax Step 4
        sMoneyMemory(i + 0).Name = "P" & Trim(Str(i \ 4 + 1)) & " - 金"
        sMoneyMemory(i + 1).Name = "P" & Trim(Str(i \ 4 + 1)) & " - 木"
        sMoneyMemory(i + 2).Name = "P" & Trim(Str(i \ 4 + 1)) & " - 最大人口"
        sMoneyMemory(i + 3).Name = "P" & Trim(Str(i \ 4 + 1)) & " - 当前人口"
        
        sMoneyMemory(i + 1).Address3Patch = sMoneyMemory(i + 0).Address3Patch + &H80
        sMoneyMemory(i + 2).Address3Patch = sMoneyMemory(i + 0).Address3Patch + &H180
        sMoneyMemory(i + 3).Address3Patch = sMoneyMemory(i + 0).Address3Patch + &H200
        
        sMoneyMemory(i + 0).DataType = 1
        sMoneyMemory(i + 1).DataType = 1
        sMoneyMemory(i + 2).DataType = 0
        sMoneyMemory(i + 3).DataType = 0
    Next
    For i = sMoneyMemoryMin To sMoneyMemoryMax
        j = j + 1
        With sMoneyMemory(i)
            dataGrid.AddItem .Name, j
            'dataGrid.AddItem .Address3Patch, j ' DEBUG
            .Address1 = 0
            .Address2 = 0
            .Address3 = &H46101C
            .Index = j
        End With
    Next
    
    sMoneyMemory(1).Address1 = &H6F088E78
    sMoneyMemory(1).Address2 = &H44A8C0
    sMoneyMemory(1).Address3 = &H46101C
    sMoneyMemory(1).Mem0 = LoadResData(100, "MONEY")
    sMoneyMemory(1).Mem1 = LoadResData(101, "MONEY")
    sMoneyMemory(1).Mem2 = LoadResData(102, "MONEY")
End Sub

Public Sub goInject()
    Dim i As Integer
    Dim var1(0 To 3) As Byte
    
    var1(0) = 0
    var1(1) = 0
    var1(2) = 0
    var1(3) = 0
    
    EditMem BroodHwnd, sMoneyMemory(1).Address2, sMoneyMemory(1).Mem2
    EditMem BroodHwnd, sMoneyMemory(1).Address1, sMoneyMemory(1).Mem1
    EditMem BroodHwnd, sMoneyMemory(1).Address3, var1
    
    sMoneyMemoryState.ok = False
    sMoneyMemoryState.Address1 = 0
End Sub

Public Sub findHero(dataGrid As MSHFlexGrid)
    Dim vMem(0 To 3) As Byte
    Dim vVal As Long
    Dim vRetLng As Long
    Dim i As Integer
    
    If sMoneyMemoryState.ok Then
        ' 恢复正常代码
    Else
        ' 看写了数据吗？
        With sMoneyMemory(1)
            ReadMem BroodHwnd, .Address3, vMem
            CopyMemory ByVal VarPtr(vVal), vMem(0), 4
            If vMem(0) = 0 _
            And vMem(1) = 0 _
            And vMem(2) = 0 _
            And vMem(3) = 0 Then
                ' 还没有搞定
            Else
                ' Money专用：
                For i = sMoneyMemoryMin To sMoneyMemoryMax

                    ' 读取
                    ReadMem BroodHwnd, vVal + sMoneyMemory(i).Address3Patch, vMem
                    CopyMemory ByVal VarPtr(vRetLng), vMem(0), 4
                    ' Money专用！
                    If .DataType = 1 Then vRetLng = vRetLng / 10
                    dataGrid.TextMatrix(sMoneyMemory(i).Index, 1) = Trim(Str(vRetLng))
                Next
                ' 记录、恢复正常代码
                sMoneyMemoryState.ok = True
                sMoneyMemoryState.Address1 = vVal
                EditMem BroodHwnd, .Address1, .Mem0
            End If
        End With
    End If
End Sub

Public Sub editHero(dataGrid As MSHFlexGrid)
    Dim vMem(0 To 3) As Byte
    Dim vVal As Long
    Dim vStr As String
    Dim vRetLng As Long

    Dim i As Integer
    For i = sMoneyMemoryMin To sMoneyMemoryMax
    With sMoneyMemory(i)
        'ReadMem BroodHwnd, .Address3, vMem
        'CopyMemory ByVal VarPtr(vVal), vMem(0), 4
        vVal = sMoneyMemoryState.Address1
        
        vStr = dataGrid.TextMatrix(.Index, 2)
        If Trim(vStr) <> "" And vVal <> 0 Then
        ' 改!
            dataGrid.TextMatrix(.Index, 1) = dataGrid.TextMatrix(.Index, 2)
            dataGrid.TextMatrix(.Index, 2) = ""
            vRetLng = Val(vStr)
            ' Money专用！
            If .DataType = 1 Then vRetLng = vRetLng * 10
            CopyMemory vMem(0), ByVal VarPtr(vRetLng), 4
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
    sMoneyMemoryState.ok = True
    ' 恢复正常代码
    EditMem BroodHwnd, sMoneyMemory(1).Address1, sMoneyMemory(1).Mem0
End Sub
