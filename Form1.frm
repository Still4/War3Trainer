VERSION 5.00
Object = "{0ECD9B60-23AA-11D0-B351-00A0C9055D8E}#6.0#0"; "MSHFLXGD.OCX"
Begin VB.Form Form1 
   AutoRedraw      =   -1  'True
   BorderStyle     =   1  'Fixed Single
   Caption         =   "[tc]魔兽3内存修改器"
   ClientHeight    =   6150
   ClientLeft      =   150
   ClientTop       =   435
   ClientWidth     =   11040
   Icon            =   "Form1.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   6150
   ScaleWidth      =   11040
   StartUpPosition =   2  '屏幕中心
   Begin VB.CommandButton cmdAntiRefreshGame 
      Caption         =   "终止刷新"
      Enabled         =   0   'False
      Height          =   375
      Left            =   8040
      TabIndex        =   13
      ToolTipText     =   "刷新后如果没有反应，请单击这里"
      Top             =   240
      Width           =   1215
   End
   Begin VB.Timer Timer2 
      Enabled         =   0   'False
      Interval        =   500
      Left            =   10560
      Top             =   240
   End
   Begin VB.TextBox txtVal2 
      Height          =   270
      Left            =   8640
      TabIndex        =   10
      ToolTipText     =   "编辑完后，请在别的单元格内双击"
      Top             =   3000
      Visible         =   0   'False
      Width           =   1500
   End
   Begin VB.CommandButton cmdEdit_Game 
      Caption         =   "修改"
      Enabled         =   0   'False
      Height          =   375
      Left            =   9360
      TabIndex        =   14
      ToolTipText     =   "修改后，游戏中不会立刻有反应，尝试在游戏中改变相应的值"
      Top             =   240
      Width           =   1215
   End
   Begin VB.CommandButton cmdReFresh_Game 
      Caption         =   "刷新"
      Height          =   375
      Left            =   6720
      TabIndex        =   12
      ToolTipText     =   "检测金钱"
      Top             =   240
      Width           =   1215
   End
   Begin VB.CommandButton cmdAntiRefresh 
      Caption         =   "终止刷新"
      Enabled         =   0   'False
      Height          =   375
      Left            =   2520
      TabIndex        =   7
      ToolTipText     =   "刷新后如果没有反应，请单击这里"
      Top             =   1200
      Width           =   1215
   End
   Begin VB.CommandButton cmdReFresh 
      Caption         =   "刷新"
      Height          =   375
      Left            =   1200
      TabIndex        =   6
      ToolTipText     =   "单击后请回到游戏，点击某个单位，然后切出游戏"
      Top             =   1200
      Width           =   1215
   End
   Begin VB.CommandButton cmdEdit 
      Caption         =   "修改"
      Enabled         =   0   'False
      Height          =   375
      Left            =   3840
      TabIndex        =   8
      ToolTipText     =   "改！当心别把修改应用到别人头上"
      Top             =   1200
      Width           =   1215
   End
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   500
      Left            =   4440
      Top             =   720
   End
   Begin VB.TextBox txtVal1 
      Height          =   270
      Left            =   3240
      TabIndex        =   4
      ToolTipText     =   "编辑完后，请在别的单元格内双击"
      Top             =   2760
      Visible         =   0   'False
      Width           =   1500
   End
   Begin VB.Frame Frame2 
      Caption         =   "单位"
      Height          =   4575
      Left            =   240
      TabIndex        =   3
      ToolTipText     =   "单位的相关属性"
      Top             =   1320
      Width           =   5055
      Begin MSHierarchicalFlexGridLib.MSHFlexGrid gridHero 
         Height          =   3975
         Left            =   240
         TabIndex        =   5
         ToolTipText     =   "双击单元格可以编辑"
         Top             =   360
         Width           =   4575
         _ExtentX        =   8070
         _ExtentY        =   7011
         _Version        =   393216
         Cols            =   3
         AllowBigSelection=   0   'False
         ScrollBars      =   0
         _NumberOfBands  =   1
         _Band(0).Cols   =   3
         _Band(0).GridLinesBand=   5
         _Band(0).TextStyleBand=   0
         _Band(0).TextStyleHeader=   0
      End
   End
   Begin VB.Frame Frame1 
      Caption         =   "程序"
      Height          =   855
      Left            =   240
      TabIndex        =   0
      ToolTipText     =   "检测游戏是否运行"
      Top             =   240
      Width           =   2295
      Begin VB.TextBox txtHwnd 
         Height          =   315
         Left            =   240
         Locked          =   -1  'True
         TabIndex        =   1
         TabStop         =   0   'False
         Text            =   "状态"
         Top             =   360
         Width           =   735
      End
      Begin VB.CommandButton cmdRe 
         Caption         =   "刷新"
         Height          =   315
         Left            =   1080
         TabIndex        =   2
         ToolTipText     =   "重新检测游戏是否运行"
         Top             =   360
         Width           =   1035
      End
   End
   Begin VB.Frame Frame3 
      Caption         =   "游戏"
      Height          =   5535
      Left            =   5520
      TabIndex        =   9
      ToolTipText     =   "游戏的属性"
      Top             =   360
      Width           =   5295
      Begin MSHierarchicalFlexGridLib.MSHFlexGrid gridGame 
         Height          =   4935
         Left            =   240
         TabIndex        =   11
         ToolTipText     =   "双击单元格可以编辑"
         Top             =   360
         Width           =   4815
         _ExtentX        =   8493
         _ExtentY        =   8705
         _Version        =   393216
         Cols            =   3
         AllowBigSelection=   0   'False
         ScrollBars      =   2
         _NumberOfBands  =   1
         _Band(0).Cols   =   3
         _Band(0).GridLinesBand=   5
         _Band(0).TextStyleBand=   0
         _Band(0).TextStyleHeader=   0
      End
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub cmdRe_Click()
    reScanGame
