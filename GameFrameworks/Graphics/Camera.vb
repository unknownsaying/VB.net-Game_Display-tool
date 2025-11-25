Imports System
Imports System.Drawing

Namespace GameFramework.Graphics
    Public Class Camera
        Private _position As PointF
        Private _zoom As Single
        Private _rotation As Single
        Private _viewPortSize As Size

        Public Sub New(viewPortSize As Size)
            _position = New PointF(0, 0)
            _zoom = 1.0F
            _rotation = 0.0F
            _viewPortSize = viewPortSize
        End Sub

        Public Property Position As PointF
            Get
                Return _position
            End Get
            Set(value As PointF)
                _position = value
            End Set
        End Property

        Public Property Zoom As Single
            Get
                Return _zoom
            End Get
            Set(value As Single)
                _zoom = Math.Max(0.1F, value)
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

        Public Property ViewPortSize As Size
            Get
                Return _viewPortSize
            End Get
            Set(value As Size)
                _viewPortSize = value
            End Set
        End Property

        Public Function WorldToScreen(worldPoint As PointF) As PointF
            ' First, subtract the camera position (so we move the world in the opposite direction)
            ' Then, scale by zoom
            ' Then, rotate around the camera center? (We might skip rotation for now)
            ' Then, offset by the viewport center?

            ' For now, let's do a simple version without rotation and with the camera at the center of the viewport.

            Dim screenX As Single = (worldPoint.X - _position.X) * _zoom + _viewPortSize.Width / 2
            Dim screenY As Single = (worldPoint.Y - _position.Y) * _zoom + _viewPortSize.Height / 2

            Return New PointF(screenX, screenY)
        End Function

        Public Function ScreenToWorld(screenPoint As PointF) As PointF
            ' Inverse of WorldToScreen

            Dim worldX As Single = (screenPoint.X - _viewPortSize.Width / 2) / _zoom + _position.X
            Dim worldY As Single = (screenPoint.Y - _viewPortSize.Height / 2) / _zoom + _position.Y

            Return New PointF(worldX, worldY)
        End Function
    End Class
End Namespace

Imports System
Imports System.Drawing
Imports GameFramework.Utilities

