using UnityEngine;

public class PlayerStateSetup : MonoBehaviour
{
    public StateMachine stateMachine;

    public PlayerPlayingState playingState;
    public PlayerWinState winState;
    public PlayerHitState hitState;

    public KeyboardMoverByTile mover;

    public Vector3 startPoint;

    void Awake()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<StateMachine>();

        // לקבוע נקודת התחלה
        startPoint = transform.position;
        hitState.respawnPoint = startPoint;

        // מוסיפים מצבים
        stateMachine
            .AddState(playingState)
            .AddState(winState)
            .AddState(hitState);

        // מעבר ל-Win:
        stateMachine.AddTransition(
            playingState,
            () => mover.ReachedCastle,
            winState
        );
    }

    // נקרא ע"י אויב
    public void GoToHitState()
    {
        stateMachine.GoToState(hitState);
    }
}
