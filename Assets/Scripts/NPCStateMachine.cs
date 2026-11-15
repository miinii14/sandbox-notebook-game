public class NPCStateMachine
{
    private INPCState currentState;

    public void ChangeState(INPCState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}