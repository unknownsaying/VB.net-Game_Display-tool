Imports System
Imports System.Drawing

Namespace GameFramework.UI
    Public MustInherit Class UIElement
        Private _bounds As Rectangle

        Public Sub New(bounds As Rectangle)
            _bounds = bounds
        End Sub

        Public MustOverride Sub Draw(graphics As Graphics)
        Public MustOverride Sub Update(deltaTime As Single)

        Public Property Bounds As Rectangle
            Get
                Return _bounds
            End Get
            Set(value As Rectangle)
                _bounds = value
            End Set
        End Property
    End Class
End Namespace