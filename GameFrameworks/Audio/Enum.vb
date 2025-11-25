Imports System

Namespace GameFramework.Audio
    Public Enum SoundType
        Effect = 0
        Music = 1
    End Enum
    
    Public Enum SoundAction
        Played = 0
        StartedLooping = 1
        Stopped = 2
    End Enum
    
    Public Enum MusicAction
        Started = 0
        Paused = 1
        Resumed = 2
        Stopped = 3
        Finished = 4
        Looped = 5
    End Enum
    
    Public Enum AudioChannel
        Master = 0
        Sound = 1
        Music = 2
    End Enum
    
    Public Class SoundEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property SoundName As String
        Public ReadOnly Property SoundType As SoundType
        Public ReadOnly Property Action As SoundAction
        Public ReadOnly Property Timestamp As DateTime
        
        Public Sub New(soundName As String, soundType As SoundType, action As SoundAction)
            Me.SoundName = soundName
            Me.SoundType = soundType
            Me.Action = action
            Me.Timestamp = DateTime.Now
        End Sub
    End Class
    
    Public Class MusicEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property MusicName As String
        Public ReadOnly Property IsLooping As Boolean
        Public ReadOnly Property Action As MusicAction
        Public ReadOnly Property Timestamp As DateTime
        
        Public Sub New(musicName As String, isLooping As Boolean, action As MusicAction)
            Me.MusicName = musicName
            Me.IsLooping = isLooping
            Me.Action = action
            Me.Timestamp = DateTime.Now
        End Sub
    End Class
    
    Public Class VolumeChangedEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property Channel As AudioChannel
        Public ReadOnly Property OldVolume As Single
        Public ReadOnly Property NewVolume As Single
        Public ReadOnly Property Timestamp As DateTime
        
        Public Sub New(channel As AudioChannel, oldVolume As Single, newVolume As Single)
            Me.Channel = channel
            Me.OldVolume = oldVolume
            Me.NewVolume = newVolume
            Me.Timestamp = DateTime.Now
        End Sub
    End Class
    
    Public Class PositionChangedEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property Position As TimeSpan
        Public ReadOnly Property Timestamp As DateTime
        
        Public Sub New(position As TimeSpan)
            Me.Position = position
            Me.Timestamp = DateTime.Now
        End Sub
    End Class
    
    Public Class AudioLoadException
        Inherits Exception
        
        Public Sub New()
            MyBase.New()
        End Sub
        
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        
        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
    End Class
    
    Public Class AudioPlaybackException
        Inherits Exception
        
        Public Sub New()
            MyBase.New()
        End Sub
        
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        
        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
    End Class
    
End Namespace