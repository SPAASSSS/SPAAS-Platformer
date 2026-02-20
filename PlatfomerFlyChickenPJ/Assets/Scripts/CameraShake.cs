using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalLocalPos;
    private Coroutine shakeCo;

    private void Awake()
    {
        originalLocalPos = transform.localPosition;
    }

    public void Shake(float duration = 0.15f, float strength = 0.2f)
    {
        if (shakeCo != null) StopCoroutine(shakeCo);
        shakeCo = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float t = 0f;

        while (t < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            transform.localPosition = originalLocalPos + new Vector3(x, y, 0f);

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        shakeCo = null;
    }
}