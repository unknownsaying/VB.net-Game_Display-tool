Imports System
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Collections.Generic

Namespace GameFramework.Resources
    Public Class ContentManager
        Private resourceLoader As ResourceLoader
        Private embeddedResources As EmbeddedResources

        Public Sub New()
            resourceLoader = New ResourceLoader()
            embeddedResources = New EmbeddedResources()
        End Sub

        Public Function LoadTexture(path As String) As Bitmap
            Return resourceLoader.LoadImage(path)
        End Function

        Public Function LoadSound(path As String) As SoundPlayer
            Return resourceLoader.LoadSound(path)
        End Function

        Public Function LoadEmbeddedTexture(resourceName As String) As Bitmap
            Return embeddedResources.GetEmbeddedImage(resourceName)
        End Function

        Public Function LoadEmbeddedSound(resourceName As String) As Stream
            Return embeddedResources.GetEmbeddedSound(resourceName)
        End Function

        Public Sub UnloadAll()
            resourceLoader.ClearImageCache()
            resourceLoader.ClearSoundCache()
        End Sub
    End Class
End Namespace