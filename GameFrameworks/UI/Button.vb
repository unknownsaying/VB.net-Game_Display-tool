Imports System
Imports System.Drawing

Namespace GameFramework.UI
    Public Class Button
        Inherits UIElement

        Private _text As String
        Private _font As Font
        Private _backColor As Color
        Private _foreColor As Color

        Public Event Click As EventHandler

        Public Sub New(bounds As Rectangle, text As String)
            MyBase.New(bounds)
            _text = text
            _font = New Font("Arial", 12)
            _backColor = Color.LightGray
            _foreColor = Color.Black
        End Sub

        Public Overrides Sub Draw(graphics As Graphics)
            Using brush As New SolidBrush(_backColor)
                graphics.FillRectangle(brush, Bounds)
            End Using
            Using pen As New Pen(_foreColor)
                graphics.DrawRectangle(pen, Bounds)
            End Using
            Using brush As New SolidBrush(_foreColor)
                graphics.DrawString(_text, _font, brush, Bounds)
            End Using
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
            ' Update logic for button (e.g., hover effect)
        End Sub

        Public Overridable Sub OnClick()
            RaiseEvent Click(Me, EventArgs.Empty)
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