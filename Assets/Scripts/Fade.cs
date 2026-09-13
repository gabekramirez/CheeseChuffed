using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (canvasGroup == null && spriteRenderer == null)
        {
            Debug.LogWarning(
                "Fade requires either a CanvasGroup or SpriteRenderer."
            );
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeTo(0f));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeTo(1f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = GetAlpha();
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / fadeDuration
            );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    public void SetAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        else if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    private float GetAlpha()
    {
        if (canvasGroup != null)
        {
            return canvasGroup.alpha;
        }

        if (spriteRenderer != null)
        {
            return spriteRenderer.color.a;
        }

        return 1f;
    }
}