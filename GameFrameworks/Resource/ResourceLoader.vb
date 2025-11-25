Imports System
Imports System.Drawing
Imports System.IO
Imports System.Media
Imports System.Collections.Generic

Namespace GameFramework.Resources
    Public Class ResourceLoader
        Private imageCache As Dictionary(Of String, Bitmap)
        Private soundCache As Dictionary(Of String, SoundPlayer)

        Public Sub New()
            imageCache = New Dictionary(Of String, Bitmap)()
            soundCache = New Dictionary(Of String, SoundPlayer)()
        End Sub

        Public Function LoadImage(path As String) As Bitmap
            If imageCache.ContainsKey(path) Then
                Return imageCache(path)
            End If

            If File.Exists(path) Then
                Dim image As Bitmap = New Bitmap(path)
                imageCache(path) = image
                Return image
            Else
                Throw New FileNotFoundException("Image file not found: " & path)
            End If
        End Function

        Public Function LoadSound(path As String) As SoundPlayer
            If soundCache.ContainsKey(path) Then
                Return soundCache(path)
            End If

            If File.Exists(path) Then
                Dim sound As New SoundPlayer(path)
                soundCache(path) = sound
                Return sound
            Else
                Throw New FileNotFoundException("Sound file not found: " & path)
            End If
        End Function

        Public Sub PreloadImages(paths As IEnumerable(Of String))
            For Each path In paths
                LoadImage(path)
            Next
        End Sub

        Public Sub PreloadSounds(paths As IEnumerable(Of String))
            For Each path In paths
                LoadSound(path)
            Next
        End Sub

        Public Sub ClearImageCache()
            For Each image In imageCache.Values
                image.Dispose()
            Next
            imageCache.Clear()
        End Sub

        Public Sub ClearSoundCache()
            For Each sound In soundCache.Values
                sound.Dispose()
            Next
            soundCache.Clear()
        End Sub
    End Class
End Namespace