End Sub

Private Sub Form_Load()
    modWar3.dataInit gridHero
    gridHero.TextMatrix(0, 1) = "原值"
    gridHero.TextMatrix(0, 2) = "目标"
    gridHero.ColWidth(0) = 100 * Screen.TwipsPerPixelX
    gridHero.ColWidth(1) = 100 * Screen.TwipsPerPixelX
    gridHero.ColWidth(2) = 100 * Screen.TwipsPerPixelX
    txtVal1.Width = 100 * Screen.TwipsPerPixelX
    gridHero.TextMatrix(gridHero.Rows - 1, 0) = "（没有内容）"
    
    modWar3Money.dataInit gridGame
    gridGame.TextMatrix(0, 1) = "原值"
    gridGame.TextMatrix(0, 2) = "目标"
    gridGame.ColWidth(0) = 100 * Screen.TwipsPerPixelX
    gridGame.ColWidth(1) = 100 * Screen.TwipsPerPixelX
    gridGame.ColWidth(2) = 100 * Screen.TwipsPerPixelX
    txtVal2.Width = 100 * Screen.TwipsPerPixelX
    gridGame.TextMatrix(gridGame.Rows - 1, 0) = "（没有内容）"

    ToKen
    reScanGame
End Sub

Public Sub reScanGame()
    BroodHwnd = GetBroodHwnd
    If BroodHwnd = 0 Then
        txtHwnd = "未启动"
        'Timer1.Enabled = False
    Else
        txtHwnd = Hex(BroodHwnd)
        'Timer1.Enabled = True
        'Call modWar3.goInject
    End If
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ' 要恢复内存！
    Call cmdAntiRefresh_Click
    Call cmdAntiRefreshGame_Click
End Sub

'实现编辑功能
Private Sub gridGame_DblClick()
    Static firstClick As Boolean
    Static lastClickRow As Long
    Static lastClickCol As Long
    
    txtVal2.Visible = False
    '如果初次双击，那么就执行如下代码
    If firstClick = True Then
        lastClickRow = gridGame.Row
        lastClickCol = gridGame.Col
        txtVal2.Text = gridGame.TextMatrix(lastClickRow, lastClickCol)
    End If
    '将文本框放置到当前单元格处
    Dim LeftOfText% '文本框的left属性
    Dim TopOfText% '文本框的top属性
    Dim selrow% '当前行
    Dim selcol% '当前列
    selrow = gridGame.Row
    selcol = gridGame.Col
    If selcol = 2 Then selcol = 1
    'LeftOfText = Frame3.Left + gridGame.Left + gridGame.ColPos(selcol) + 45
    LeftOfText = Frame3.Left + gridGame.Left + gridGame.ColPos(2) + 45
    TopOfText = Frame3.Top + gridGame.Top + gridGame.RowPos(selrow) + 45
    txtVal2.Move LeftOfText, TopOfText
    txtVal2.Visible = True
    '如果不是初次双击，那么就执行如下代码
    If firstClick = False Then
        gridGame.TextMatrix(lastClickRow, lastClickCol) = txtVal2.Text
        lastClickRow = gridGame.Row
        lastClickCol = gridGame.Col
    End If
    txtVal2.Text = gridGame.TextMatrix(selrow, selcol)
    '已经不是第一次进行双击操作
    firstClick = False
    txtVal2.SetFocus
