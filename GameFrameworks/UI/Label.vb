Imports System
Imports System.Drawing

Namespace GameFramework.UI
    Public Class Label
        Inherits UIElement

        Private _text As String
        Private _font As Font
        Private _foreColor As Color

        Public Sub New(bounds As Rectangle, text As String)
            MyBase.New(bounds)
            _text = text
            _font = New Font("Arial", 12)
            _foreColor = Color.Black
        End Sub

        Public Overrides Sub Draw(graphics As Graphics)
            Using brush As New SolidBrush(_foreColor)
                graphics.DrawString(_text, _font, brush, Bounds)
            End Using
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
            ' Label doesn't need update by default
        End Sub

        Public Property Text As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value
            End Set
        End Property
    End Class
End Namespace