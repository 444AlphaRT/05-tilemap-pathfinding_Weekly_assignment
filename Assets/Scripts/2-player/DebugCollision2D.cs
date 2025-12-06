using UnityEngine;

public class DebugCollision2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(name + " OnTriggerEnter2D with: " + other.name);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(name + " OnCollisionEnter2D with: " + other.gameObject.name);
    }
}
