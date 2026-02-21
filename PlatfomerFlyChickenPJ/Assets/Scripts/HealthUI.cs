using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image[] hearts;

    private void OnEnable()
    {
        if (!playerHealth)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth)
        {
            playerHealth.OnHealthChanged += UpdateHearts;
            UpdateHearts(playerHealth.currentHP);
        }
    }

    private void OnDisable()
    {
        if (playerHealth)
            playerHealth.OnHealthChanged -= UpdateHearts;
    }

    private void UpdateHearts(int hp)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = (i < hp);
    }
}