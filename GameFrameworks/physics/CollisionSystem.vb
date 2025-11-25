Imports System
Imports System.Collections.Generic
Imports GameFramework.Entities

Namespace GameFramework.Physics
    Public Class CollisionSystem
        Public Function CheckCollision(obj1 As GameObject, obj2 As GameObject) As Boolean
            ' Simple AABB collision
            Dim rect1 As New RectangleF(obj1.Position, obj1.Size)
            Dim rect2 As New RectangleF(obj2.Position, obj2.Size)

            Return rect1.IntersectsWith(rect2)
        End Function

        Public Function FindCollisions(entities As List(Of GameObject)) As List(Of Tuple(Of GameObject, GameObject))
            Dim collisions As New List(Of Tuple(Of GameObject, GameObject))()

            For i As Integer = 0 To entities.Count - 1
                For j As Integer = i + 1 To entities.Count - 1
                    If CheckCollision(entities(i), entities(j)) Then
                        collisions.Add(New Tuple(Of GameObject, GameObject)(entities(i), entities(j)))
                    End If
                Next
            Next

            Return collisions
        End Function
    End Class
End Namespace

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports GameFramework.Utilities
Imports GameFramework.Entities

Namespace GameFramework.Physics
    Public Structure CollisionResult
        Public Property IsColliding As Boolean
        Public Property Normal As PointF
        Public Property Depth As Single
        Public Property ContactPoint As PointF
        
        Public Shared ReadOnly Property Empty As CollisionResult
            Get
                Return New CollisionResult With {
                    .IsColliding = False,
                    .Normal = PointF.Empty,
                    .Depth = 0.0F,
                    .ContactPoint = PointF.Empty
                }
            End Get
        End Property
    End Structure
    
    Public Class CollisionSystem
        Private collisionPairs As List(Of Tuple(Of GameObject, GameObject))
        
        Public Sub New()
            collisionPairs = New List(Of Tuple(Of GameObject, GameObject))()
        End Sub
        
        Public Function CheckCollision(obj1 As GameObject, obj2 As GameObject) As CollisionResult
            If obj1 Is Nothing OrElse obj2 Is Nothing Then
                Return CollisionResult.Empty
            End If
            
            Dim rect1 As New RectangleF(obj1.Position, obj1.Size)
            Dim rect2 As New RectangleF(obj2.Position, obj2.Size)
            
            Return RectangleCollision(rect1, rect2)
        End Function
        
        Private Function RectangleCollision(rect1 As RectangleF, rect2 As RectangleF) As CollisionResult
            Dim result As New CollisionResult()
            
            ' Calculate overlap
            Dim overlapX As Single = Math.Min(rect1.Right, rect2.Right) - Math.Max(rect1.Left, rect2.Left)
            Dim overlapY As Single = Math.Min(rect1.Bottom, rect2.Bottom) - Math.Max(rect1.Top, rect2.Top)
            
            If overlapX > 0 AndAlso overlapY > 0 Then
                result.IsColliding = True
                
                ' Determine collision normal and depth
                If overlapX < overlapY Then
                    result.Depth = overlapX
                    result.Normal = New PointF(If(rect1.Center.X < rect2.Center.X, -1.0F, 1.0F), 0.0F)
                Else
                    result.Depth = overlapY
                    result.Normal = New PointF(0.0F, If(rect1.Center.Y < rect2.Center.Y, -1.0F, 1.0F))
                End If
                
                ' Calculate contact point (simplified)
                result.ContactPoint = New PointF(
                    (Math.Max(rect1.Left, rect2.Left) + Math.Min(rect1.Right, rect2.Right)) / 2.0F,
                    (Math.Max(rect1.Top, rect2.Top) + Math.Min(rect1.Bottom, rect2.Bottom)) / 2.0F
                )
            Else
                result = CollisionResult.Empty
            End If
            
            Return result
        End Function
        
        Public Function CheckCollisions(entities As IEnumerable(Of GameObject)) As List(Of Tuple(Of GameObject, GameObject, CollisionResult))
            Dim collisions As New List(Of Tuple(Of GameObject, GameObject, CollisionResult))()
            Dim entityList As List(Of GameObject) = New List(Of GameObject)(entities)
            
            For i As Integer = 0 To entityList.Count - 2
                For j As Integer = i + 1 To entityList.Count - 1
                    Dim obj1 As GameObject = entityList(i)
                    Dim obj2 As GameObject = entityList(j)
                    
                    Dim result As CollisionResult = CheckCollision(obj1, obj2)
                    If result.IsColliding Then
                        collisions.Add(New Tuple(Of GameObject, GameObject, CollisionResult)(obj1, obj2, result))
                    End If
                Next
            Next
            
            Return collisions
        End Function
        
        Public Function Raycast(origin As PointF, direction As PointF, entities As IEnumerable(Of GameObject)) As List(Of Tuple(Of GameObject, PointF, Single))
            Dim hits As New List(Of Tuple(Of GameObject, PointF, Single))()
            Dim normalizedDirection As PointF = MathHelper.Normalize(direction)
            
            For Each entity In entities
                Dim hitResult As Tuple(Of PointF, Single) = RaycastRectangle(origin, normalizedDirection, New RectangleF(entity.Position, entity.Size))
                If hitResult IsNot Nothing Then
                    hits.Add(New Tuple(Of GameObject, PointF, Single)(entity, hitResult.Item1, hitResult.Item2))
                End If
            Next
            
            ' Sort by distance
            hits.Sort(Function(x, y) x.Item3.CompareTo(y.Item3))
            Return hits
        End Function
        
        Private Function RaycastRectangle(origin As PointF, direction As PointF, rect As RectangleF) As Tuple(Of PointF, Single)
            ' Simple ray-rectangle intersection
            Dim tNear As New PointF(
                (rect.Left - origin.X) / direction.X,
                (rect.Top - origin.Y) / direction.Y
            )
            
            Dim tFar As New PointF(
                (rect.Right - origin.X) / direction.X,
                (rect.Bottom - origin.Y) / direction.Y
            )
            
            If Single.IsNaN(tNear.X) OrElse Single.IsNaN(tNear.Y) Then Return Nothing
            
            ' Swap if needed
            If tNear.X > tFar.X Then Dim temp As Single = tNear.X : tNear.X = tFar.X : tFar.X = temp
            If tNear.Y > tFar.Y Then Dim temp As Single = tNear.Y : tNear.Y = tFar.Y : tFar.Y = temp
            
            If tNear.X > tFar.Y OrElse tNear.Y > tFar.X Then Return Nothing
            
            Dim tHitNear As Single = Math.Max(tNear.X, tNear.Y)
            Dim tHitFar As Single = Math.Min(tFar.X, tFar.Y)
            
            If tHitFar < 0 Then Return Nothing
            
            Dim hitPoint As PointF = New PointF(
                origin.X + tHitNear * direction.X,
                origin.Y + tHitNear * direction.Y
            )
            
            Return New Tuple(Of PointF, Single)(hitPoint, tHitNear)
        End Function
    End Class
End Namespace