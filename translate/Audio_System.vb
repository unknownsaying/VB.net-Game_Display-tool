Public Class AudioPlayer
    Private soundPlayers As New Dictionary(Of String, SoundPlayer)()
    
    Public Sub LoadSound(name As String, soundData As Byte())
        Using stream As New IO.MemoryStream(soundData)
            soundPlayers(name) = New SoundPlayer(stream)
        End Using
    End Sub
    
    Public Sub PlaySound(name As String)
        If soundPlayers.ContainsKey(name) Then
            soundPlayers(name).Play()
        End If
    End Sub
    
    Public Sub PlaySoundLooping(name As String)
        If soundPlayers.ContainsKey(name) Then
            soundPlayers(name).PlayLooping()
        End If
    End Sub
    
    Public Sub StopSound(name As String)
        If soundPlayers.ContainsKey(name) Then
            soundPlayers(name).Stop()
        End If
    End Sub
End Class