Imports System
Imports System.Collections.Generic
Imports GameFramework.Entities

Namespace GameFramework.Physics
    Public Class PhysicsEngine
        Private collisionSystem As CollisionSystem

        Public Sub New()
            collisionSystem = New CollisionSystem()
        End Sub

        Public Sub Update(entities As List(Of GameObject), deltaTime As Single)
            ' Update positions based on velocities
            For Each entity In entities
                entity.Position = New PointF(
                    entity.Position.X + entity.Velocity.X * deltaTime,
                    entity.Position.Y + entity.Velocity.Y * deltaTime
                )
            Next

            ' Check for collisions
            Dim collisions = collisionSystem.FindCollisions(entities)
            For Each collision In collisions
                ' Handle collision (e.g., bounce, destroy, etc.)
                ' This is a simple example: stop both entities
                collision.Item1.Velocity = New PointF(0, 0)
                collision.Item2.Velocity = New PointF(0, 0)
            Next
        End Sub
    End Class
End Namespace

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports GameFramework.Utilities
Imports GameFramework.Entities

Namespace GameFramework.Physics
    Public Class PhysicsEngine
        Private gravityField As PointF
        Private collisionSystemField As CollisionSystem
        Private physicsObjects As List(Of PhysicsObject)
        Private fixedTimeStepField As Single
        
        Public Property Gravity As PointF
            Get
                Return gravityField
            End Get
            Set(value As PointF)
                gravityField = value
            End Set
        End Property
        
        Public Property FixedTimeStep As Single
            Get
                Return fixedTimeStepField
            End Get
            Set(value As Single)
                fixedTimeStepField = Math.Max(0.001F, value)
            End Set
        End Property
        
        Public ReadOnly Property CollisionSystem As CollisionSystem
            Get
                Return collisionSystemField
            End Get
        End Property
        
        Public Event CollisionDetected As EventHandler(Of CollisionEventArgs)
        
        Public Sub New()
            gravityField = New PointF(0, 98.1F) ' Default gravity
            collisionSystemField = New CollisionSystem()
            physicsObjects = New List(Of PhysicsObject)()
            fixedTimeStepField = 1.0F / 60.0F ' 60 FPS
        End Sub
        
        Public Sub AddObject(obj As PhysicsObject)
            If Not physicsObjects.Contains(obj) Then
                physicsObjects.Add(obj)
            End If
        End Sub
        
        Public Sub RemoveObject(obj As PhysicsObject)
            physicsObjects.Remove(obj)
        End Sub
        
        Public Sub Update(deltaTime As Single)
            ' Fixed timestep accumulation
            Static accumulatedTime As Single = 0.0F
            accumulatedTime += deltaTime
            
            While accumulatedTime >= fixedTimeStepField
                UpdatePhysics(fixedTimeStepField)
                accumulatedTime -= fixedTimeStepField
            End While
        End Sub
        
        Private Sub UpdatePhysics(deltaTime As Single)
            ' Update velocities with gravity
            For Each obj In physicsObjects
                If obj.IsKinematic Then Continue For
                
                ' Apply gravity
                obj.Velocity = New PointF(
                    obj.Velocity.X + gravityField.X * deltaTime,
                    obj.Velocity.Y + gravityField.Y * deltaTime
                )
                
                ' Apply damping
                obj.Velocity = New PointF(
                    obj.Velocity.X * (1.0F - obj.LinearDamping),
                    obj.Velocity.Y * (1.0F - obj.LinearDamping)
                )
                
                ' Update position
                obj.Position = New PointF(
                    obj.Position.X + obj.Velocity.X * deltaTime,
                    obj.Position.Y + obj.Velocity.Y * deltaTime
                )
            Next
            
            ' Check collisions
            Dim collisions = collisionSystemField.CheckCollisions(physicsObjects)
            For Each collision In collisions
                HandleCollision(collision.Item1, collision.Item2, collision.Item3)
                RaiseEvent CollisionDetected(Me, New CollisionEventArgs(collision.Item1, collision.Item2, collision.Item3))
            Next
        End Sub
        
        Private Sub HandleCollision(obj1 As GameObject, obj2 As GameObject, result As CollisionResult)
            Dim physObj1 As PhysicsObject = TryCast(obj1, PhysicsObject)
            Dim physObj2 As PhysicsObject = TryCast(obj2, PhysicsObject)
            
            If physObj1 Is Nothing OrElse physObj2 Is Nothing Then Return
            
            ' Simple collision response
            If Not physObj1.IsKinematic Then
                physObj1.Position = New PointF(
                    physObj1.Position.X - result.Normal.X * result.Depth * 0.5F,
                    physObj1.Position.Y - result.Normal.Y * result.Depth * 0.5F
                )
                
                ' Reflect velocity
                Dim dotProduct As Single = MathHelper.DotProduct(physObj1.Velocity, result.Normal)
                physObj1.Velocity = New PointF(
                    physObj1.Velocity.X - 2 * dotProduct * result.Normal.X * physObj1.Bounciness,
                    physObj1.Velocity.Y - 2 * dotProduct * result.Normal.Y * physObj1.Bounciness
                )
            End If
            
            If Not physObj2.IsKinematic Then
                physObj2.Position = New PointF(
                    physObj2.Position.X + result.Normal.X * result.Depth * 0.5F,
                    physObj2.Position.Y + result.Normal.Y * result.Depth * 0.5F
                )
                
                ' Reflect velocity
                Dim dotProduct As Single = MathHelper.DotProduct(physObj2.Velocity, result.Normal)
                physObj2.Velocity = New PointF(
                    physObj2.Velocity.X - 2 * dotProduct * result.Normal.X * physObj2.Bounciness,
                    physObj2.Velocity.Y - 2 * dotProduct * result.Normal.Y * physObj2.Bounciness
                )
            End If
        End Sub
        
        Public Function Raycast(origin As PointF, direction As PointF) As List(Of Tuple(Of PhysicsObject, PointF, Single))
            Return collisionSystemField.Raycast(origin, direction, physicsObjects)
        End Function
        
        Public Sub Clear()
            physicsObjects.Clear()
        End Sub
    End Class
    
    Public Class CollisionEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property Object1 As GameObject
        Public ReadOnly Property Object2 As GameObject
        Public ReadOnly Property CollisionResult As CollisionResult
        
        Public Sub New(obj1 As GameObject, obj2 As GameObject, result As CollisionResult)
            Me.Object1 = obj1
            Me.Object2 = obj2
            Me.CollisionResult = result
        End Sub
    End Class
End Namespace