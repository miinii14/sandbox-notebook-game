using UnityEngine;

public class IdleState : INPCState
{
    private NPCController npc;
    private float timer;

    public IdleState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        timer = Random.Range(3f, 8f);
    }

    public void Exit()
    {
        timer = 0f;
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            npc.stateMachine.ChangeState(new PatrolState(npc));
        }
    }
}
