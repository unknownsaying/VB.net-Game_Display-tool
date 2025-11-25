Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Collections.Generic

Namespace GameFramework.Graphics
    Public Class Renderer
        Implements IDisposable
        
        Private gameWindow As GameWindow
        Private backBuffer As Bitmap
        Private graphicsContext As Graphics
        Private spriteBatch As List(Of Sprite)
        Private renderStates As Stack(Of GraphicsState)
        
        Public ReadOnly Property BackBuffer As Bitmap
            Get
                Return backBuffer
            End Get
        End Property
        
        Public ReadOnly Property Graphics As Graphics
            Get
                Return graphicsContext
            End Get
        End Property
        
        Public Sub New(window As GameWindow)
            gameWindow = window
            InitializeBackBuffer()
            spriteBatch = New List(Of Sprite)()
            renderStates = New Stack(Of GraphicsState)()
        End Sub
        
        Private Sub InitializeBackBuffer()
            backBuffer = New Bitmap(gameWindow.ClientSize.Width, 
                                  gameWindow.ClientSize.Height, 
                                  PixelFormat.Format32bppArgb)
            graphicsContext = Graphics.FromImage(backBuffer)
            graphicsContext.SmoothingMode = SmoothingMode.AntiAlias
            graphicsContext.InterpolationMode = InterpolationMode.NearestNeighbor
        End Sub
        
        Public Sub BeginDraw()
            graphicsContext.Clear(Color.CornflowerBlue)
            SaveState()
        End Sub
        
        Public Sub EndDraw()
            RestoreState()
        End Sub
        
        Public Sub SaveState()
            renderStates.Push(graphicsContext.Save())
        End Sub
        
        Public Sub RestoreState()
            If renderStates.Count > 0 Then
                graphicsContext.Restore(renderStates.Pop())
            End If
        End Sub
        
        Public Sub DrawSprite(sprite As Sprite)
            If sprite.Texture IsNot Nothing Then
                graphicsContext.DrawImage(sprite.Texture, 
                                        sprite.Position.X, 
                                        sprite.Position.Y, 
                                        sprite.Size.Width, 
                                        sprite.Size.Height)
            End If
        End Sub
        
        Public Sub DrawRectangle(rect As RectangleF, color As Color, Optional filled As Boolean = True)
            Using pen As New Pen(color), brush As New SolidBrush(color)
                If filled Then
                    graphicsContext.FillRectangle(brush, rect)
                Else
                    graphicsContext.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
                End If
            End Using
        End Sub
        
        Public Sub DrawText(text As String, position As PointF, font As Font, color As Color)
            Using brush As New SolidBrush(color)
                graphicsContext.DrawString(text, font, brush, position)
            End Using
        End Sub
        
        Public Sub ResizeBackBuffer(newSize As Size)
            If backBuffer IsNot Nothing Then
                backBuffer.Dispose()
            End If
            If graphicsContext IsNot Nothing Then
                graphicsContext.Dispose()
            End If
            
            backBuffer = New Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppArgb)
            graphicsContext = Graphics.FromImage(backBuffer)
        End Sub
        
#Region "IDisposable Implementation"
        Private disposedValue As Boolean = False
        
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    backBuffer?.Dispose()
                    graphicsContext?.Dispose()
                    For Each sprite In spriteBatch
                        sprite.Dispose()
                    Next
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