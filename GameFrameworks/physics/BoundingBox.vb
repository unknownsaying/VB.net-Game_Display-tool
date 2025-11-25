Imports System
Imports System.Drawing

Namespace GameFramework.Physics
    Public Class BoundingBox
        Private _position As PointF
        Private _size As SizeF

        Public Sub New(position As PointF, size As SizeF)
            _position = position
            _size = size
        End Sub

        Public Function IntersectsWith(other As BoundingBox) As Boolean
            Dim rect1 As New RectangleF(_position, _size)
            Dim rect2 As New RectangleF(other._position, other._size)

            Return rect1.IntersectsWith(rect2)
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
    End Class
End Namespace

Imports System
Imports System.Drawing
Imports GameFramework.Utilities

Namespace GameFramework.Physics
    Public Class BoundingBox
        Private minField As PointF
        Private maxField As PointF
        
        Public Property Min As PointF
            Get
                Return minField
            End Get
            Set(value As PointF)
                minField = value
                UpdateProperties()
            End Set
        End Property
        
        Public Property Max As PointF
            Get
                Return maxField
            End Get
            Set(value As PointF)
                maxField = value
                UpdateProperties()
            End Set
        End Property
        
        Public ReadOnly Property Center As PointF
        Public ReadOnly Property Size As SizeF
        Public ReadOnly Property Width As Single
        Public ReadOnly Property Height As Single
        
        Public Sub New(min As PointF, max As PointF)
            Me.minField = min
            Me.maxField = max
            UpdateProperties()
        End Sub
        
        Public Sub New(rect As RectangleF)
            minField = New PointF(rect.Left, rect.Top)
            maxField = New PointF(rect.Right, rect.Bottom)
            UpdateProperties()
        End Sub
        
        Public Sub New(position As PointF, size As SizeF)
            minField = position
            maxField = New PointF(position.X + size.Width, position.Y + size.Height)
            UpdateProperties()
        End Sub
        
        Private Sub UpdateProperties()
            Center = New PointF((minField.X + maxField.X) / 2.0F, (minField.Y + maxField.Y) / 2.0F)
            Width = maxField.X - minField.X
            Height = maxField.Y - minField.Y
            Size = New SizeF(Width, Height)
        End Sub
        
        Public Function Contains(point As PointF) As Boolean
            Return point.X >= minField.X AndAlso point.X <= maxField.X AndAlso
                   point.Y >= minField.Y AndAlso point.Y <= maxField.Y
        End Function
        
        Public Function Intersects(other As BoundingBox) As Boolean
            Return minField.X <= other.maxField.X AndAlso maxField.X >= other.minField.X AndAlso
                   minField.Y <= other.maxField.Y AndAlso maxField.Y >= other.minField.Y
        End Function
        
        Public Function GetIntersectionDepth(other As BoundingBox) As PointF
            Dim depth As New PointF(0, 0)
            
            If Intersects(other) Then
                ' Calculate horizontal and vertical overlap
                Dim horizontalOverlap As Single = Math.Min(maxField.X - other.minField.X, other.maxField.X - minField.X)
                Dim verticalOverlap As Single = Math.Min(maxField.Y - other.minField.Y, other.maxField.Y - minField.Y)
                
                ' Choose the minimum overlap direction
                If horizontalOverlap < verticalOverlap Then
                    depth.X = If(Center.X < other.Center.X, -horizontalOverlap, horizontalOverlap)
                Else
                    depth.Y = If(Center.Y < other.Center.Y, -verticalOverlap, verticalOverlap)
                End If
            End If
            
            Return depth
        End Function
        
        Public Function ToRectangleF() As RectangleF
            Return New RectangleF(minField, Size)
        End Function
        
        Public Sub Inflate(horizontalAmount As Single, verticalAmount As Single)
            minField = New PointF(minField.X - horizontalAmount, minField.Y - verticalAmount)
            maxField = New PointF(maxField.X + horizontalAmount, maxField.Y + verticalAmount)
            UpdateProperties()
        End Sub
        
        Public Sub Offset(offset As PointF)
            minField = New PointF(minField.X + offset.X, minField.Y + offset.Y)
            maxField = New PointF(maxField.X + offset.X, maxField.Y + offset.Y)
            UpdateProperties()
        End Sub
        
        Public Function Union(other As BoundingBox) As BoundingBox
            Return New BoundingBox(
                New PointF(Math.Min(minField.X, other.minField.X), Math.Min(minField.Y, other.minField.Y)),
                New PointF(Math.Max(maxField.X, other.maxField.X), Math.Max(maxField.Y, other.maxField.Y))
            )
        End Function
        
        Public Shared Function CreateFromPoints(points As IEnumerable(Of PointF)) As BoundingBox
            If points Is Nothing Then Throw New ArgumentNullException(NameOf(points))
            
            Dim enumerator As IEnumerator(Of PointF) = points.GetEnumerator()
            If Not enumerator.MoveNext() Then
                Return New BoundingBox(PointF.Empty, PointF.Empty)
            End If
            
            Dim min As PointF = enumerator.Current
            Dim max As PointF = enumerator.Current
            
            While enumerator.MoveNext()
                min = New PointF(Math.Min(min.X, enumerator.Current.X), Math.Min(min.Y, enumerator.Current.Y))
                max = New PointF(Math.Max(max.X, enumerator.Current.X), Math.Max(max.Y, enumerator.Current.Y))
            End While
            
            Return New BoundingBox(min, max)
        End Function
        
        Public Overrides Function ToString() As String
            Return $"BoundingBox(Min: {minField}, Max: {maxField}, Size: {Size})"
        End Function
    End Class
End Namespace