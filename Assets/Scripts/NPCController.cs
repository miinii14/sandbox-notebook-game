using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private AStarPathfinder pathFinder;
    public Transform player;
    public float fleeDistance = 3f;

    public NPCStateMachine stateMachine;

    private void Start()
    {
        stateMachine = new NPCStateMachine();
        stateMachine.ChangeState(new IdleState(this));
    }

    public void Init()
    {
        player = GameObject.Find("PLAYER").transform;
    }

    private void Update()
    {
        stateMachine.Update();
    }
}
