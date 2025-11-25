Imports System
Imports System.Collections.Generic
Imports System.Drawing

Namespace GameFramework.UI
    Public Class UIManager
        Private uiElements As List(Of UIElement)

        Public Sub New()
            uiElements = New List(Of UIElement)()
        End Sub

        Public Sub AddElement(element As UIElement)
            uiElements.Add(element)
        End Sub

        Public Sub RemoveElement(element As UIElement)
            uiElements.Remove(element)
        End Sub

        Public Sub Update(deltaTime As Single)
            For Each element In uiElements
                element.Update(deltaTime)
            Next
        End Sub

        Public Sub Draw(graphics As Graphics)
            For Each element In uiElements
                element.Draw(graphics)
            Next
        End Sub

        Public Sub HandleMouseClick(position As Point)
            For Each element In uiElements
                If TypeOf element Is Button Then
                    Dim button As Button = CType(element, Button)
                    If button.Bounds.Contains(position) Then
                        button.OnClick()
                    End If
                End If
            Next
        End Sub
    End Class
End Namespace