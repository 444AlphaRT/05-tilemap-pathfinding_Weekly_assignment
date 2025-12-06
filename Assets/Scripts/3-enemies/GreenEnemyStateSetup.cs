using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class GreenEnemyStateSetup : MonoBehaviour
{
    public StateMachine stateMachine;
    public GreenEnemyIdleState idleState;
    public GreenEnemyDefendBoatState defendState;
    public Transform player;      
    public Transform boatCenter;  
    public float defendRadius = 3f;  

    void Awake()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<StateMachine>();
        stateMachine
            .AddState(idleState)
            .AddState(defendState);
        stateMachine.AddTransition(
            idleState,
            () => IsPlayerNearBoat(),
            defendState
        );
        stateMachine.AddTransition(
            defendState,
            () => !IsPlayerNearBoat(),
            idleState
        );
        defendState.player = player;
        defendState.boatCenter = boatCenter;
    }
    private bool IsPlayerNearBoat()
    {
        if (player == null || boatCenter == null) return false;

        float dist = Vector3.Distance(player.position, boatCenter.position);
        return dist <= defendRadius;
    }
}
