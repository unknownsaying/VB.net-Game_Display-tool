Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Text

' XInput structures and constants
Public Class XINPUT_STATE
    Public dwPacketNumber As Integer
    Public Gamepad As XINPUT_GAMEPAD
End Class

Public Class XINPUT_GAMEPAD
    Public wButtons As Short
    Public bLeftTrigger As Byte
    Public bRightTrigger As Byte
    Public sThumbLX As Short
    Public sThumbLY As Short
    Public sThumbRX As Short
    Public sThumbRY As Short
End Class

' XInput button constants
Public Module XInputConstants
    Public Const XINPUT_GAMEPAD_DPAD_UP As Short = &H0001
    Public Const XINPUT_GAMEPAD_DPAD_DOWN As Short = &H0002
    Public Const XINPUT_GAMEPAD_DPAD_LEFT As Short = &H0004
    Public Const XINPUT_GAMEPAD_DPAD_RIGHT As Short = &H0008
    Public Const XINPUT_GAMEPAD_START As Short = &H0010
    Public Const XINPUT_GAMEPAD_BACK As Short = &H0020
    Public Const XINPUT_GAMEPAD_LEFT_THUMB As Short = &H0040
    Public Const XINPUT_GAMEPAD_RIGHT_THUMB As Short = &H0080
    Public Const XINPUT_GAMEPAD_LEFT_SHOULDER As Short = &H0100
    Public Const XINPUT_GAMEPAD_RIGHT_SHOULDER As Short = &H0200
    Public Const XINPUT_GAMEPAD_A As Short = &H1000
    Public Const XINPUT_GAMEPAD_B As Short = &H2000
    Public Const XINPUT_GAMEPAD_X As Short = &H4000
    Public Const XINPUT_GAMEPAD_Y As Short = &H8000
End Module

' XInput API declaration
Public Module XInputAPI
    <DllImport("xinput1_3.dll")>
    Public Function XInputGetState(ByVal dwUserIndex As Integer, ByRef pState As XINPUT_STATE) As Integer
    End Function
End Module

' Main form class
Public Class GamepadForm
    Inherits Form

    Private statusLabel As Label
    Private updateTimer As Timer

    Public Sub New()
        ' Initialize form properties
        Me.Text = "Gamepad and Joystick Interface"
        Me.Size = New Size(500, 400)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Create status label
        statusLabel = New Label()
        statusLabel.Location = New Point(10, 10)
        statusLabel.Size = New Size(460, 340)
        statusLabel.Font = New Font("Consolas", 10)
        statusLabel.BorderStyle = BorderStyle.FixedSingle
        Me.Controls.Add(statusLabel)

        ' Create and configure timer
        updateTimer = New Timer()
        updateTimer.Interval = 50 ' Update every 50ms
        AddHandler updateTimer.Tick, AddressOf UpdateGamepadStatus
        updateTimer.Start()
    End Sub

    Private Sub UpdateGamepadStatus(sender As Object, e As EventArgs)
        Dim state As New XINPUT_STATE()
        Dim result As Integer = XInputGetState(0, state) ' Player 1

        Dim sb As New StringBuilder()

        If result = 0 Then ' Success
            sb.AppendLine("Gamepad Connected - Player 1")
            sb.AppendLine("========================")

            ' Button states
            sb.AppendLine("BUTTONS:")
            sb.Append("DPad: ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_DPAD_UP) <> 0 Then sb.Append("UP ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_DPAD_DOWN) <> 0 Then sb.Append("DOWN ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_DPAD_LEFT) <> 0 Then sb.Append("LEFT ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_DPAD_RIGHT) <> 0 Then sb.Append("RIGHT ")
            sb.AppendLine()

            sb.Append("Face Buttons: ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_A) <> 0 Then sb.Append("A ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_B) <> 0 Then sb.Append("B ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_X) <> 0 Then sb.Append("X ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_Y) <> 0 Then sb.Append("Y ")
            sb.AppendLine()

            sb.Append("Shoulders: ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_LEFT_SHOULDER) <> 0 Then sb.Append("LB ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_RIGHT_SHOULDER) <> 0 Then sb.Append("RB ")
            sb.AppendLine()

            sb.Append("Thumbsticks: ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_LEFT_THUMB) <> 0 Then sb.Append("L3 ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_RIGHT_THUMB) <> 0 Then sb.Append("R3 ")
            sb.AppendLine()

            sb.Append("Special: ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_START) <> 0 Then sb.Append("START ")
            If (state.Gamepad.wButtons And XINPUT_GAMEPAD_BACK) <> 0 Then sb.Append("BACK ")
            sb.AppendLine()

            ' Analog values
            sb.AppendLine()
            sb.AppendLine("ANALOG INPUTS:")
            sb.AppendLine(String.Format("Left Thumbstick: X={0,6} Y={1,6}", state.Gamepad.sThumbLX, state.Gamepad.sThumbLY))
            sb.AppendLine(String.Format("Right Thumbstick: X={0,6} Y={1,6}", state.Gamepad.sThumbRX, state.Gamepad.sThumbRY))
            sb.AppendLine(String.Format("Left Trigger: {0,3}%", CInt(state.Gamepad.bLeftTrigger / 255.0 * 100)))
            sb.AppendLine(String.Format("Right Trigger: {0,3}%", CInt(state.Gamepad.bRightTrigger / 255.0 * 100)))

            ' Packet number (for change detection)
            sb.AppendLine()
            sb.AppendLine(String.Format("Packet Number: {0}", state.dwPacketNumber))
        Else
            sb.AppendLine("Gamepad Not Connected")
            sb.AppendLine("Please connect an Xbox-compatible controller.")
        End If

        statusLabel.Text = sb.ToString()
    End Sub
End Class

' Application entry point
Public Module Program
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New GamepadForm())
    End Sub
End Module
