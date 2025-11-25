Imports System
Imports System.Windows.Forms
Imports System.Collections.Generic

Namespace GameFramework.Input
    Public Class InputHandler
        Private keyStates As Dictionary(Of Keys, Boolean)
        Private previousKeyStates As Dictionary(Of Keys, Boolean)
        Private mousePosition As Point
        Private mouseButtons As MouseButtons
        Private previousMouseButtons As MouseButtons
        
        Public Event KeyPressed As EventHandler(Of KeyEventArgs)
        Public Event KeyReleased As EventHandler(Of KeyEventArgs)
        Public Event MouseMoved As EventHandler(Of MouseEventArgs)
        Public Event MouseClicked As EventHandler(Of MouseEventArgs)
        
        Public Sub New(window As GameWindow)
            keyStates = New Dictionary(Of Keys, Boolean)()
            previousKeyStates = New Dictionary(Of Keys, Boolean)()
            
            AddHandler window.KeyDown, AddressOf OnKeyDown
            AddHandler window.KeyUp, AddressOf OnKeyUp
            AddHandler window.MouseMove, AddressOf OnMouseMove
            AddHandler window.MouseDown, AddressOf OnMouseDown
            AddHandler window.MouseUp, AddressOf OnMouseUp
        End Sub
        
        Public Sub Update()
            ' Copy current states to previous states
            previousKeyStates = New Dictionary(Of Keys, Boolean)(keyStates)
            previousMouseButtons = mouseButtons
        End Sub
        
        Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
            keyStates(e.KeyCode) = True
            RaiseEvent KeyPressed(Me, e)
        End Sub
        
        Private Sub OnKeyUp(sender As Object, e As KeyEventArgs)
            keyStates(e.KeyCode) = False
            RaiseEvent KeyReleased(Me, e)
        End Sub
        
        Private Sub OnMouseMove(sender As Object, e As MouseEventArgs)
            mousePosition = e.Location
            RaiseEvent MouseMoved(Me, e)
        End Sub
        
        Private Sub OnMouseDown(sender As Object, e As MouseEventArgs)
            mouseButtons = e.Button
            RaiseEvent MouseClicked(Me, e)
        End Sub
        
        Private Sub OnMouseUp(sender As Object, e As MouseEventArgs)
            mouseButtons = MouseButtons.None
        End Sub
        
        Public Function IsKeyDown(key As Keys) As Boolean
            Return keyStates.ContainsKey(key) AndAlso keyStates(key)
        End Function
        
        Public Function IsKeyPressed(key As Keys) As Boolean
            Return IsKeyDown(key) AndAlso 
                   (Not previousKeyStates.ContainsKey(key) OrElse Not previousKeyStates(key))
        End Function
        
        Public Function IsKeyReleased(key As Keys) As Boolean
            Return Not IsKeyDown(key) AndAlso 
                   (previousKeyStates.ContainsKey(key) AndAlso previousKeyStates(key))
        End Function
        
        Public ReadOnly Property MousePosition As Point
            Get
                Return mousePosition
            End Get
        End Property
        
        Public Function IsMouseButtonDown(button As MouseButtons) As Boolean
            Return (mouseButtons And button) = button
        End Function
        
        Public Function IsMouseButtonPressed(button As MouseButtons) As Boolean
            Return IsMouseButtonDown(button) AndAlso (previousMouseButtons And button) <> button
        End Function
        
        Public Sub ProcessKeyDown(e As KeyEventArgs)
            OnKeyDown(Me, e)
        End Sub
    End Class
End Namespace