using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Vector3 respawnPoint;

    private void Awake()
    {
        respawnPoint = transform.position;
    }

    public void SetCheckpoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Checkpoint set: " + respawnPoint);
    }

    public void Respawn()
    {
        transform.position = respawnPoint;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        var hp = GetComponent<PlayerHealth>();
        if (hp != null) hp.Revive();
    }
}