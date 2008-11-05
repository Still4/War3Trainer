VERSION 5.00
Object = "{0ECD9B60-23AA-11D0-B351-00A0C9055D8E}#6.0#0"; "MSHFLXGD.OCX"
Object = "{38911DA0-E448-11D0-84A3-00DD01104159}#1.1#0"; "COMCT332.OCX"
Object = "{BDC217C8-ED16-11CD-956C-0000C04E4C0A}#1.1#0"; "TABCTL32.OCX"
Begin VB.Form frmMain 
   AutoRedraw      =   -1  'True
   BorderStyle     =   1  'Fixed Single
   Caption         =   "[tc]魔兽3内存修改器 v7"
   ClientHeight    =   6645
   ClientLeft      =   150
   ClientTop       =   735
   ClientWidth     =   8040
   Icon            =   "Form1.frx":0000
   LinkTopic       =   "Form1"
   LockControls    =   -1  'True
   MaxButton       =   0   'False
   ScaleHeight     =   6645
   ScaleWidth      =   8040
   StartUpPosition =   2  '屏幕中心
   Begin VB.CommandButton cmdRefresh 
      Caption         =   "刷新"
      Enabled         =   0   'False
      Height          =   375
      Left            =   5520
      TabIndex        =   11
      ToolTipText     =   "改！当心别把修改应用到别人头上"
      Top             =   6060
      Width           =   1095
   End
   Begin VB.CommandButton cmdEdit 
      Caption         =   "修改"
      Enabled         =   0   'False
      Height          =   375
      Left            =   6720
      TabIndex        =   12
      ToolTipText     =   "改！当心别把修改应用到别人头上"
      Top             =   6060
      Width           =   1095
   End
   Begin TabDlg.SSTab SSTab1 
      Height          =   5415
      Left            =   120
      TabIndex        =   2
      Top             =   480
      Width           =   7725
      _ExtentX        =   13626
      _ExtentY        =   9551
      _Version        =   393216
      Style           =   1
      Tabs            =   2
      TabsPerRow      =   2
      TabHeight       =   520
      TabCaption(0)   =   " 单位 "
      TabPicture(0)   =   "Form1.frx":000C
      Tab(0).ControlEnabled=   -1  'True
      Tab(0).Control(0)=   "Frame2"
      Tab(0).Control(0).Enabled=   0   'False
      Tab(0).Control(1)=   "Frame4"
      Tab(0).Control(1).Enabled=   0   'False
      Tab(0).Control(2)=   "txtVal1"
      Tab(0).Control(2).Enabled=   0   'False
      Tab(0).ControlCount=   3
      TabCaption(1)   =   " 游戏 "
      TabPicture(1)   =   "Form1.frx":0028
      Tab(1).ControlEnabled=   0   'False
      Tab(1).Control(0)=   "Frame3"
      Tab(1).Control(1)=   "txtVal2"
      Tab(1).ControlCount=   2
      Begin VB.TextBox txtVal2 
         Height          =   270
         Left            =   -73440
         MaxLength       =   9
         TabIndex        =   10
         ToolTipText     =   "编辑完后，请在别的单元格内双击"
         Top             =   1740
         Visible         =   0   'False
         Width           =   1500
      End
      Begin VB.TextBox txtVal1 
         Height          =   270
         Left            =   4560
         MaxLength       =   9
         TabIndex        =   7
         ToolTipText     =   "编辑完后，请在别的单元格内双击"
         Top             =   1680
         Visible         =   0   'False
         Width           =   1500
      End
      Begin VB.Frame Frame3 
         Caption         =   "经济"
         Height          =   4635
         Left            =   -74760
         TabIndex        =   8
         ToolTipText     =   "游戏的属性"
         Top             =   480
         Width           =   5175
         Begin MSHierarchicalFlexGridLib.MSHFlexGrid gridGame 
            Height          =   4035
            Left            =   240
            TabIndex        =   9
            ToolTipText     =   "双击单元格可以编辑"
            Top             =   360
            Width           =   4695
            _ExtentX        =   8281
            _ExtentY        =   7117
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
      Begin VB.Frame Frame4 
         Caption         =   "选中的单位"
         Height          =   4755
         Left            =   240
         TabIndex        =   3
         Top             =   420
         Width           =   2775
         Begin VB.ListBox lstSelectedUnits 
            Height          =   4200
            Left            =   240
            TabIndex        =   4
            Top             =   360
            Width           =   2295
         End
      End
      Begin VB.Frame Frame2 
         Caption         =   "单位"
         Height          =   4755
         Left            =   3240
         TabIndex        =   5
         ToolTipText     =   "单位的相关属性"
         Top             =   420
         Width           =   4215
         Begin MSHierarchicalFlexGridLib.MSHFlexGrid gridHero 
            Height          =   4215
            Left            =   240
            TabIndex        =   6
            ToolTipText     =   "双击单元格可以编辑"
            Top             =   360
            Width           =   3735
            _ExtentX        =   6588
            _ExtentY        =   7435
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
   Begin ComCtl3.CoolBar CoolBar1 
      Align           =   1  'Align Top
      Height          =   420
      Left            =   0
      TabIndex        =   0
      Top             =   0
      Width           =   8040
      _ExtentX        =   14182
      _ExtentY        =   741
      BandCount       =   2
      VariantHeight   =   0   'False
      _CBWidth        =   8040
      _CBHeight       =   420
      _Version        =   "6.7.9782"
      Caption1        =   "游戏状态："
      MinHeight1      =   360
      Width1          =   3600
      NewRow1         =   0   'False
      BandForeColor2  =   8388608
      Child2          =   "cmdRe"
      MinWidth2       =   1140
      MinHeight2      =   360
      Width2          =   1200
      UseCoolbarColors2=   0   'False
      NewRow2         =   0   'False
      BandStyle2      =   1
      Begin VB.CommandButton cmdRe 
         Appearance      =   0  'Flat
         Caption         =   " 查找游戏 "
         Height          =   360
         Left            =   6810
         TabIndex        =   1
         ToolTipText     =   "重新检测游戏是否运行"
         Top             =   30
         Width           =   1200
      End
   End
   Begin VB.Menu mnuFile 
      Caption         =   "文件(&F)"
      Begin VB.Menu mnuExit 
         Caption         =   "退出(&X)"
      End
   End
   Begin VB.Menu mnuHelp 
      Caption         =   "帮助(&H)"
      Begin VB.Menu mnuHelpTheme 
         Caption         =   "帮助主题(&H)"
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
    gridHero.ColWidth(0) = 85 * Screen.TwipsPerPixelX
    gridHero.ColWidth(1) = 71 * Screen.TwipsPerPixelX
    gridHero.ColWidth(2) = 74 * Screen.TwipsPerPixelX
    txtVal1.Width = 74 * Screen.TwipsPerPixelX
    gridHero.RemoveItem gridHero.Rows - 1
    
    modModifyMoney.dataInit gridGame
    gridGame.TextMatrix(0, 1) = "原值"
    gridGame.TextMatrix(0, 2) = "目标"
    gridGame.ColWidth(0) = 105 * Screen.TwipsPerPixelX
    gridGame.ColWidth(1) = 93 * Screen.TwipsPerPixelX
    gridGame.ColWidth(2) = 95 * Screen.TwipsPerPixelX
    txtVal2.Width = 95 * Screen.TwipsPerPixelX
    gridGame.RemoveItem gridGame.Rows - 1
    
    ToKen
    reScanGame
