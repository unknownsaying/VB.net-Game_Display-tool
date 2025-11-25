Public Interface IGameState
    Sub Update(deltaTime As Double)
    Sub Render(g As Graphics)
    Sub HandleInput(input As InputManager)
    Sub OnEnter()
    Sub OnExit()
End Interface

Public Class GameStateManager
    Private states As Stack(Of IGameState)
    
    Public Sub New()
        states = New Stack(Of IGameState)()
    End Sub
    
    Public Sub PushState(state As IGameState)
        If states.Count > 0 Then
            states.Peek().OnExit()
        End If
        states.Push(state)
        state.OnEnter()
    End Sub
    
    Public Sub PopState()
        If states.Count > 0 Then
            states.Pop().OnExit()
        End If
        If states.Count > 0 Then
            states.Peek().OnEnter()
        End If
    End Sub
    
    Public Sub ChangeState(state As IGameState)
        While states.Count > 0
            states.Pop().OnExit()
        End While
        PushState(state)
    End Sub
    
    Public ReadOnly Property CurrentState As IGameState
        Get
            Return If(states.Count > 0, states.Peek(), Nothing)
        End Get
    End Property
End Class