End Sub

'实现编辑功能
Private Sub gridHero_DblClick()
    Static firstClick As Boolean
    Static lastClickRow As Long
    Static lastClickCol As Long
    
    txtVal1.Visible = False
    '如果初次双击，那么就执行如下代码
    If firstClick = True Then
        lastClickRow = gridHero.Row
        lastClickCol = gridHero.Col
        txtVal1.Text = gridHero.TextMatrix(lastClickRow, lastClickCol)
    End If
    '将文本框放置到当前单元格处
    Dim LeftOfText% '文本框的left属性
    Dim TopOfText% '文本框的top属性
    Dim selrow% '当前行
    Dim selcol% '当前列
    selrow = gridHero.Row
    selcol = gridHero.Col
    If selcol = 2 Then selcol = 1
    'LeftOfText = Frame2.Left + gridHero.Left + gridHero.ColPos(selcol) + 45
    LeftOfText = Frame2.Left + gridHero.Left + gridHero.ColPos(2) + 45
    TopOfText = Frame2.Top + gridHero.Top + gridHero.RowPos(selrow) + 45
    txtVal1.Move LeftOfText, TopOfText
    txtVal1.Visible = True
    '如果不是初次双击，那么就执行如下代码
    If firstClick = False Then
        gridHero.TextMatrix(lastClickRow, lastClickCol) = txtVal1.Text
        lastClickRow = gridHero.Row
        lastClickCol = gridHero.Col
    End If
    txtVal1.Text = gridHero.TextMatrix(selrow, selcol)
    '已经不是第一次进行双击操作
    firstClick = False
    txtVal1.SetFocus
End Sub

Private Sub Timer1_Timer()
    modWar3.findHero gridHero
    
    Dim ok As Boolean
    Dim i As Long
    ok = True
    ' 如果完全ok的话，终止timer
    For i = modWar3.sMemoryMin To modWar3.sMemoryMax
        If Not modWar3.sMemoryState(i).ok Then
            ok = False
            Exit For
        End If
    Next
    If ok Then
        Call cmdAntiRefresh_Click
    End If
End Sub

Private Sub cmdReFresh_Click()
    Call modWar3.goInject
    Timer1.Enabled = True
    cmdAntiRefresh.Enabled = True
    cmdEdit.Enabled = False
End Sub

Private Sub cmdEdit_Click()
    modWar3.editHero gridHero
End Sub

Private Sub cmdAntiRefresh_Click()
    Call modWar3.AntiRefresh
    Timer1.Enabled = False
    cmdEdit.Enabled = True
    cmdAntiRefresh.Enabled = False
End Sub

Private Sub cmdReFresh_Game_Click()
    Call modWar3Money.goInject
    Timer2.Enabled = True
    cmdAntiRefreshGame.Enabled = True
    cmdEdit_Game.Enabled = False
End Sub

Private Sub cmdAntiRefreshGame_Click()
    Call modWar3Money.AntiRefresh
    Timer2.Enabled = False
    cmdEdit_Game.Enabled = True
    cmdAntiRefreshGame.Enabled = False
End Sub

Private Sub cmdEdit_Game_Click()
    Dim ret As Long
    If Val(gridGame.TextMatrix(4, 1)) = 0 Then  ' 1p的当前人口为0，不太可能
        ret = MsgBox("我觉得上次搜索的数据不可靠，建议刷新。仍然继续吗？", vbYesNo, "警告")
        If ret = vbNo Then Exit Sub
    End If

    modWar3Money.editHero gridGame
End Sub

Private Sub Timer2_Timer()
    modWar3Money.findHero gridGame
    
    If modWar3Money.sMoneyMemoryState.ok Then
        Call cmdAntiRefreshGame_Click
    End If
End Sub