Namespace GameFramework.Graphics
    Public Class Camera
        Private positionField As PointF
        Private zoomField As Single
        Private rotationField As Single
        Private viewportSizeField As Size
        Private boundsField As RectangleF?
        Private lerpSpeedField As Single
        Private targetPositionField As PointF?
        
        Public Property Position As PointF
            Get
                Return positionField
            End Get
            Set(value As PointF)
                positionField = If(boundsField.HasValue, ConstrainToBounds(value), value)
            End Set
        End Property
        
        Public Property Zoom As Single
            Get
                Return zoomField
            End Get
            Set(value As Single)
                zoomField = MathHelper.Clamp(value, 0.1F, 5.0F)
            End Set
        End Property
        
        Public Property Rotation As Single
            Get
                Return rotationField
            End Get
            Set(value As Single)
                rotationField = value Mod MathHelper.TwoPI
            End Set
        End Property
        
        Public Property ViewportSize As Size
            Get
                Return viewportSizeField
            End Get
            Set(value As Size)
                viewportSizeField = value
            End Set
        End Property
        
        Public Property Bounds As RectangleF?
            Get
                Return boundsField
            End Get
            Set(value As RectangleF?)
                boundsField = value
                ' Re-constrain position to new bounds
                If value.HasValue Then
                    Position = positionField
                End If
            End Set
        End Property
        
        Public Property LerpSpeed As Single
            Get
                Return lerpSpeedField
            End Get
            Set(value As Single)
                lerpSpeedField = MathHelper.Clamp(value, 0.0F, 1.0F)
            End Set
        End Property
        
        Public Sub New(viewportSize As Size)
            Me.viewportSizeField = viewportSize
            positionField = PointF.Empty
            zoomField = 1.0F
            rotationField = 0.0F
            lerpSpeedField = 0.1F
        End Sub
        
        Public Function WorldToScreen(worldPoint As PointF) As PointF
            ' Translate to camera-relative coordinates
            Dim translated As PointF = New PointF(worldPoint.X - positionField.X,
                                                worldPoint.Y - positionField.Y)
            
            ' Apply rotation
            If Math.Abs(rotationField) > MathHelper.Epsilon Then
                translated = MathHelper.RotatePoint(translated, PointF.Empty, -rotationField)
            End If
            
            ' Apply zoom
            translated = New PointF(translated.X * zoomField, translated.Y * zoomField)
            
            ' Translate to screen center
            translated = New PointF(translated.X + viewportSizeField.Width / 2,
                                  translated.Y + viewportSizeField.Height / 2)
            
            Return translated
        End Function
        
        Public Function ScreenToWorld(screenPoint As PointF) As PointF
            ' Translate from screen center
            Dim translated As PointF = New PointF(screenPoint.X - viewportSizeField.Width / 2,
                                                screenPoint.Y - viewportSizeField.Height / 2)
            
            ' Apply inverse zoom
            translated = New PointF(translated.X / zoomField, translated.Y / zoomField)
            
            ' Apply inverse rotation
            If Math.Abs(rotationField) > MathHelper.Epsilon Then
                translated = MathHelper.RotatePoint(translated, PointF.Empty, rotationField)
            End If
            
            ' Translate to world coordinates
            translated = New PointF(translated.X + positionField.X,
                                  translated.Y + positionField.Y)
            
            Return translated
        End Function
        
        Public Function GetViewMatrix() As Drawing2D.Matrix
            Dim matrix As New Drawing2D.Matrix()
            
            ' Translate to screen center
            matrix.Translate(viewportSizeField.Width / 2, viewportSizeField.Height / 2)
            
            ' Apply zoom
            matrix.Scale(zoomField, zoomField)
            
            ' Apply rotation
            matrix.Rotate(MathHelper.ToDegrees(rotationField))
            
            ' Translate by camera position (inverse)
            matrix.Translate(-positionField.X, -positionField.Y)
            
            Return matrix
        End Function
        
        Public Sub LookAt(target As PointF)
            If lerpSpeedField > 0 Then
                targetPositionField = target
            Else
                Position = New PointF(target.X - viewportSizeField.Width / (2 * zoomField),
                                    target.Y - viewportSizeField.Height / (2 * zoomField))
            End If
        End Sub
        
        Public Sub Update(deltaTime As Single)
            If targetPositionField.HasValue Then
                Dim target As PointF = targetPositionField.Value
                target = New PointF(target.X - viewportSizeField.Width / (2 * zoomField),
                                  target.Y - viewportSizeField.Height / (2 * zoomField))
                
                Position = MathHelper.Lerp(positionField, target, lerpSpeedField)
                
                ' Check if close enough to target
                If MathHelper.Distance(positionField, target) < 1.0F Then
                    targetPositionField = Nothing
                End If
            End If
        End Sub
        
        Public Function IsInView(worldBounds As RectangleF) As Boolean
            Dim screenBounds As RectangleF = GetScreenViewBounds()
            Return screenBounds.IntersectsWith(worldBounds)
        End Function
        
        Public Function GetScreenViewBounds() As RectangleF
            Dim topLeft As PointF = ScreenToWorld(PointF.Empty)
            Dim bottomRight As PointF = ScreenToWorld(New PointF(viewportSizeField.Width, viewportSizeField.Height))
            Return New RectangleF(topLeft.X, topLeft.Y,
                                bottomRight.X - topLeft.X,
                                bottomRight.Y - topLeft.Y)
        End Function
        
        Private Function ConstrainToBounds(position As PointF) As PointF
            If Not boundsField.HasValue Then Return position
            
            Dim bounds As RectangleF = boundsField.Value
            Dim halfViewport As SizeF = New SizeF(viewportSizeField.Width / (2 * zoomField),
                                                viewportSizeField.Height / (2 * zoomField))
            
            Dim constrainedX As Single = MathHelper.Clamp(position.X,
                                                        bounds.Left + halfViewport.Width,
                                                        bounds.Right - halfViewport.Width)
            
            Dim constrainedY As Single = MathHelper.Clamp(position.Y,
                                                        bounds.Top + halfViewport.Height,
                                                        bounds.Bottom - halfViewport.Height)
            
            Return New PointF(constrainedX, constrainedY)
        End Function
        
        Public Sub ZoomAt(point As PointF, zoomDelta As Single)
            Dim oldZoom As Single = zoomField
            Zoom += zoomDelta
            
            ' Adjust position to zoom at point
            Dim worldPoint As PointF = ScreenToWorld(point)
            Dim zoomFactor As Single = zoomField / oldZoom
            
            Position = New PointF(worldPoint.X - (worldPoint.X - positionField.X) / zoomFactor,
                                worldPoint.Y - (worldPoint.Y - positionField.Y) / zoomFactor)
        End Sub
    End Class
End Namespace