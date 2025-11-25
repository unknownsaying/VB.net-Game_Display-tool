Imports System
Imports System.Drawing
Imports System.IO
Imports System.Reflection

Namespace GameFramework.Resources
    Public Class EmbeddedResources
        Public Function GetEmbeddedImage(resourceName As String) As Bitmap
            Dim assembly As Assembly = Assembly.GetExecutingAssembly()
            Using stream As Stream = assembly.GetManifestResourceStream(resourceName)
                If stream Is Nothing Then
                    Throw New ArgumentException("Embedded resource not found: " & resourceName)
                End If
                Return New Bitmap(stream)
            End Using
        End Function

        Public Function GetEmbeddedSound(resourceName As String) As Stream
            Dim assembly As Assembly = Assembly.GetExecutingAssembly()
            Dim stream As Stream = assembly.GetManifestResourceStream(resourceName)
            If stream Is Nothing Then
                Throw New ArgumentException("Embedded resource not found: " & resourceName)
            End If
            Return stream
        End Function
    End Class
End Namespace