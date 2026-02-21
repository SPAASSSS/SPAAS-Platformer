using UnityEngine;

public class GameClearTrigger : MonoBehaviour
{
    public GameObject visuals;
    public Collider2D triggerCollider;

    private void Awake()
    {
        if (!triggerCollider) triggerCollider = GetComponent<Collider2D>();
        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        if (visuals != null) visuals.SetActive(visible);
        if (triggerCollider != null) triggerCollider.enabled = visible;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameClearUI.Instance?.Show(true); // true = Game Clear
    }
}