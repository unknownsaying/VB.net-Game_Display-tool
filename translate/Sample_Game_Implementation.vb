Public Class SampleGame
    Inherits GameFramework
    
    Private playerX As Integer = 400
    Private playerY As Integer = 300
    Private playerSpeed As Integer = 5
    Private playerSize As Integer = 32
    
    Public Sub New()
        MyBase.New()
        Me.Text = "Sample VB.NET Game"
        
        ' Load resources
        resourceManager.LoadImage("player", playerSize, playerSize)
        resourceManager.LoadFont("gameFont", "Arial", 16)
    End Sub
    
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        InitializeGame()
    End Sub
    
    Private Sub InitializeGame()
        ' Initialize game objects
        audioPlayer.LoadSound("beep", New Byte() {}) ' Placeholder for sound data
    End Sub
    
    Private Sub GameLoop(sender As Object, e As EventArgs)
        UpdateGame()
        RenderGame()
    End Sub
    
    Private Sub UpdateGame()
        ' Handle input
        If inputHandler.IsKeyDown(Keys.Left) Then playerX -= playerSpeed
        If inputHandler.IsKeyDown(Keys.Right) Then playerX += playerSpeed
        If inputHandler.IsKeyDown(Keys.Up) Then playerY -= playerSpeed
        If inputHandler.IsKeyDown(Keys.Down) Then playerY += playerSpeed
        
        ' Boundary checking
        playerX = Math.Max(0, Math.Min(gameWidth - playerSize, playerX))
        playerY = Math.Max(0, Math.Min(gameHeight - playerSize, playerY))
        
        ' Play sound on space press
        If inputHandler.IsKeyPressed(Keys.Space) Then
            audioPlayer.PlaySound("beep")
        End If
    End Sub
    
    Private Sub RenderGame()
        ' Clear canvas
        graphicsBuffer.Clear(Color.CornflowerBlue)
        
        ' Draw player
        graphicsBuffer.FillRectangle(Brushes.Red, playerX, playerY, playerSize, playerSize)
        graphicsBuffer.DrawRectangle(Pens.DarkRed, playerX, playerY, playerSize, playerSize)
        
        ' Draw UI
        graphicsBuffer.DrawString($"Position: ({playerX}, {playerY})", 
                                resourceManager.GetFont("gameFont"), 
                                Brushes.White, 10, 10)
        
        ' Draw to screen
        renderGraphics.DrawImage(gameCanvas, 0, 0)
    End Sub
    
    Private Sub OnPaint(sender As Object, e As PaintEventArgs)
        RenderGame()
    End Sub
    
    Private Sub OnFormClosing(sender As Object, e As FormClosingEventArgs)
        isRunning = False
        gameTimer.Stop()
        graphicsBuffer.Dispose()
        gameCanvas.Dispose()
        renderGraphics.Dispose()
    End Sub
End Class