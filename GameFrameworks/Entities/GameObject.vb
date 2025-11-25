Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports GameFramework.Graphics

Namespace GameFramework.Entities
    Public Class GameObject
        Private _position As PointF
        Private _size As SizeF
        Private _velocity As PointF
        Private _components As List(Of Component)

        Public Sub New()
            _components = New List(Of Component)()
            _position = New PointF(0, 0)
            _size = New SizeF(32, 32)
            _velocity = New PointF(0, 0)
        End Sub

        Public Overridable Sub Update(deltaTime As Single)
            ' Update position based on velocity
            _position.X += _velocity.X * deltaTime
            _position.Y += _velocity.Y * deltaTime

            ' Update all components
            For Each component In _components
                component.Update(deltaTime)
            Next
        End Sub

        Public Overridable Sub Draw(graphics As Graphics)
            ' Draw the game object (override in derived classes)
        End Sub

        Public Sub AddComponent(component As Component)
            _components.Add(component)
            component.GameObject = Me
        End Sub

        Public Function GetComponent(Of T As Component)() As T
            For Each component In _components
                If TypeOf component Is T Then
                    Return CType(component, T)
                End If
            Next
            Return Nothing
        End Function

        Public Property Position As PointF
            Get
                Return _position
            End Get
            Set(value As PointF)
                _position = value
            End Set
        End Property

        Public Property Size As SizeF
            Get
                Return _size
            End Get
            Set(value As SizeF)
                _size = value
            End Set
        End Property

        Public Property Velocity As PointF
            Get
                Return _velocity
            End Get
            Set(value As PointF)
                _velocity = value
            End Set
        End Property
    End Class
End Namespace