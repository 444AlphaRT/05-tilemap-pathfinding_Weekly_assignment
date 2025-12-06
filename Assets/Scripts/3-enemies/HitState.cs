using UnityEngine;
using System.Collections;
public class PlayerHitState : MonoBehaviour
{
    public Transform player;
    public Vector3 respawnPoint;  
    public StateMachine machine;  
    public MonoBehaviour playingState;
    void OnEnable()
    {
        Debug.Log("Player HIT! Respawning...");
        player.GetComponent<KeyboardMoverByTile>().enabled = false;
        player.position = respawnPoint;
        StartCoroutine(ReturnToPlaying());
    }
     IEnumerator ReturnToPlaying()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Player returned to Playing state");
        player.GetComponent<KeyboardMoverByTile>().enabled = true;
        machine.GoToState(playingState as MonoBehaviour);
    }
}
