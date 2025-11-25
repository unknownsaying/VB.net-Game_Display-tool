Imports System
Imports System.IO
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace GameFramework.Audio
    Public Class MusicPlayer
        Implements IDisposable
        Implements INotifyPropertyChanged
        
#Region "Win32 API Declarations"
        <DllImport("winmm.dll")>
        Private Shared Function mciSendString(command As String, buffer As String, bufferSize As Integer, hwndCallback As IntPtr) As Integer
        End Function
        
        <DllImport("winmm.dll")>
        Private Shared Function mciGetErrorString(errorCode As Integer, buffer As String, bufferSize As Integer) As Boolean
        End Function
#End Region
        
        Private aliasNameField As String
        Private filePathField As String
        Private nameField As String
        Private volumeField As Single
        Private isMutedField As Boolean
        Private isPlayingField As Boolean
        Private isPausedField As Boolean
        Private loopField As Boolean
        Private loopCountField As Integer
        Private durationField As TimeSpan
        Private positionField As TimeSpan
        Private loadExceptionField As Exception
        
        Public Event PropertyChanged As PropertyChangedEventHandler
        Public Event MusicFinished As EventHandler
        Public Event MusicLooped As EventHandler
        Public Event PositionChanged As EventHandler(Of PositionChangedEventArgs)
        
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
                    UpdateMciVolume()
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
                    UpdateMciVolume()
                End If
            End Set
        End Property
        
        Public ReadOnly Property IsPlaying As Boolean
            Get
                Return isPlayingField
            End Get
        End Property
        
        Public ReadOnly Property IsPaused As Boolean
            Get
                Return isPausedField
            End Get
        End Property
        
        Public Property Loop As Boolean
            Get
                Return loopField
            End Get
            Set(value As Boolean)
                If loopField <> value Then
                    loopField = value
                    OnPropertyChanged(NameOf(Loop))
                End If
            End Set
        End Property
        
        Public ReadOnly Property LoopCount As Integer
            Get
                Return loopCountField
            End Get
        End Property
        
        Public ReadOnly Property Duration As TimeSpan
            Get
                Return durationField
            End Get
        End Property
        
        Public Property Position As TimeSpan
            Get
                UpdatePosition()
                Return positionField
            End Get
            Set(value As TimeSpan)
                If value < TimeSpan.Zero Then value = TimeSpan.Zero
                If value > durationField Then value = durationField
                
                If positionField <> value Then
                    positionField = value
                    SetMciPosition(value)
                    OnPropertyChanged(NameOf(Position))
                    RaiseEvent PositionChanged(Me, New PositionChangedEventArgs(value))
                End If
            End Set
        End Property
        
        Public ReadOnly Property IsLoaded As Boolean
            Get
                Return Not String.IsNullOrEmpty(aliasNameField) AndAlso loadExceptionField Is Nothing
            End Get
        End Property
        
        Public ReadOnly Property LoadException As Exception
            Get
                Return loadExceptionField
            End Get
        End Property
        
        Public ReadOnly Property Progress As Single
            Get
                If durationField.TotalMilliseconds = 0 Then Return 0.0F
                Return CSng(Position.TotalMilliseconds / durationField.TotalMilliseconds)
            End Get
        End Property
        
        Public Sub New(filePath As String)
            Me.New()
            LoadFromFile(filePath)
        End Sub
        
        Private Sub New()
            aliasNameField = String.Empty
            nameField = String.Empty
            filePathField = String.Empty
            volumeField = 1.0F
            isMutedField = False
            isPlayingField = False
            isPausedField = False
            loopField = False
            loopCountField = 0
            durationField = TimeSpan.Zero
            positionField = TimeSpan.Zero
            
            ' Generate unique alias name
            aliasNameField = "MusicPlayer_" & Guid.NewGuid().ToString("N").Substring(0, 8)
        End Sub
        
        Private Sub LoadFromFile(filePath As String)
            Try
                If Not File.Exists(filePath) Then
                    Throw New FileNotFoundException($"Music file not found: {filePath}")
                End If
                
                ' Close any existing file
                CloseMciDevice()
                
                ' Open the music file with MCI
                Dim openCommand As String = $"open ""{filePath}"" type mpegvideo alias {aliasNameField}"
                Dim result As Integer = mciSendString(openCommand, Nothing, 0, IntPtr.Zero)
                
                If result <> 0 Then
                    Throw New AudioLoadException($"MCI failed to open file (Error: {GetMciError(result)})")
                End If
                
                filePathField = filePath
                nameField = Path.GetFileNameWithoutExtension(filePath)
                
                ' Get duration
                UpdateDuration()
                
                ' Set time format to milliseconds
                mciSendString($"set {aliasNameField} time format milliseconds", Nothing, 0, IntPtr.Zero)
                
                Logger.Debug($"Music loaded: {filePath} (Duration: {durationField})")
            Catch ex As Exception
                loadExceptionField = ex
                Logger.Error($"Failed to load music '{filePath}': {ex.Message}")
                Throw New AudioLoadException($"Could not load music file: {filePath}", ex)
            End Try
        End Sub
        
        Public Sub Play()
            If Not IsLoaded Then Return
            
            Try
                Dim command As String
                
                If isPausedField Then
                    command = $"play {aliasNameField} from {CInt(positionField.TotalMilliseconds)}"
                Else
                    command = $"play {aliasNameField} from 0"
                End If
                
                If loopField Then
                    command &= " repeat"
                End If
                
                Dim result As Integer = mciSendString(command, Nothing, 0, IntPtr.Zero)
                If result <> 0 Then
                    Throw New AudioPlaybackException($"MCI play failed: {GetMciError(result)}")
                End If
                
                isPlayingField = True
                isPausedField = False
                
                OnPropertyChanged(NameOf(IsPlaying))
                OnPropertyChanged(NameOf(IsPaused))
                
                ' Start monitoring playback
                StartPlaybackMonitor()
                
                Logger.Debug($"Music started: {Name} (Loop: {loopField})")
            Catch ex As Exception
                Logger.Error($"Failed to play music '{Name}': {ex.Message}")
                isPlayingField = False
            End Try
        End Sub
        
        Public Sub Pause()
            If Not IsLoaded OrElse Not isPlayingField Then Return
            
            Try
                UpdatePosition()
                
                Dim result As Integer = mciSendString($"pause {aliasNameField}", Nothing, 0, IntPtr.Zero)
                If result <> 0 Then
                    Throw New AudioPlaybackException($"MCI pause failed: {GetMciError(result)}")
                End If
                
                isPlayingField = False
                isPausedField = True
                
                OnPropertyChanged(NameOf(IsPlaying))
                OnPropertyChanged(NameOf(IsPaused))
                
                StopPlaybackMonitor()
                
                Logger.Debug($"Music paused: {Name}")
            Catch ex As Exception
                Logger.Error($"Failed to pause music '{Name}': {ex.Message}")
            End Try
        End Sub
        
        Public Sub [Stop]()
            If Not IsLoaded Then Return
            
            Try
                Dim result As Integer = mciSendString($"stop {aliasNameField}", Nothing, 0, IntPtr.Zero)
                If result <> 0 Then
                    Throw New AudioPlaybackException($"MCI stop failed: {GetMciError(result)}")
                End If
                
                isPlayingField = False
                isPausedField = False
                positionField = TimeSpan.Zero
                loopCountField = 0
                
                OnPropertyChanged(NameOf(IsPlaying))
                OnPropertyChanged(NameOf(IsPaused))
                OnPropertyChanged(NameOf(Position))
                OnPropertyChanged(NameOf(Progress))
                
                StopPlaybackMonitor()
                
                Logger.Debug($"Music stopped: {Name}")
            Catch ex As Exception
                Logger.Error($"Failed to stop music '{Name}': {ex.Message}")
            End Try
        End Sub
        
        Public Sub Resume()
            If Not IsLoaded OrElse Not isPausedField Then Return
            
            Play() ' Resume from current position
            Logger.Debug($"Music resumed: {Name}")
        End Sub
        
        Private Sub StartPlaybackMonitor()
            ' In a real implementation, you'd use a timer to monitor playback status
            ' and handle loop events and completion
        End Sub
        
        Private Sub StopPlaybackMonitor()
            ' Stop the monitoring timer
        End Sub
        
        Private Sub UpdateDuration()
            Try
                Dim buffer As String = New String(" "c, 255)
                Dim result As Integer = mciSendString($"status {aliasNameField} length", buffer, buffer.Length, IntPtr.Zero)
                
                If result = 0 AndAlso Integer.TryParse(buffer.Trim(), Nothing) Then
                    Dim milliseconds As Integer = CInt(buffer.Trim())
                    durationField = TimeSpan.FromMilliseconds(milliseconds)
                Else
                    durationField = TimeSpan.Zero
                End If
                
                OnPropertyChanged(NameOf(Duration))
            Catch ex As Exception
                Logger.Warning($"Failed to get music duration: {ex.Message}")
                durationField = TimeSpan.Zero
            End Try
        End Sub
        
        Private Sub UpdatePosition()
            If Not IsLoaded Then Return
            
            Try
                Dim buffer As String = New String(" "c, 255)
                Dim result As Integer = mciSendString($"status {aliasNameField} position", buffer, buffer.Length, IntPtr.Zero)
                
                If result = 0 AndAlso Integer.TryParse(buffer.Trim(), Nothing) Then
                    Dim newPosition As TimeSpan = TimeSpan.FromMilliseconds(CInt(buffer.Trim()))
                    If positionField <> newPosition Then
                        positionField = newPosition
                        OnPropertyChanged(NameOf(Position))
                        OnPropertyChanged(NameOf(Progress))
                        RaiseEvent PositionChanged(Me, New PositionChangedEventArgs(newPosition))
                    End If
                End If
            Catch ex As Exception
                Logger.Warning($"Failed to get music position: {ex.Message}")
            End Try
        End Sub
        
        Private Sub SetMciPosition(position As TimeSpan)
            If Not IsLoaded Then Return
            
            Try
                Dim command As String = $"seek {aliasNameField} to {CInt(position.TotalMilliseconds)}"
                Dim result As Integer = mciSendString(command, Nothing, 0, IntPtr.Zero)
                If result <> 0 Then
                    Throw New AudioPlaybackException($"MCI seek failed: {GetMciError(result)}")
                End If
            Catch ex As Exception
                Logger.Error($"Failed to set music position: {ex.Message}")
            End Try
        End Sub
        
        Private Sub UpdateMciVolume()
            If Not IsLoaded Then Return
            
            Try
                Dim actualVolume As Single = If(isMutedField, 0.0F, volumeField)
                Dim mciVolume As Integer = CInt(actualVolume * 1000) ' MCI volume range: 0-1000
                
                Dim command As String = $"setaudio {aliasNameField} volume to {mciVolume}"
                Dim result As Integer = mciSendString(command, Nothing, 0, IntPtr.Zero)
                If result <> 0 Then
                    Throw New AudioPlaybackException($"MCI volume set failed: {GetMciError(result)}")
                End If
            Catch ex As Exception
                Logger.Error($"Failed to set music volume: {ex.Message}")
            End Try
        End Sub
        
        Private Sub CloseMciDevice()
            If String.IsNullOrEmpty(aliasNameField) Then Return
            
            Try
                [Stop]()
                mciSendString($"close {aliasNameField}", Nothing, 0, IntPtr.Zero)
            Catch ex As Exception
                Logger.Warning($"Failed to close MCI device: {ex.Message}")
            End Try
        End Sub
        
        Private Function GetMciError(errorCode As Integer) As String
            Dim buffer As String = New String(" "c, 255)
            If mciGetErrorString(errorCode, buffer, buffer.Length) Then
                Return buffer.Trim()
            Else
                Return $"Unknown MCI error: {errorCode}"
            End If
        End Function
        
        Public Sub Reload()
            If String.IsNullOrEmpty(filePathField) Then Return
            
            [Stop]()
            CloseMciDevice()
            loadExceptionField = Nothing
            
            LoadFromFile(filePathField)
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
                    CloseMciDevice()
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
            Return $"MusicPlayer: {Name} (Playing: {IsPlaying}, Position: {Position:mm\:ss}/{Duration:mm\:ss})"
        End Function
    End Class
End Namespace