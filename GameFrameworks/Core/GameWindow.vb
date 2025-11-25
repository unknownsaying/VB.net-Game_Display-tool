Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace GameFramework.Core
    Public Class GameWindow
        Inherits Form
        
        Private windowTitle As String
        Private windowSize As Size
        Private isFullScreen As Boolean
        
        Public Sub New(Optional title As String = "VB.NET Game", 
                      Optional width As Integer = 800, 
                      Optional height As Integer = 600)
            MyBase.New()
            
            windowTitle = title
            windowSize = New Size(width, height)
            isFullScreen = False
            
            InitializeWindow()
            ConfigureWindowStyles()
        End Sub
        
        Private Sub InitializeWindow()
            Me.Text = windowTitle
            Me.ClientSize = windowSize
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.BackColor = Color.Black
            Me.DoubleBuffered = True
            Me.KeyPreview = True
        End Sub
        
        Private Sub ConfigureWindowStyles()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or 
                       ControlStyles.UserPaint Or 
                       ControlStyles.DoubleBuffer, True)
        End Sub
        
        Public Sub ToggleFullScreen()
            If isFullScreen Then
                WindowState = FormWindowState.Normal
                FormBorderStyle = FormBorderStyle.Sizable
                isFullScreen = False
            Else
                WindowState = FormWindowState.Maximized
                FormBorderStyle = FormBorderStyle.None
                isFullScreen = True
            End If
        End Sub
        
        Public ReadOnly Property IsFullScreenMode As Boolean
            Get
                Return isFullScreen
            End Get
        End Property
        
        Public Shadows Sub Show()
            MyBase.Show()
            Focus()
        End Sub
    End Class
End Namespace