End Sub

Public Sub reScanGame()
    BroodHwnd = GetBroodHwnd
    If BroodHwnd = 0 Then
        CoolBar1.Bands(1).Caption = "游戏未运行"
        cmdRefresh.Enabled = False
    Else
        CoolBar1.Bands(1).Caption = "检测到游戏（" & Hex(BroodHwnd) & "）：" & getWar3Version
        cmdRefresh.Enabled = True
    End If
End Sub

Private Sub Form_Unload(Cancel As Integer)
    'End
End Sub

Private Sub cmdRefresh_Click()
    Select Case SSTab1.Tab
    Case 0:
        lstSelectedUnits.Clear
        Call modModifyUnit.goInject
        
        Dim i As Long
        For i = 1 To modModifyUnit.nSelectedUnitList
            lstSelectedUnits.AddItem Hex(modModifyUnit.sMemorySelectedUnitList(i).heroESI) _
                                        & ":" & getUnitName(modModifyUnit.sMemorySelectedUnitList(i).heroESI)
        Next
        
    Case 1:
        Call modModifyMoney.goInject
        Call modModifyMoney.fillData(gridGame)
    End Select
    
    cmdEdit.Enabled = True
End Sub

Private Sub cmdEdit_Click()
    Select Case SSTab1.Tab
    Case 0:
        modModifyUnit.putUnitValues gridHero
    
    Case 1:
        modModifyMoney.editData gridGame
        
    End Select
End Sub

Private Sub lstSelectedUnits_Click()
    If lstSelectedUnits.ListIndex <> -1 Then
        modWar3Common.pUnit_ThisUnit = modModifyUnit.sMemorySelectedUnitList(lstSelectedUnits.ListIndex + 1).heroESI
        
        modModifyUnit.unitCaleAllAdress
        modModifyUnit.getUnitValues gridHero
        cmdEdit.Enabled = True
    End If
End Sub

' ******************************************************************************************
' * GUI
' ******************************************************************************************
Private Sub mnuExit_Click()
    Unload Me
End Sub

Private Sub mnuHelpTheme_Click()
    MsgBox "要求还挺高……" & vbCrLf & "没啥好说的，看着办吧。" & vbCrLf & "有问题来信：tctianchi@163.com"
End Sub

Private Sub CoolBar1_MouseMove(button As Integer, Shift As Integer, x As Single, Y As Single)
    If button = 2 Then
        CoolBar1.Bands(1).ForeColor = &H66FF&
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

Private Sub gridGame_Click()
    If isGridGame_FirstClick = False And posGridGame_LastClickRow <> gridGame.Row Then
        gridGame.TextMatrix(posGridGame_LastClickRow, 2) = txtVal2.Text
        posGridGame_LastClickRow = gridGame.Row
        isGridGame_FirstClick = True
        txtVal2.Visible = False
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
