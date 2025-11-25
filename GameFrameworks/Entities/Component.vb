Imports System

Namespace GameFramework.Entities
    Public MustInherit Class Component
        Private _gameObject As GameObject

        Public Property GameObject As GameObject
            Get
                Return _gameObject
            End Get
            Set(value As GameObject)
                _gameObject = value
            End Set
        End Property

        Public Overridable Sub Update(deltaTime As Single)
            ' Override in derived classes
        End Sub
    End Class
End Namespace