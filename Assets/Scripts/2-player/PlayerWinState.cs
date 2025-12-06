using UnityEngine;

public class PlayerWinState : MonoBehaviour
{
    public KeyboardMoverByTile mover;

    void OnEnable()
    {
        Debug.Log("PLAYER STATE: Win");
    }
}
