Imports System
Imports System.Drawing

Namespace GameFramework.Graphics
    Public Class Sprite
        Implements IDisposable
        
        Private _texture As Bitmap
        Private _position As PointF
        Private _size As SizeF
        Private _scale As Single
        Private _rotation As Single
        Private _color As Color
        Private _visible As Boolean
        
        Public Property Texture As Bitmap
            Get
                Return _texture
            End Get
            Set(value As Bitmap)
                _texture = value
            End Set
        End Property
        
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
        
        Public Property Scale As Single
            Get
                Return _scale
            End Get
            Set(value As Single)
                _scale = value
            End Set
        End Property
        
        Public Property Rotation As Single
            Get
                Return _rotation
            End Get
            Set(value As Single)
                _rotation = value
            End Set
        End Property
        
        Public Property Color As Color
            Get
                Return _color
            End Get
            Set(value As Color)
                _color = value
            End Set
        End Property
        
        Public Property Visible As Boolean
            Get
                Return _visible
            End Get
            Set(value As Boolean)
                _visible = value
            End Set
        End Property
        
        Public Sub New(Optional texture As Bitmap = Nothing, 
                      Optional position As PointF = Nothing, 
                      Optional size As SizeF = Nothing)
            _texture = texture
            _position = If(position, New PointF(0, 0))
            _size = If(size, If(texture IsNot Nothing, 
                              New SizeF(texture.Width, texture.Height), 
                              New SizeF(32, 32)))
            _scale = 1.0F
            _rotation = 0.0F
            _color = Color.White
            _visible = True
        End Sub
        
        Public Overloads Sub Draw(graphics As Graphics)
            If _visible AndAlso _texture IsNot Nothing Then
                graphics.DrawImage(_texture, _position.X, _position.Y, _size.Width, _size.Height)
            End If
        End Sub
        
        Public Function Contains(point As PointF) As Boolean
            Return New RectangleF(_position, _size).Contains(point)
        End Function
        
#Region "IDisposable Implementation"
        Private disposedValue As Boolean = False
        
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    _texture?.Dispose()
                End If
                disposedValue = True
            End If
        End Sub
        
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace