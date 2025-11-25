Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Collections.Generic
Imports System.ComponentModel

Namespace ComfortInput

    ' Keyboard input manager with comfortable key handling
    Public Class KeyboardManager
        Implements IDisposable

        Private _keyStates As New Dictionary(Of Keys, Boolean)
        Private _keyPressCallbacks As New Dictionary(Of Keys, List(Of Action))
        Private _keyReleaseCallbacks As New Dictionary(Of Keys, List(Of Action))
        Private _keyCombinationCallbacks As New Dictionary(Of String, List(Of Action))
        Private _isDisposed As Boolean = False

        Public Event KeyPressed As EventHandler(Of KeyEventArgs)
        Public Event KeyReleased As EventHandler(Of KeyEventArgs)
        Public Event KeyCombinationPressed As EventHandler(Of String)

        Public Sub New()
            ' Initialize all keys to not pressed
            For Each key As Keys In [Enum].GetValues(GetType(Keys))
                _keyStates(key) = False
            Next
        End Sub

        ' Update key state
        Public Sub UpdateKeyState(key As Keys, isPressed As Boolean)
            If _keyStates.ContainsKey(key) Then
                Dim wasPressed As Boolean = _keyStates(key)
                _keyStates(key) = isPressed

                If isPressed AndAlso Not wasPressed Then
                    OnKeyPressed(New KeyEventArgs(key))
                    ExecuteCallbacks(_keyPressCallbacks, key)
                ElseIf Not isPressed AndAlso wasPressed Then
                    OnKeyReleased(New KeyEventArgs(key))
                    ExecuteCallbacks(_keyReleaseCallbacks, key)
                End If
            End If

            CheckKeyCombinations()
        End Sub

        ' Check if key is currently pressed
        Public Function IsKeyPressed(key As Keys) As Boolean
            Return _keyStates.ContainsKey(key) AndAlso _keyStates(key)
        End Function

        ' Register callback for key press
        Public Sub RegisterKeyPress(key As Keys, callback As Action)
            If Not _keyPressCallbacks.ContainsKey(key) Then
                _keyPressCallbacks(key) = New List(Of Action)()
            End If
            _keyPressCallbacks(key).Add(callback)
        End Sub

        ' Register callback for key release
        Public Sub RegisterKeyRelease(key As Keys, callback As Action)
            If Not _keyReleaseCallbacks.ContainsKey(key) Then
                _keyReleaseCallbacks(key) = New List(Of Action)()
            End If
            _keyReleaseCallbacks(key).Add(callback)
        End Sub

        ' Register key combination (e.g., "Ctrl+S")
        Public Sub RegisterKeyCombination(combination As String, callback As Action)
            If Not _keyCombinationCallbacks.ContainsKey(combination) Then
                _keyCombinationCallbacks(combination) = New List(Of Action)()
            End If
            _keyCombinationCallbacks(combination).Add(callback)
        End Sub

        Private Sub ExecuteCallbacks(callbacksDict As Dictionary(Of Keys, List(Of Action)), key As Keys)
            If callbacksDict.ContainsKey(key) Then
                For Each callback As Action In callbacksDict(key)
                    callback.Invoke()
                Next
            End If
        End Sub

        Private Sub CheckKeyCombinations()
            For Each combination As String In _keyCombinationCallbacks.Keys
                If IsKeyCombinationPressed(combination) Then
                    OnKeyCombinationPressed(combination)
                    For Each callback As Action In _keyCombinationCallbacks(combination)
                        callback.Invoke()
                    Next
                End If
            Next
        End Sub

        Private Function IsKeyCombinationPressed(combination As String) As Boolean
            Dim parts As String() = combination.Split("+"c)
            For Each part As String In parts
                part = part.Trim()
                Dim key As Keys
                If [Enum].TryParse(part, True, key) Then
                    If Not IsKeyPressed(key) Then
                        Return False
                    End If
                Else
                    Return False
                End If
            Next
            Return True
        End Function

        Protected Overridable Sub OnKeyPressed(e As KeyEventArgs)
            RaiseEvent KeyPressed(Me, e)
        End Sub

        Protected Overridable Sub OnKeyReleased(e As KeyEventArgs)
            RaiseEvent KeyReleased(Me, e)
        End Sub

        Protected Overridable Sub OnKeyCombinationPressed(combination As String)
            RaiseEvent KeyCombinationPressed(Me, combination)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _isDisposed Then
                _keyPressCallbacks.Clear()
                _keyReleaseCallbacks.Clear()
                _keyCombinationCallbacks.Clear()
                _keyStates.Clear()
                _isDisposed = True
            End If
        End Sub
    End Class

    ' Mouse input manager with comfortable handling
    Public Class MouseManager
        Implements IDisposable

        Private _mousePosition As Point
        Private _lastMousePosition As Point
        Private _mouseDelta As Point
        Private _buttonStates As New Dictionary(Of MouseButtons, Boolean)
        Private _scrollValue As Integer
        Private _isDisposed As Boolean = False

        Public Event MouseMove As EventHandler(Of MouseEventArgs)
        Public Event MouseDown As EventHandler(Of MouseEventArgs)
        Public Event MouseUp As EventHandler(Of MouseEventArgs)
        Public Event MouseClick As EventHandler(Of MouseEventArgs)
        Public Event MouseDoubleClick As EventHandler(Of MouseEventArgs)
        Public Event MouseWheel As EventHandler(Of MouseEventArgs)

        Public Sub New()
            ' Initialize all mouse buttons to not pressed
            For Each button As MouseButtons In [Enum].GetValues(GetType(MouseButtons))
                _buttonStates(button) = False
            Next
        End Sub

        ' Update mouse position
        Public Sub UpdateMousePosition(x As Integer, y As Integer)
            _lastMousePosition = _mousePosition
            _mousePosition = New Point(x, y)
            _mouseDelta = New Point(x - _lastMousePosition.X, y - _lastMousePosition.Y)
            OnMouseMove(New MouseEventArgs(MouseButtons.None, 0, x, y, 0))
        End Sub

        ' Update mouse button state
        Public Sub UpdateMouseButton(button As MouseButtons, isPressed As Boolean)
            If _buttonStates.ContainsKey(button) Then
                Dim wasPressed As Boolean = _buttonStates(button)
                _buttonStates(button) = isPressed

                If isPressed AndAlso Not wasPressed Then
                    OnMouseDown(New MouseEventArgs(button, 1, _mousePosition.X, _mousePosition.Y, 0))
                ElseIf Not isPressed AndAlso wasPressed Then
                    OnMouseUp(New MouseEventArgs(button, 1, _mousePosition.X, _mousePosition.Y, 0))
                    OnMouseClick(New MouseEventArgs(button, 1, _mousePosition.X, _mousePosition.Y, 0))
                End If
            End If
        End Sub

        ' Update mouse wheel
        Public Sub UpdateMouseWheel(delta As Integer)
            _scrollValue += delta
            OnMouseWheel(New MouseEventArgs(MouseButtons.None, 0, _mousePosition.X, _mousePosition.Y, delta))
        End Sub

        ' Check if mouse button is pressed
        Public Function IsMouseButtonPressed(button As MouseButtons) As Boolean
            Return _buttonStates.ContainsKey(button) AndAlso _buttonStates(button)
        End Function

        Public ReadOnly Property Position As Point
            Get
                Return _mousePosition
            End Get
        End Property

        Public ReadOnly Property Delta As Point
            Get
                Return _mouseDelta
            End Get
        End Property

        Public ReadOnly Property ScrollValue As Integer
            Get
                Return _scrollValue
            End Get
        End Property

        Protected Overridable Sub OnMouseMove(e As MouseEventArgs)
            RaiseEvent MouseMove(Me, e)
        End Sub

        Protected Overridable Sub OnMouseDown(e As MouseEventArgs)
            RaiseEvent MouseDown(Me, e)
        End Sub

        Protected Overridable Sub OnMouseUp(e As MouseEventArgs)
            RaiseEvent MouseUp(Me, e)
        End Sub

        Protected Overridable Sub OnMouseClick(e As MouseEventArgs)
            RaiseEvent MouseClick(Me, e)
        End Sub

        Protected Overridable Sub OnMouseDoubleClick(e As MouseEventArgs)
            RaiseEvent MouseDoubleClick(Me, e)
        End Sub

        Protected Overridable Sub OnMouseWheel(e As MouseEventArgs)
            RaiseEvent MouseWheel(Me, e)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _isDisposed Then
                _buttonStates.Clear()
                _isDisposed = True
            End If
        End Sub
    End Class

    ' Input manager that combines keyboard and mouse
    Public Class ComfortInputManager
        Implements IDisposable

        Private _keyboardManager As New KeyboardManager()
        Private _mouseManager As New MouseManager()
        Private _inputForm As InputForm
        Private _isDisposed As Boolean = False

        Public Sub New()
            _inputForm = New InputForm(Me)
            AddHandler _inputForm.KeyDown, AddressOf OnFormKeyDown
            AddHandler _inputForm.KeyUp, AddressOf OnFormKeyUp
            AddHandler _inputForm.MouseMove, AddressOf OnFormMouseMove
            AddHandler _inputForm.MouseDown, AddressOf OnFormMouseDown
            AddHandler _inputForm.MouseUp, AddressOf OnFormMouseUp
            AddHandler _inputForm.MouseWheel, AddressOf OnFormMouseWheel
        End Sub

        Public ReadOnly Property Keyboard As KeyboardManager
            Get
                Return _keyboardManager
            End Get
        End Property

        Public ReadOnly Property Mouse As MouseManager
            Get
                Return _mouseManager
            End Get
        End Property

        Public Sub ShowInputWindow()
            _inputForm.Show()
        End Sub

        Public Sub HideInputWindow()
            _inputForm.Hide()
        End Sub

        Private Sub OnFormKeyDown(sender As Object, e As KeyEventArgs)
            _keyboardManager.UpdateKeyState(e.KeyCode, True)
        End Sub

        Private Sub OnFormKeyUp(sender As Object, e As KeyEventArgs)
            _keyboardManager.UpdateKeyState(e.KeyCode, False)
        End Sub

        Private Sub OnFormMouseMove(sender As Object, e As MouseEventArgs)
            _mouseManager.UpdateMousePosition(e.X, e.Y)
        End Sub

        Private Sub OnFormMouseDown(sender As Object, e As MouseEventArgs)
            _mouseManager.UpdateMouseButton(e.Button, True)
        End Sub

        Private Sub OnFormMouseUp(sender As Object, e As MouseEventArgs)
            _mouseManager.UpdateMouseButton(e.Button, False)
        End Sub

        Private Sub OnFormMouseWheel(sender As Object, e As MouseEventArgs)
            _mouseManager.UpdateMouseWheel(e.Delta)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _isDisposed Then
                _keyboardManager.Dispose()
                _mouseManager.Dispose()
                _inputForm.Dispose()
                _isDisposed = True
            End If
        End Sub

        ' Internal form for capturing input
        Private Class InputForm
            Inherits Form

            Private _parentManager As ComfortInputManager

            Public Sub New(parentManager As ComfortInputManager)
                _parentManager = parentManager
                Me.Text = "Comfort Input Manager"
                Me.Size = New Size(300, 200)
                Me.StartPosition = FormStartPosition.CenterScreen
                Me.KeyPreview = True ' Important for capturing key events
            End Sub

            Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
                e.Cancel = True ' Prevent closing
                Me.Hide()
                MyBase.OnFormClosing(e)
            End Sub
        End Class
    End Class

    ' Custom event args for key events
    Public Class KeyEventArgs
        Inherits EventArgs

        Public ReadOnly Property KeyCode As Keys

        Public Sub New(keyCode As Keys)
            Me.KeyCode = keyCode
        End Sub
    End Class

