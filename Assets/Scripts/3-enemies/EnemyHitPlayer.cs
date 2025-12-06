using UnityEngine;
public class EnemyHitPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(name + " [EnemyHitPlayer] OnTriggerEnter2D with: " + other.name);
        if (!other.CompareTag("Player"))
        {
            Debug.Log(name + " hit something that is NOT Player (tag = " + other.tag + ")");
            return;
        }
        Debug.Log(name + " confirmed hit on PLAYER");
        PlayerSpawn spawn = other.GetComponent<PlayerSpawn>();
        if (spawn == null)
        {
            Debug.LogWarning("Player object has NO PlayerSpawn component!");
            return;
        }

        Debug.Log("Calling Respawn() on PlayerSpawn");
        spawn.Respawn();
    }
}
