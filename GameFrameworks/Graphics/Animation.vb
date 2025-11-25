Imports System
Imports System.Collections.Generic
Imports System.Drawing

Namespace GameFramework.Graphics
    Public Class Animation
        Private frames As List(Of Rectangle)
        Private frameDurations As List(Of Single)
        Private currentFrameIndex As Integer
        Private timer As Single
        Private isLooping As Boolean
        Private isPlaying As Boolean

        Public Sub New()
            frames = New List(Of Rectangle)()
            frameDurations = New List(Of Single)()
            currentFrameIndex = 0
            timer = 0
            isLooping = True
            isPlaying = False
        End Sub

        Public Sub AddFrame(frame As Rectangle, duration As Single)
            frames.Add(frame)
            frameDurations.Add(duration)
        End Sub

        Public Sub Update(deltaTime As Single)
            If Not isPlaying OrElse frames.Count = 0 Then Return

            timer += deltaTime

            If timer >= frameDurations(currentFrameIndex) Then
                timer = 0
                currentFrameIndex += 1

                If currentFrameIndex >= frames.Count Then
                    If isLooping Then
                        currentFrameIndex = 0
                    Else
                        currentFrameIndex = frames.Count - 1
                        isPlaying = False
                    End If
                End If
            End If
        End Sub

        Public Sub Play()
            isPlaying = True
            currentFrameIndex = 0
            timer = 0
        End Sub

        Public Sub [Stop]()
            isPlaying = False
        End Sub

        Public ReadOnly Property CurrentFrame As Rectangle
            Get
                If frames.Count = 0 Then
                    Return Rectangle.Empty
                End If
                Return frames(currentFrameIndex)
            End Get
        End Property

        Public Property LoopAnimation As Boolean
            Get
                Return isLooping
            End Get
            Set(value As Boolean)
                isLooping = value
            End Set
        End Property

        Public ReadOnly Property IsRunning As Boolean
            Get
                Return isPlaying
            End Get
        End Property
    End Class
End Namespace


Namespace GameFramework.Graphics
    Public Class AnimationFrame
        Public Property Texture As Bitmap
        Public Property Duration As Single ' In seconds
        Public Property SourceRectangle As Rectangle?
        
        Public Sub New(texture As Bitmap, duration As Single, Optional sourceRect As Rectangle? = Nothing)
            Me.Texture = texture
            Me.Duration = duration
            Me.SourceRectangle = sourceRect
        End Sub
    End Class
    
    Public Class Animation
        Implements IDisposable
        
        Private frames As List(Of AnimationFrame)
        Private currentFrameIndex As Integer
        Private frameTimer As Single
        Private isPlayingField As Boolean
        Private isLoopingField As Boolean
        Private animationSpeedField As Single
        
        Public Property Name As String
        Public Property IsPaused As Boolean
        
        Public Event AnimationCompleted As EventHandler
        Public Event FrameChanged As EventHandler(Of FrameChangedEventArgs)
        
        Public Sub New(Optional name As String = "")
            Me.Name = name
            frames = New List(Of AnimationFrame)()
            currentFrameIndex = 0
            frameTimer = 0
            isPlayingField = False
            isLoopingField = True
            animationSpeedField = 1.0F
            IsPaused = False
        End Sub
        
        Public Sub AddFrame(texture As Bitmap, duration As Single, Optional sourceRect As Rectangle? = Nothing)
            frames.Add(New AnimationFrame(texture, duration, sourceRect))
        End Sub
        
        Public Sub AddFrame(frame As AnimationFrame)
            frames.Add(frame)
        End Sub
        
        Public Sub Update(deltaTime As Single)
            If Not isPlayingField OrElse IsPaused OrElse frames.Count = 0 Then Return
            
            frameTimer += deltaTime * animationSpeedField
            
            If frameTimer >= CurrentFrame.Duration Then
                frameTimer = 0
                currentFrameIndex += 1
                
                If currentFrameIndex >= frames.Count Then
                    If isLoopingField Then
                        currentFrameIndex = 0
                    Else
                        currentFrameIndex = frames.Count - 1
                        [Stop]()
                        RaiseEvent AnimationCompleted(Me, EventArgs.Empty)
                    End If
                End If
                
                RaiseEvent FrameChanged(Me, New FrameChangedEventArgs(currentFrameIndex, CurrentFrame))
            End If
        End Sub
        
        Public Sub Play()
            isPlayingField = True
            IsPaused = False
        End Sub
        
        Public Sub [Stop]()
            isPlayingField = False
            IsPaused = False
            currentFrameIndex = 0
            frameTimer = 0
        End Sub
        
        Public Sub Pause()
            IsPaused = True
        End Sub
        
        Public Sub Resume()
            IsPaused = False
        End Sub
        
        Public Sub Reset()
            currentFrameIndex = 0
            frameTimer = 0
        End Sub
        
        Public ReadOnly Property CurrentFrame As AnimationFrame
            Get
                If frames.Count = 0 Then Return Nothing
                Return frames(currentFrameIndex)
            End Get
        End Property
        
        Public ReadOnly Property FrameCount As Integer
            Get
                Return frames.Count
            End Get
        End Property
        
        Public ReadOnly Property IsPlaying As Boolean
            Get
                Return isPlayingField
            End Get
        End Property
        
        Public Property IsLooping As Boolean
            Get
                Return isLoopingField
            End Get
            Set(value As Boolean)
                isLoopingField = value
            End Set
        End Property
        
        Public Property AnimationSpeed As Single
            Get
                Return animationSpeedField
            End Get
            Set(value As Single)
                animationSpeedField = Math.Max(0.0F, value)
            End Set
        End Property
        
        Public ReadOnly Property CurrentFrameIndex As Integer
            Get
                Return currentFrameIndex
            End Get
        End Property
        
        Public Function GetFrame(index As Integer) As AnimationFrame
            If index < 0 OrElse index >= frames.Count Then
                Throw New ArgumentOutOfRangeException(NameOf(index))
            End If
            Return frames(index)
        End Function
        
#Region "IDisposable Implementation"
        Private disposedValue As Boolean = False
        
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    For Each frame In frames
                        frame.Texture?.Dispose()
                    Next
                    frames.Clear()
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
    
    Public Class FrameChangedEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property FrameIndex As Integer
        Public ReadOnly Property Frame As AnimationFrame
        
        Public Sub New(frameIndex As Integer, frame As AnimationFrame)
            Me.FrameIndex = frameIndex
            Me.Frame = frame
        End Sub
    End Class
End Namespace