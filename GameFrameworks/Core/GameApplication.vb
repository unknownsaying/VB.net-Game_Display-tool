Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Windows.Forms
Imports GameFramework.Graphics
Imports GameFramework.Input
Imports GameFramework.Audio
Imports GameFramework.Resources

Namespace GameFramework.Core
    Public MustInherit Class GameApplication
        Implements IDisposable
        
        Protected gameWindow As GameWindow
        Protected renderer As Renderer
        Protected inputHandler As InputHandler
        Protected soundSystem As SoundSystem
        Protected resourceLoader As ResourceLoader
        Protected gameTime As GameTime
        Protected isInitialized As Boolean = False
        Protected isRunning As Boolean = False
        
        Public ReadOnly Property Window As GameWindow
            Get
                Return gameWindow
            End Get
        End Property
        
        Public ReadOnly Property Graphics As Renderer
            Get
                Return renderer
            End Get
        End Property
        
        Public ReadOnly Property Input As InputHandler
            Get
                Return inputHandler
            End Get
        End Property
        
        Public ReadOnly Property Audio As SoundSystem
            Get
                Return soundSystem
            End Get
        End Property
        
        Public ReadOnly Property Content As ResourceLoader
            Get
                Return resourceLoader
            End Get
        End Property
        
        Public Sub New()
            InitializeCoreSystems()
        End Sub
        
        Private Sub InitializeCoreSystems()
            gameTime = New GameTime()
            gameWindow = New GameWindow()
            renderer = New Renderer(gameWindow)
            inputHandler = New InputHandler(gameWindow)
            soundSystem = New SoundSystem()
            resourceLoader = New ResourceLoader()
            
            AddHandler gameWindow.Load, AddressOf OnGameLoad
            AddHandler gameWindow.FormClosing, AddressOf OnGameClosing
        End Sub
        
        Public Overridable Sub Run()
            If Not isInitialized Then
                Initialize()
                isInitialized = True
            End If
            
            isRunning = True
            gameWindow.Show()
            Application.Run(gameWindow)
        End Sub
        
        Protected MustOverride Sub Initialize()
        Protected MustOverride Sub Update(gameTime As GameTime)
        Protected MustOverride Sub Draw(gameTime As GameTime)
        
        Private Sub OnGameLoad(sender As Object, e As EventArgs)
            StartGameLoop()
        End Sub
        
        Private Sub OnGameClosing(sender As Object, e As FormClosingEventArgs)
            [Stop]()
        End Sub
        
        Private Sub StartGameLoop()
            Dim gameTimer As New Stopwatch()
            gameTimer.Start()
            
            AddHandler gameWindow.Paint, 
                Sub(s As Object, paintArgs As PaintEventArgs)
                    If isRunning Then
                        Dim elapsed As TimeSpan = gameTimer.Elapsed
                        gameTimer.Restart()
                        
                        gameTime.Update(elapsed)
                        Update(gameTime)
                        Draw(gameTime)
                        
                        paintArgs.Graphics.DrawImage(renderer.BackBuffer, 0, 0)
                    End If
                End Sub
            
            AddHandler gameWindow.KeyDown, 
                Sub(s As Object, keyArgs As KeyEventArgs)
                    inputHandler.ProcessKeyDown(keyArgs)
                End Sub
        End Sub
        
        Public Overridable Sub [Stop]()
            isRunning = False
            soundSystem.Dispose()
            renderer.Dispose()
            gameWindow.Close()
        End Sub
        
#Region "IDisposable Implementation"
        Private disposedValue As Boolean = False
        
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    soundSystem?.Dispose()
                    renderer?.Dispose()
                    gameWindow?.Dispose()
                    resourceLoader?.Dispose()
                End If
                disposedValue = True
            End If
        End Sub
        
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
    
    Public Class GameTime
        Private _totalGameTime As TimeSpan
        Private _elapsedGameTime As TimeSpan
        
        Public Property TotalGameTime As TimeSpan
            Get
                Return _totalGameTime
            End Get
            Private Set(value As TimeSpan)
                _totalGameTime = value
            End Set
        End Property
        
        Public Property ElapsedGameTime As TimeSpan
            Get
                Return _elapsedGameTime
            End Get
            Private Set(value As TimeSpan)
                _elapsedGameTime = value
            End Set
        End Property
        
        Public ReadOnly Property TotalSeconds As Double
            Get
                Return _totalGameTime.TotalSeconds
            End Get
        End Property
        
        Public Sub Update(elapsedTime As TimeSpan)
            _elapsedGameTime = elapsedTime
            _totalGameTime = _totalGameTime.Add(elapsedTime)
        End Sub
    End Class
End Namespace