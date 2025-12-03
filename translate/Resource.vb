Public Class ResourceManager
    Private images As New Dictionary(Of String, Bitmap)()
    Private fonts As New Dictionary(Of String, Font)()
    
    Public Sub LoadImage(name As String, width As Integer, height As Integer)
        ' Create placeholder graphics - in real implementation, load from embedded resources
        Dim bmp As New Bitmap(width, height)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.LightGray)
            g.DrawRectangle(Pens.Black, 0, 0, width - 1, height - 1)
        End Using
        images(name) = bmp
    End Sub
    
    Public Function GetImage(name As String) As Bitmap
        Return If(images.ContainsKey(name), images(name), Nothing)
    End Function
    
    Public Sub LoadFont(name As String, fontFamily As String, size As Single)
        fonts(name) = New Font(fontFamily, size)
    End Sub
    
    Public Function GetFont(name As String) As Font
        Return If(fonts.ContainsKey(name), fonts(name), SystemFonts.DefaultFont)
    End Function

End Class
