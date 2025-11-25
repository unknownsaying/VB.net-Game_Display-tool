Imports System

Namespace GameFramework.Utilities
    Public Class MathHelper
        Public Shared Function Clamp(value As Single, min As Single, max As Single) As Single
            Return Math.Max(min, Math.Min(max, value))
        End Function

        Public Shared Function Lerp(start As Single, [end] As Single, amount As Single) As Single
            Return start + ([end] - start) * amount
        End Function

        Public Shared Function ToRadians(degrees As Single) As Single
            Return degrees * CSng(Math.PI / 180.0)
        End Function

        Public Shared Function ToDegrees(radians As Single) As Single
            Return radians * CSng(180.0 / Math.PI)
        End Function
    End Class
End Namespace

Imports System
Imports System.Collections.Generic

Namespace GameFramework.Utilities
    Public Module MathHelper
        Public ReadOnly PI As Single = CSng(Math.PI)
        Public ReadOnly TwoPI As Single = CSng(2 * Math.PI)
        Public ReadOnly Epsilon As Single = 0.00001F
        
        Public Function Clamp(value As Single, min As Single, max As Single) As Single
            Return If(value < min, min, If(value > max, max, value))
        End Function
        
        Public Function Clamp(value As Integer, min As Integer, max As Integer) As Integer
            Return If(value < min, min, If(value > max, max, value))
        End Function
        
        Public Function Lerp(start As Single, [end] As Single, amount As Single) As Single
            Return start + ([end] - start) * Clamp(amount, 0.0F, 1.0F)
        End Function
        
        Public Function SmoothStep(value As Single, edge0 As Single, edge1 As Single) As Single
            Dim x As Single = Clamp((value - edge0) / (edge1 - edge0), 0.0F, 1.0F)
            Return x * x * (3.0F - 2.0F * x)
        End Function
        
        Public Function ToRadians(degrees As Single) As Single
            Return degrees * PI / 180.0F
        End Function
        
        Public Function ToDegrees(radians As Single) As Single
            Return radians * 180.0F / PI
        End Function
        
        Public Function Distance(point1 As PointF, point2 As PointF) As Single
            Dim dx As Single = point2.X - point1.X
            Dim dy As Single = point2.Y - point1.Y
            Return CSng(Math.Sqrt(dx * dx + dy * dy))
        End Function
        
        Public Function DistanceSquared(point1 As PointF, point2 As PointF) As Single
            Dim dx As Single = point2.X - point1.X
            Dim dy As Single = point2.Y - point1.Y
            Return dx * dx + dy * dy
        End Function
        
        Public Function VectorLength(vector As PointF) As Single
            Return CSng(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y))
        End Function
        
        Public Function Normalize(vector As PointF) As PointF
            Dim length As Single = VectorLength(vector)
            If length > Epsilon Then
                Return New PointF(vector.X / length, vector.Y / length)
            Else
                Return PointF.Empty
            End If
        End Function
        
        Public Function DotProduct(vector1 As PointF, vector2 As PointF) As Single
            Return vector1.X * vector2.X + vector1.Y * vector2.Y
        End Function
        
        Public Function CrossProduct(vector1 As PointF, vector2 As PointF) As Single
            Return vector1.X * vector2.Y - vector1.Y * vector2.X
        End Function
        
        Public Function RotatePoint(point As PointF, origin As PointF, angle As Single) As PointF
            Dim cosTheta As Single = CSng(Math.Cos(angle))
            Dim sinTheta As Single = CSng(Math.Sin(angle))
            
            Dim translatedX As Single = point.X - origin.X
            Dim translatedY As Single = point.Y - origin.Y
            
            Dim rotatedX As Single = translatedX * cosTheta - translatedY * sinTheta
            Dim rotatedY As Single = translatedX * sinTheta + translatedY * cosTheta
            
            Return New PointF(rotatedX + origin.X, rotatedY + origin.Y)
        End Function
        
        Public Function CreateBoundingBox(position As PointF, size As SizeF) As RectangleF
            Return New RectangleF(position, size)
        End Function
        
        Public Function Intersects(rect1 As RectangleF, rect2 As RectangleF) As Boolean
            Return rect1.Left < rect2.Right AndAlso
                   rect1.Right > rect2.Left AndAlso
                   rect1.Top < rect2.Bottom AndAlso
                   rect1.Bottom > rect2.Top
        End Function
        
        Public Function Contains(container As RectangleF, point As PointF) As Boolean
            Return point.X >= container.Left AndAlso
                   point.X <= container.Right AndAlso
                   point.Y >= container.Top AndAlso
                   point.Y <= container.Bottom
        End Function
    End Module
    
    Public Class Randomizer
        Private random As Random
        Private seedValue As Integer
        
        Public Sub New(Optional seed As Integer = -1)
            If seed = -1 Then
                seed = Environment.TickCount
            End If
            seedValue = seed
            random = New Random(seed)
        End Sub
        
        Public Function [Next]() As Integer
            Return random.Next()
        End Function
        
        Public Function [Next](maxValue As Integer) As Integer
            Return random.Next(maxValue)
        End Function
        
        Public Function [Next](minValue As Integer, maxValue As Integer) As Integer
            Return random.Next(minValue, maxValue)
        End Function
        
        Public Function NextSingle() As Single
            Return CSng(random.NextDouble())
        End Function
        
        Public Function NextSingle(minValue As Single, maxValue As Single) As Single
            Return minValue + CSng(random.NextDouble()) * (maxValue - minValue)
        End Function
        
        Public Function NextPoint(area As RectangleF) As PointF
            Return New PointF(NextSingle(area.Left, area.Right),
                            NextSingle(area.Top, area.Bottom))
        End Function
        
        Public Function NextColor() As Color
            Return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))
        End Function
        
        Public Function NextItem(Of T)(items As IList(Of T)) As T
            If items Is Nothing OrElse items.Count = 0 Then
                Throw New ArgumentException("Item list cannot be null or empty")
            End If
            Return items(random.Next(items.Count))
        End Function
        
        Public ReadOnly Property Seed As Integer
            Get
                Return seedValue
            End Get
        End Property
        
        Public Sub Reset()
            random = New Random(seedValue)
        End Sub
    End Class
End Namespace