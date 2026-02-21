using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitFlag : MonoBehaviour
{
    public int nextSceneBuildIndex = -1;
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

        int next = (nextSceneBuildIndex >= 0)
            ? nextSceneBuildIndex
            : SceneManager.GetActiveScene().buildIndex + 1;

        GameManager.Instance.LoadNextLevel(next);
    }
}