End Namespace

' Demo application showing usage
Public Class ComfortInputDemo
    Inherits Form

    Private WithEvents inputManager As New ComfortInput.ComfortInputManager()
    Private statusLabel As Label
    Private logTextBox As TextBox

    Public Sub New()
        Me.Text = "Comfort Input Demo"
        Me.Size = New Size(600, 400)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Create UI elements
        statusLabel = New Label()
        statusLabel.Location = New Point(10, 10)
        statusLabel.Size = New Size(560, 30)
        statusLabel.Font = New Font("Arial", 12, FontStyle.Bold)
        Me.Controls.Add(statusLabel)

        logTextBox = New TextBox()
        logTextBox.Location = New Point(10, 50)
        logTextBox.Size = New Size(560, 300)
        logTextBox.Multiline = True
        logTextBox.ScrollBars = ScrollBars.Vertical
        logTextBox.Font = New Font("Consolas", 9)
        Me.Controls.Add(logTextBox)

        ' Setup comfortable input callbacks
        SetupComfortableInputs()

        ' Show input window
        inputManager.ShowInputWindow()

        UpdateStatus()
    End Sub

    Private Sub SetupComfortableInputs()
        ' Register comfortable key combinations
        inputManager.Keyboard.RegisterKeyCombination("Control+S", AddressOf SaveShortcut)
        inputManager.Keyboard.RegisterKeyCombination("Control+O", AddressOf OpenShortcut)
        inputManager.Keyboard.RegisterKeyCombination("Control+Q", AddressOf QuitShortcut)
        inputManager.Keyboard.RegisterKeyCombination("Alt+F4", AddressOf AltF4Shortcut)

        ' Register individual key callbacks
        inputManager.Keyboard.RegisterKeyPress(Keys.Space, AddressOf SpacePressed)
        inputManager.Keyboard.RegisterKeyRelease(Keys.Space, AddressOf SpaceReleased)

        ' Register mouse event handlers
        AddHandler inputManager.Mouse.MouseMove, AddressOf OnMouseMove
        AddHandler inputManager.Mouse.MouseClick, AddressOf OnMouseClick
        AddHandler inputManager.Mouse.MouseWheel, AddressOf OnMouseWheel
    End Sub

    Private Sub LogMessage(message As String)
        If logTextBox.InvokeRequired Then
            logTextBox.Invoke(New Action(Of String)(AddressOf LogMessage), message)
        Else
            logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}" & Environment.NewLine)
            logTextBox.SelectionStart = logTextBox.TextLength
            logTextBox.ScrollToCaret()
        End If
    End Sub

    Private Sub UpdateStatus()
        statusLabel.Text = "Comfort Input Demo - Use Ctrl+S, Ctrl+O, Space, etc."
    End Sub

    ' Keyboard callback implementations
    Private Sub SaveShortcut()
        LogMessage("💾 Comfortable Save shortcut activated (Ctrl+S)")
    End Sub

    Private Sub OpenShortcut()
        LogMessage("📁 Comfortable Open shortcut activated (Ctrl+O)")
    End Sub

    Private Sub QuitShortcut()
        LogMessage("🚪 Comfortable Quit shortcut activated (Ctrl+Q)")
        Me.Close()
    End Sub

    Private Sub AltF4Shortcut()
        LogMessage("⚠️  Alt+F4 pressed - Application will close")
        Me.Close()
    End Sub

    Private Sub SpacePressed()
        LogMessage("␣ Space key pressed")
    End Sub

    Private Sub SpaceReleased()
        LogMessage("␣ Space key released")
    End Sub

    ' Mouse callback implementations
    Private Sub OnMouseMove(sender As Object, e As MouseEventArgs)
        ' Only log occasionally to avoid spam
        If DateTime.Now.Second Mod 5 = 0 Then
            LogMessage($"🖱️  Mouse moved to ({e.X}, {e.Y})")
        End If
    End Sub

    Private Sub OnMouseClick(sender As Object, e As MouseEventArgs)
        LogMessage($"🖱️  Mouse clicked with {e.Button} at ({e.X}, {e.Y})")
    End Sub

    Private Sub OnMouseWheel(sender As Object, e As MouseEventArgs)
        LogMessage($"🖱️  Mouse wheel scrolled: {e.Delta}")
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        inputManager.Dispose()
        MyBase.OnFormClosing(e)
    End Sub
End Class

' Application entry point
Public Module Program
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New ComfortInputDemo())
    End Sub
End Module