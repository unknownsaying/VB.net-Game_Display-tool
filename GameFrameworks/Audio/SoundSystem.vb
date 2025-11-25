Imports System
Imports System.Media
Imports System.Collections.Generic
Imports System.IO

Namespace GameFramework.Audio
    Public Class SoundSystem
        Implements IDisposable
        
        Private soundEffects As Dictionary(Of String, SoundEffect)
        Private musicPlayer As MusicPlayer
        Private masterVolume As Single
        
        Public Property Volume As Single
            Get
                Return masterVolume
            End Get
            Set(value As Single)
                masterVolume = Math.Max(0.0F, Math.Min(1.0F, value))
                UpdateVolumes()
            End Set
        End Property
        
        Public Sub New()
            soundEffects = New Dictionary(Of String, SoundEffect)()
            musicPlayer = New MusicPlayer()
            masterVolume = 1.0F
        End Sub
        
        Public Sub LoadSound(name As String, soundData As Stream)
            Dim soundEffect As New SoundEffect(soundData)
            soundEffects(name) = soundEffect
            soundEffect.Volume = masterVolume
        End Sub
        
        Public Sub PlaySound(name As String)
            If soundEffects.ContainsKey(name) Then
                soundEffects(name).Play()
            End If
        End Sub
        
        Public Sub PlaySoundLooping(name As String)
            If soundEffects.ContainsKey(name) Then
                soundEffects(name).PlayLooping()
            End If
        End Sub
        
        Public Sub StopSound(name As String)
            If soundEffects.ContainsKey(name) Then
                soundEffects(name).Stop()
            End If
        End Sub
        
        Public Sub PlayMusic(musicData As Stream, Optional loop As Boolean = True)
            musicPlayer.Play(musicData, loop)
            musicPlayer.Volume = masterVolume
        End Sub
        
        Public Sub StopMusic()
            musicPlayer.Stop()
        End Sub
        
        Public Sub PauseMusic()
            musicPlayer.Pause()
        End Sub
        
        Public Sub ResumeMusic()
            musicPlayer.Resume()
        End Sub
        
        Private Sub UpdateVolumes()
            For Each effect In soundEffects.Values
                effect.Volume = masterVolume
            Next
            musicPlayer.Volume = masterVolume
        End Sub
        
#Region "IDisposable Implementation"
        Private disposedValue As Boolean = False
        
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    musicPlayer.Dispose()
                    For Each effect In soundEffects.Values
                        effect.Dispose()
                    Next
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
    
    Public Class SoundEffect
        Implements IDisposable
        
        Private soundPlayer As SoundPlayer
        Private isDisposed As Boolean
        
        Public Property Volume As Single
        
        Public Sub New(stream As Stream)
            soundPlayer = New SoundPlayer(stream)
            Volume = 1.0F
        End Sub
        
        Public Sub Play()
            If Not isDisposed Then
                soundPlayer.Play()
            End If
        End Sub
        
        Public Sub PlayLooping()
            If Not isDisposed Then
                soundPlayer.PlayLooping()
            End If
        End Sub
        
        Public Sub [Stop]()
            If Not isDisposed Then
                soundPlayer.Stop()
            End If
        End Sub
        
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not isDisposed Then
                soundPlayer.Dispose()
                isDisposed = True
            End If
        End Sub
    End Class
    
    Public Class MusicPlayer
        Implements IDisposable
        
        ' Implementation for background music
        Public Sub Play(stream As Stream, loop As Boolean)
            ' Implementation for music playback
        End Sub
        
        Public Sub [Stop]()
            ' Stop music
        End Sub
        
        Public Sub Pause()
            ' Pause music
        End Sub
        
        Public Sub Resume()
            ' Resume music
        End Sub
        
        Public Property Volume As Single
        
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Cleanup
        End Sub
    End Class
End Namespace