using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private string coinId;

    public string CoinId => coinId;

    private void Awake()
    {
        if (string.IsNullOrEmpty(coinId))
        {
            int s = gameObject.scene.buildIndex;
            Vector3 p = transform.position;

            int x = Mathf.RoundToInt(p.x * 100f);
            int y = Mathf.RoundToInt(p.y * 100f);

            coinId = $"{s}:{x}:{y}";
        }

        if (GameManager.Instance != null && GameManager.Instance.IsCoinCollected(coinId))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
            GameManager.Instance.CollectCoin(coinId);

        Destroy(gameObject);
    }
}