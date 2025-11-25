Public Class InputManager
    Private keyStates As New Dictionary(Of Keys, Boolean)()
    Private mousePosition As Point
    Private mouseButtons As MouseButtons
    
    Public Sub New(parentForm As Form)
        AddHandler parentForm.KeyDown, AddressOf OnKeyDown
        AddHandler parentForm.KeyUp, AddressOf OnKeyUp
        AddHandler parentForm.MouseMove, AddressOf OnMouseMove
        AddHandler parentForm.MouseDown, AddressOf OnMouseDown
        AddHandler parentForm.MouseUp, AddressOf OnMouseUp
        
        ' Enable keyboard input
        parentForm.KeyPreview = True
    End Sub
    
    Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
        keyStates(e.KeyCode) = True
    End Sub
    
    Private Sub OnKeyUp(sender As Object, e As KeyEventArgs)
        keyStates(e.KeyCode) = False
    End Sub
    
    Public Function IsKeyDown(key As Keys) As Boolean
        Return keyStates.ContainsKey(key) AndAlso keyStates(key)
    End Function
    
    Public Function IsKeyPressed(key As Keys) As Boolean
        ' Implementation for key press detection
        Return IsKeyDown(key)
    End Function
    
    Private Sub OnMouseMove(sender As Object, e As MouseEventArgs)
        mousePosition = e.Location
    End Sub
    
    Private Sub OnMouseDown(sender As Object, e As MouseEventArgs)
        mouseButtons = e.Button
    End Sub
    
    Private Sub OnMouseUp(sender As Object, e As MouseEventArgs)
        mouseButtons = MouseButtons.None
    End Sub
    
    Public ReadOnly Property MousePos As Point
        Get
            Return mousePosition
        End Get
    End Property
    
    Public ReadOnly Property MouseButton As MouseButtons
        Get
            Return mouseButtons
        End Get
    End Property
End Class