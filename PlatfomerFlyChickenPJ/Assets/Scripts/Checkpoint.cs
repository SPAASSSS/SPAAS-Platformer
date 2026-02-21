using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.SetCheckpoint(transform.position);
    }
}