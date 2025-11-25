Imports System
Imports System.Media
Imports System.IO
Imports System.ComponentModel

Namespace GameFramework.Audio
    Public Class SoundEffect
        Implements IDisposable
        Implements INotifyPropertyChanged
        
        Private soundPlayerField As SoundPlayer
        Private filePathField As String
        Private nameField As String
        Private volumeField As Single
        Private isMutedField As Boolean
        Private isLoopingField As Boolean
        Private isPlayingField As Boolean
        Private playCountField As Integer
        Private durationField As TimeSpan
        Private loadExceptionField As Exception
        
        Public Event PropertyChanged As PropertyChangedEventHandler
        Public Event PlaybackStarted As EventHandler
        Public Event PlaybackStopped As EventHandler
        Public Event PlaybackLooped As EventHandler
        
        Public Property Name As String
            Get
                Return nameField
            End Get
            Set(value As String)
                If nameField <> value Then
                    nameField = value
                    OnPropertyChanged(NameOf(Name))
                End If
            End Set
        End Property
        
        Public Property FilePath As String
            Get
                Return filePathField
            End Get
            Private Set(value As String)
                filePathField = value
            End Set
        End Property
        
        Public Property Volume As Single
            Get
                Return volumeField
            End Get
            Set(value As Single)
                Dim newVolume As Single = MathHelper.Clamp(value, 0.0F, 1.0F)
                If Math.Abs(volumeField - newVolume) > Single.Epsilon Then
                    volumeField = newVolume
                    OnPropertyChanged(NameOf(Volume))
                    UpdatePlayerVolume()
                End If
            End Set
        End Property
        
        Public Property IsMuted As Boolean
            Get
                Return isMutedField
            End Get
            Set(value As Boolean)
                If isMutedField <> value Then
                    isMutedField = value
                    OnPropertyChanged(NameOf(IsMuted))
                    UpdatePlayerVolume()
                End If
            End Set
        End Property
        
        Public ReadOnly Property IsPlaying As Boolean
            Get
                Return isPlayingField
            End Get
        End Property
        
        Public ReadOnly Property IsLooping As Boolean
            Get
                Return isLoopingField
            End Get
        End Property
        
        Public ReadOnly Property PlayCount As Integer
            Get
                Return playCountField
            End Get
        End Property
        
        Public ReadOnly Property Duration As TimeSpan
            Get
                Return durationField
            End Get
        End Property
        
        Public ReadOnly Property IsLoaded As Boolean
            Get
                Return soundPlayerField IsNot Nothing AndAlso loadExceptionField Is Nothing
            End Get
        End Property
        
        Public ReadOnly Property LoadException As Exception
            Get
                Return loadExceptionField
            End Get
        End Property
        
        Public Sub New(filePath As String)
            Me.New()
            LoadFromFile(filePath)
        End Sub
        
        Public Sub New(stream As Stream)
            Me.New()
            LoadFromStream(stream)
        End Sub
        
        Public Sub New(soundData As Byte())
            Me.New()
            LoadFromBytes(soundData)
        End Sub
        
        Private Sub New()
            nameField = String.Empty
            filePathField = String.Empty
            volumeField = 1.0F
            isMutedField = False
            isLoopingField = False
            isPlayingField = False
            playCountField = 0
            durationField = TimeSpan.Zero
        End Sub
        
        Private Sub LoadFromFile(filePath As String)
            Try
                If Not File.Exists(filePath) Then
                    Throw New FileNotFoundException($"Sound file not found: {filePath}")
                End If
                
                soundPlayerField = New SoundPlayer(filePath)
                filePathField = filePath
                nameField = Path.GetFileNameWithoutExtension(filePath)
                
                ' Load the sound to get duration (this is approximate for WAV files)
                soundPlayerField.Load()
                EstimateDuration()
                
                Logger.Debug($"Sound effect loaded from file: {filePath}")
            Catch ex As Exception
                loadExceptionField = ex
                Logger.Error($"Failed to load sound from file '{filePath}': {ex.Message}")
                Throw New AudioLoadException($"Could not load sound file: {filePath}", ex)
            End Try
        End Sub
        
        Private Sub LoadFromStream(stream As Stream)
            Try
                If stream Is Nothing Then
                    Throw New ArgumentNullException(NameOf(stream))
                End If
                
                If Not stream.CanRead Then
                    Throw New InvalidOperationException("Stream is not readable")
                End If
                
                soundPlayerField = New SoundPlayer(stream)
                nameField = "StreamSound_" & Guid.NewGuid().ToString("N").Substring(0, 8)
                
                ' Try to estimate duration from stream
                EstimateDuration()
                
                Logger.Debug("Sound effect loaded from stream")
            Catch ex As Exception
                loadExceptionField = ex
                Logger.Error($"Failed to load sound from stream: {ex.Message}")
                Throw New AudioLoadException("Could not load sound from stream", ex)
            End Try
        End Sub
        
        Private Sub LoadFromBytes(soundData As Byte())
            Using stream As New MemoryStream(soundData)
                LoadFromStream(stream)
            End Using
        End Sub
        
        Private Sub EstimateDuration()
            ' This is a very basic estimation for WAV files
            ' In a real implementation, you would parse the WAV header
            durationField = TimeSpan.FromSeconds(2.0) ' Default estimate
        End Sub
        
        Public Sub Play()
            If Not IsLoaded OrElse isMutedField Then Return
            
            Try
                If isLoopingField Then
                    soundPlayerField.PlayLooping()
                Else
                    soundPlayerField.Play()
                End If
                
                isPlayingField = True
                playCountField += 1
                isLoopingField = False ' Reset looping flag for next play
                
                RaiseEvent PlaybackStarted(Me, EventArgs.Empty)
                OnPropertyChanged(NameOf(IsPlaying))
                OnPropertyChanged(NameOf(PlayCount))
                
                Logger.Debug($"Sound played: {Name}")
            Catch ex As Exception
                Logger.Error($"Failed to play sound '{Name}': {ex.Message}")
                isPlayingField = False
            End Try
        End Sub
        
        Public Sub PlayLooping()
            If Not IsLoaded OrElse isMutedField Then Return
            
            Try
                isLoopingField = True
                soundPlayerField.PlayLooping()
                isPlayingField = True
                playCountField += 1
                
                RaiseEvent PlaybackStarted(Me, EventArgs.Empty)
                RaiseEvent PlaybackLooped(Me, EventArgs.Empty)
                OnPropertyChanged(NameOf(IsPlaying))
                OnPropertyChanged(NameOf(IsLooping))
                OnPropertyChanged(NameOf(PlayCount))
                
                Logger.Debug($"Sound started looping: {Name}")
            Catch ex As Exception
                Logger.Error($"Failed to play looping sound '{Name}': {ex.Message}")
                isPlayingField = False
                isLoopingField = False
            End Try
        End Sub
        
        Public Sub [Stop]()
            If Not IsLoaded Then Return
            
            Try
                soundPlayerField.Stop()
                isPlayingField = False
                isLoopingField = False
                
                RaiseEvent PlaybackStopped(Me, EventArgs.Empty)
                OnPropertyChanged(NameOf(IsPlaying))
                OnPropertyChanged(NameOf(IsLooping))
                
                Logger.Debug($"Sound stopped: {Name}")
            Catch ex As Exception
                Logger.Error($"Failed to stop sound '{Name}': {ex.Message}")
            End Try
        End Sub
        
        Public Sub Reload()
            If String.IsNullOrEmpty(filePathField) Then Return
            
            [Stop]()
            soundPlayerField?.Dispose()
            soundPlayerField = Nothing
            loadExceptionField = Nothing
            
            LoadFromFile(filePathField)
        End Sub
        
        Private Sub UpdatePlayerVolume()
            ' Note: SoundPlayer doesn't support volume control directly
            ' This would require using a different audio API for volume control
            ' For now, we just track the volume property
        End Sub
        
        Protected Overridable Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
        
#Region "IDisposable Implementation"
        Private disposedValue As Boolean = False
        
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    [Stop]()
                    soundPlayerField?.Dispose()
                    soundPlayerField = Nothing
                End If
                disposedValue = True
            End If
        End Sub
        
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
        
        Public Overrides Function ToString() As String
            Return $"SoundEffect: {Name} (Loaded: {IsLoaded}, Playing: {IsPlaying})"
        End Function
    End Class
End Namespace