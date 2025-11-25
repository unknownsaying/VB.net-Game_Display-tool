Imports System
Imports System.Collections.Generic

Namespace GameFramework.Entities
    Public Class EntityManager
        Private entities As List(Of GameObject)
        Private toAdd As List(Of GameObject)
        Private toRemove As List(Of GameObject)

        Public Sub New()
            entities = New List(Of GameObject)()
            toAdd = New List(Of GameObject)()
            toRemove = New List(Of GameObject)()
        End Sub

        Public Sub AddEntity(entity As GameObject)
            toAdd.Add(entity)
        End Sub

        Public Sub RemoveEntity(entity As GameObject)
            toRemove.Add(entity)
        End Sub

        Public Sub Update(deltaTime As Single)
            ' Remove entities
            For Each entity In toRemove
                entities.Remove(entity)
            Next
            toRemove.Clear()

            ' Add new entities
            For Each entity In toAdd
                entities.Add(entity)
            Next
            toAdd.Clear()

            ' Update all entities
            For Each entity In entities
                entity.Update(deltaTime)
            Next
        End Sub

        Public Sub Draw(graphics As Graphics)
            For Each entity In entities
                entity.Draw(graphics)
            Next
        End Sub

        Public Function FindEntitiesByTag(tag As String) As List(Of GameObject)
            Dim found As New List(Of GameObject)()
            For Each entity In entities
                ' If we had a Tag property in GameObject, we could use it.
                ' For now, we'll leave this for the user to implement if needed.
            Next
            Return found
        End Function
    End Class
End Namespace