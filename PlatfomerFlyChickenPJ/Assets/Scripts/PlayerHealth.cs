using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{

    public int maxHP = 3;
    public int currentHP;

    public float invincibleTime = 0.6f;
    private float invincibleTimer;

    public SpriteRenderer sprite;
    public Color damageColor = Color.red;
    public float flashDuration = 0.12f;
    public int flashCount = 2;

    private Color originalColor;
    private Coroutine flashRoutine;

    public CameraShake cameraShake;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.2f;

    public float deathDelay = 0.15f;
    private bool isDying = false;

    public event Action<int> OnHealthChanged;

    private void Awake()
    {
        currentHP = maxHP;

        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite) originalColor = sprite.color;

        if (!cameraShake && Camera.main != null)
            cameraShake = Camera.main.GetComponent<CameraShake>();

        OnHealthChanged?.Invoke(currentHP);
    }

    private void Update()
    {
        if (invincibleTimer > 0f)
            invincibleTimer -= Time.deltaTime;
    }

    public void TakeDamage(int dmg)
    {
        if (invincibleTimer > 0f) return;

        currentHP -= dmg;
        invincibleTimer = invincibleTime;

        OnHealthChanged?.Invoke(currentHP);

        if (sprite)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashRed());
        }

        cameraShake?.Shake(shakeDuration, shakeStrength);

        Debug.Log($"Player HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        originalColor = sprite.color;

        for (int i = 0; i < flashCount; i++)
        {
            sprite.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            sprite.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        sprite.color = originalColor;
        flashRoutine = null;
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.linearVelocity = Vector2.zero;

        var move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        StartCoroutine(DieRoutine());
    }

    public void TakeMaxDamage()
    {
        invincibleTimer = 0f;

        TakeDamage(maxHP);
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(deathDelay);

        gameObject.SetActive(false);
    }
}