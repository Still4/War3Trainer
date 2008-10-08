VERSION 5.00
Object = "{0ECD9B60-23AA-11D0-B351-00A0C9055D8E}#6.0#0"; "MSHFLXGD.OCX"
Begin VB.Form frmMain 
   AutoRedraw      =   -1  'True
   BorderStyle     =   1  'Fixed Single
   Caption         =   "[tc]魔兽3内存修改器 v5"
   ClientHeight    =   6990
   ClientLeft      =   150
   ClientTop       =   435
   ClientWidth     =   11040
   Icon            =   "Form1.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   6990
   ScaleWidth      =   11040
   StartUpPosition =   2  '屏幕中心
   Begin VB.TextBox txtVal2 
      Height          =   270
      Left            =   8640
      MaxLength       =   9
      TabIndex        =   10
      ToolTipText     =   "编辑完后，请在别的单元格内双击"
      Top             =   2760
      Visible         =   0   'False
      Width           =   1500
   End
   Begin VB.CommandButton cmdEdit_Game 
      Caption         =   "修改"
      Enabled         =   0   'False
      Height          =   375
      Left            =   9360
      TabIndex        =   13
      ToolTipText     =   "修改后，游戏中不会立刻有反应，尝试在游戏中改变相应的值"
      Top             =   240
      Width           =   1215
   End
   Begin VB.CommandButton cmdReFresh_Game 
      Caption         =   "数值更新"
      Enabled         =   0   'False
      Height          =   375
      Left            =   8040
      TabIndex        =   12
      ToolTipText     =   "检测金钱"
      Top             =   240
      Width           =   1215
   End
   Begin VB.CommandButton cmdUnit_AntiInject 
      Caption         =   "撤销注入"
      Enabled         =   0   'False
      Height          =   435
      Left            =   3960
      TabIndex        =   7
      ToolTipText     =   "刷新后如果没有反应，请单击这里"
      Top             =   660
      Width           =   1095
   End
   Begin VB.CommandButton cmdUnitInject 
      Caption         =   "注入"
      Enabled         =   0   'False
      Height          =   435
      Left            =   2760
      TabIndex        =   6
      ToolTipText     =   "单击后请回到游戏，点击某个单位，然后切出游戏"
      Top             =   660
      Width           =   1095
   End
   Begin VB.CommandButton cmdEdit 
      Caption         =   "修改"
      Enabled         =   0   'False
      Height          =   375
      Left            =   3960
      TabIndex        =   8
      ToolTipText     =   "改！当心别把修改应用到别人头上"
      Top             =   1440
      Width           =   1095
   End
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   500
      Left            =   4800
      Top             =   120
   End
   Begin VB.TextBox txtVal1 
      Height          =   270
      Left            =   3240
      MaxLength       =   9
      TabIndex        =   4
      ToolTipText     =   "编辑完后，请在别的单元格内双击"
      Top             =   2760
      Visible         =   0   'False
      Width           =   1500
   End
   Begin VB.Frame Frame2 
      Caption         =   "单位（需要先注入）"
      Height          =   5235
      Left            =   240
      TabIndex        =   3
      ToolTipText     =   "单位的相关属性"
      Top             =   1560
      Width           =   5055
      Begin MSHierarchicalFlexGridLib.MSHFlexGrid gridHero 
         Height          =   4635
         Left            =   240
         TabIndex        =   5
         ToolTipText     =   "双击单元格可以编辑"
         Top             =   360
         Width           =   4575
         _ExtentX        =   8070
         _ExtentY        =   8176
         _Version        =   393216
         Cols            =   3
         GridColor       =   -2147483637
         AllowBigSelection=   0   'False
         ScrollBars      =   2
         _NumberOfBands  =   1
         _Band(0).Cols   =   3
         _Band(0).GridLinesBand=   1
         _Band(0).TextStyleBand=   0
         _Band(0).TextStyleHeader=   0
      End
   End
   Begin VB.Frame Frame1 
      Caption         =   "程序"
      Height          =   915
      Left            =   240
      TabIndex        =   0
      ToolTipText     =   "检测游戏是否运行"
      Top             =   360
      Width           =   5055
      Begin VB.TextBox txtHwnd 
         Height          =   315
         Left            =   240
         Locked          =   -1  'True
         TabIndex        =   1
         TabStop         =   0   'False
         Text            =   "状态"
         Top             =   360
         Width           =   855
      End
      Begin VB.CommandButton cmdRe 
         Caption         =   "查找游戏"
         Height          =   435
         Left            =   1200
         TabIndex        =   2
         ToolTipText     =   "重新检测游戏是否运行"
         Top             =   300
         Width           =   1155
      End
   End
   Begin VB.Frame Frame3 
      Caption         =   "游戏（不需注入）"
      Height          =   6435
      Left            =   5520
      TabIndex        =   9
      ToolTipText     =   "游戏的属性"
      Top             =   360
      Width           =   5295
      Begin MSHierarchicalFlexGridLib.MSHFlexGrid gridGame 
         Height          =   5835
         Left            =   240
         TabIndex        =   11
         ToolTipText     =   "双击单元格可以编辑"
         Top             =   360
         Width           =   4815
         _ExtentX        =   8493
         _ExtentY        =   10292
         _Version        =   393216
         Cols            =   3
         GridColor       =   -2147483637
         AllowBigSelection=   0   'False
         ScrollBars      =   2
         _NumberOfBands  =   1
         _Band(0).Cols   =   3
         _Band(0).GridLinesBand=   1
         _Band(0).TextStyleBand=   0
         _Band(0).TextStyleHeader=   0
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Dim isGridGame_FirstClick As Boolean
Dim posGridGame_LastClickRow As Long
Dim isGridUnit_FirstClick As Boolean
Dim posGridUnit_LastClickRow As Long

