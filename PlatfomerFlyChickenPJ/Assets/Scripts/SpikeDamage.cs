using UnityEngine;

public class SpikeDamage : MonoBehaviour
{

    public int damage = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var hp = other.GetComponent<PlayerHealth>();
        if (hp != null)
            hp.TakeDamage(damage);
    }
}
