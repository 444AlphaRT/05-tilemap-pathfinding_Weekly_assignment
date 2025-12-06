using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
    } public void Respawn()
    {
        transform.position = startPosition;
        Debug.Log("Player respawned to start position");
    }
}