Private Sub cmdRe_Click()
    reScanGame
End Sub

Private Sub Form_Load()
    isGridGame_FirstClick = True
    isGridUnit_FirstClick = True

    modModifyUnit.dataInit gridHero
    gridHero.TextMatrix(0, 1) = "原值"
    gridHero.TextMatrix(0, 2) = "目标"
    gridHero.ColWidth(0) = 100 * Screen.TwipsPerPixelX
    gridHero.ColWidth(1) = 100 * Screen.TwipsPerPixelX
    gridHero.ColWidth(2) = 102 * Screen.TwipsPerPixelX
    txtVal1.Width = 104 * Screen.TwipsPerPixelX
    gridHero.TextMatrix(gridHero.Rows - 1, 0) = "（没有内容）"
    
    modModifyMoney.dataInit gridGame
    gridGame.TextMatrix(0, 1) = "原值"
    gridGame.TextMatrix(0, 2) = "目标"
    gridGame.ColWidth(0) = 100 * Screen.TwipsPerPixelX
    gridGame.ColWidth(1) = 100 * Screen.TwipsPerPixelX
    gridGame.ColWidth(2) = 102 * Screen.TwipsPerPixelX
    txtVal2.Width = 104 * Screen.TwipsPerPixelX
    gridGame.TextMatrix(gridGame.Rows - 1, 0) = "（没有内容）"

    ToKen
    reScanGame
End Sub

Public Sub reScanGame()
    BroodHwnd = GetBroodHwnd
    If BroodHwnd = 0 Then
        txtHwnd = "未启动"
        cmdUnitInject.Enabled = False
        cmdReFresh_Game.Enabled = False
    Else
        txtHwnd = Hex(BroodHwnd)
        cmdUnitInject.Enabled = True
        cmdReFresh_Game.Enabled = True
    End If
    cmdUnit_AntiInject.Enabled = False
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ' 要恢复内存！
    Call cmdUnit_AntiInject_Click
End Sub

Private Sub cmdEdit_Click()
    modModifyUnit.putUnitValues gridHero
End Sub

Private Sub cmdReFresh_Game_Click()
    Call modModifyMoney.goInject
    Call modModifyMoney.fillData(gridGame)
    cmdEdit_Game.Enabled = True
End Sub

Private Sub cmdEdit_Game_Click()
    modModifyMoney.editData gridGame
End Sub

Private Sub cmdUnitInject_Click()
    cmdUnitInject.Enabled = False
    cmdEdit.Enabled = False
    
    Call modModifyUnit.goInject
    
    Timer1.Enabled = True
    cmdUnit_AntiInject.Enabled = True
End Sub

Private Sub cmdUnit_AntiInject_Click()
    Timer1.Enabled = False
    Call modModifyUnit.AntiRefresh
    cmdEdit.Enabled = True
    cmdUnit_AntiInject.Enabled = False
    cmdUnitInject.Enabled = True
End Sub

Private Sub Timer1_Timer()
    If modModifyUnit.detectInject Then
        Frame2.Caption = "单位：ESI = " & Hex(modWar3Common.pUnit_ThisUnit)
        Call cmdUnit_AntiInject_Click
        
        ' 根据表格获得具体数据
        modModifyUnit.getUnitValues gridHero
    End If
End Sub

' ******************************************************************************************
' * GUI
' ******************************************************************************************
'实现编辑功能
Private Sub gridGame_Click()
    If isGridGame_FirstClick = False And posGridGame_LastClickRow <> gridGame.Row Then
        gridGame.TextMatrix(posGridGame_LastClickRow, 2) = txtVal2.Text
        posGridGame_LastClickRow = gridGame.Row
        isGridGame_FirstClick = True
        txtVal2.Visible = False
    End If
End Sub

Private Sub gridGame_DblClick()
    '如果初次双击
    If isGridGame_FirstClick Then
        posGridGame_LastClickRow = gridGame.Row
        txtVal2.Text = gridGame.TextMatrix(posGridGame_LastClickRow, 1)
        
        isGridGame_FirstClick = False
    End If
    '将文本框放置到当前单元格处
    txtVal2.Move Frame3.Left + gridGame.Left + gridGame.ColPos(2) + 1 * Screen.TwipsPerPixelX, _
                Frame3.Top + gridGame.Top + gridGame.RowPos(gridGame.Row) + 1 * Screen.TwipsPerPixelY
    txtVal2.Visible = True
    txtVal2.SetFocus
End Sub

'实现编辑功能
Private Sub gridHero_Click()
    If isGridUnit_FirstClick = False And posGridUnit_LastClickRow <> gridHero.Row Then
        gridHero.TextMatrix(posGridUnit_LastClickRow, 2) = txtVal1.Text
        posGridUnit_LastClickRow = gridHero.Row
        isGridUnit_FirstClick = True
        txtVal1.Visible = False
    End If
End Sub

Private Sub gridHero_DblClick()
    '如果初次双击
    If isGridUnit_FirstClick Then
        posGridUnit_LastClickRow = gridHero.Row
        txtVal1.Text = gridHero.TextMatrix(posGridUnit_LastClickRow, 1)
        
        isGridUnit_FirstClick = False
    End If
    '将文本框放置到当前单元格处
    txtVal1.Move Frame2.Left + gridHero.Left + gridHero.ColPos(2) + 1 * Screen.TwipsPerPixelX, _
                Frame2.Top + gridHero.Top + gridHero.RowPos(gridHero.Row) + 1 * Screen.TwipsPerPixelY
    txtVal1.Visible = True
    txtVal1.SetFocus
End Sub


