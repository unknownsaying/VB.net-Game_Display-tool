Imports System
Imports System.Drawing
Imports GameFramework.Core
Imports GameFramework.Graphics
Imports GameFramework.Input

Public Class SampleGame
    Inherits GameApplication
    
    Private playerSprite As Sprite
    Private enemySprites As List(Of Sprite)
    Private gameFont As Font
    Private score As Integer
    Private gameOver As Boolean
    
    Public Sub New()
        MyBase.New()
        Window.Text = "Advanced VB.NET Game Framework"
    End Sub
    
    Protected Overrides Sub Initialize()
        ' Load resources
        gameFont = New Font("Arial", 16, FontStyle.Bold)
        
        ' Create player sprite
        playerSprite = New Sprite()
        playerSprite.Position = New PointF(400, 300)
        playerSprite.Size = New SizeF(50, 50)
        playerSprite.Color = Color.Blue
        
        ' Create enemies
        enemySprites = New List(Of Sprite)()
        For i As Integer = 0 To 4
            Dim enemy As New Sprite()
            enemy.Position = New PointF(100 + i * 150, 100)
            enemy.Size = New SizeF(40, 40)
            enemy.Color = Color.Red
            enemySprites.Add(enemy)
        Next
        
        score = 0
        gameOver = False
        
        AddHandler Input.KeyPressed, AddressOf OnKeyPressed
        AddHandler Input.MouseClicked, AddressOf OnMouseClicked
    End Sub
    
    Protected Overrides Sub Update(gameTime As GameTime)
        If gameOver Then Return
        
        UpdatePlayerMovement()
        UpdateGameLogic()
        CheckCollisions()
    End Sub
    
    Protected Overrides Sub Draw(gameTime As GameTime)
        Graphics.BeginDraw()
        
        ' Draw player
        Graphics.DrawRectangle(New RectangleF(playerSprite.Position, playerSprite.Size), 
                             playerSprite.Color, True)
        
        ' Draw enemies
        For Each enemy In enemySprites
            Graphics.DrawRectangle(New RectangleF(enemy.Position, enemy.Size), 
                                 enemy.Color, True)
        Next
        
        ' Draw UI
        Graphics.DrawText($"Score: {score}", New PointF(10, 10), gameFont, Color.White)
        Graphics.DrawText($"Time: {gameTime.TotalSeconds:F1}", New PointF(10, 40), gameFont, Color.White)
        
        If gameOver Then
            Graphics.DrawText("GAME OVER", New PointF(300, 250), gameFont, Color.Red)
        End If
        
        Graphics.EndDraw()
    End Sub
    
    Private Sub UpdatePlayerMovement()
        Dim speed As Single = 5.0F
        
        If Input.IsKeyDown(Keys.Left) Then playerSprite.Position.X -= speed
        If Input.IsKeyDown(Keys.Right) Then playerSprite.Position.X += speed
        If Input.IsKeyDown(Keys.Up) Then playerSprite.Position.Y -= speed
        If Input.IsKeyDown(Keys.Down) Then playerSprite.Position.Y += speed
        
        ' Boundary checking
        playerSprite.Position.X = Math.Max(0, Math.Min(Window.ClientSize.Width - playerSprite.Size.Width, playerSprite.Position.X))
        playerSprite.Position.Y = Math.Max(0, Math.Min(Window.ClientSize.Height - playerSprite.Size.Height, playerSprite.Position.Y))
    End Sub
    
    Private Sub UpdateGameLogic()
        ' Move enemies
        For Each enemy In enemySprites
            enemy.Position.Y += 2
            If enemy.Position.Y > Window.ClientSize.Height Then
                enemy.Position.Y = -enemy.Size.Height
                enemy.Position.X = New Random().Next(0, Window.ClientSize.Width - CInt(enemy.Size.Width))
                score += 10
            End If
        Next
    End Sub
    
    Private Sub CheckCollisions()
        Dim playerRect As New RectangleF(playerSprite.Position, playerSprite.Size)
        
        For Each enemy In enemySprites
            Dim enemyRect As New RectangleF(enemy.Position, enemy.Size)
            If playerRect.IntersectsWith(enemyRect) Then
                gameOver = True
                Audio.PlaySound("crash")
                Exit For
            End If
        Next
    End Sub
    
    Private Sub OnKeyPressed(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.R
                If gameOver Then ResetGame()
            Case Keys.Escape
                [Stop]()
            Case Keys.F11
                Window.ToggleFullScreen()
        End Select
    End Sub
    
    Private Sub OnMouseClicked(sender As Object, e As MouseEventArgs)
        If gameOver AndAlso e.Button = MouseButtons.Left Then
            ResetGame()
        End If
    End Sub
    
    Private Sub ResetGame()
        playerSprite.Position = New PointF(400, 300)
        For Each enemy In enemySprites
            enemy.Position = New PointF(100 + enemySprites.IndexOf(enemy) * 150, 100)
        Next
        score = 0
        gameOver = False
    End Sub
    
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            gameFont?.Dispose()
            playerSprite?.Dispose()
            For Each enemy In enemySprites
                enemy?.Dispose()
            Next
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class