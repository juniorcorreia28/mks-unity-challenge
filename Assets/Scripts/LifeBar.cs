using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField][Range(0,1)]
    private float fadeDuration;
    [SerializeField]
    private float lifeBarDuration;
    [Header("OBJECTS")]
    [SerializeField]
    private Image imageLifeBar;

    private bool isFading;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    public bool IsShowing { get; private set; }
    public float Duration { get { return lifeBarDuration; } private set { lifeBarDuration = value; } }
    public Color Color { get; set; }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        imageLifeBar.color = Color;
    }

    /// <summary>
    /// Atualiza a barra de vida
    /// </summary>
    /// <param name="health">Vida atual</param>
    /// <param name="healthMax">Vida maxima</param>
    public void UpdateLifeBar(int health, int healthMax)
    {
        imageLifeBar.fillAmount = Mathf.Clamp((float)health / (float)healthMax, 0f, 1f);
    }

    /// <summary>
    /// Atualiza a posição da barra de vida
    /// </summary>
    /// <param name="position">Posição</param>
    public void Position(Vector2 position)
    {
        if (rectTransform)
        {
            rectTransform.position = position;
        }
    }

    /// <summary>
    /// Mostra a barra de vida
    /// </summary>
    /// <param name="active">Ativa ou Desativa</param>
    public void Show(bool active)
    {
        if (active)
        {
            if (isFading)
            {
                StopCoroutine(FadeOutCoroutine());
            }
            else
            {
                StartCoroutine(FadeInCoroutine());
            }
        }
        else
        {
            if (isFading)
            {
                StopCoroutine(FadeInCoroutine());
            }
            else
            {
                StartCoroutine(FadeOutCoroutine());
            }
        }
    }

    #region Coroutines
    /// <summary>
    /// Mostra a barra de vida
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        isFading = true;
        IsShowing = true;

        canvasGroup.alpha = 0;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration;

            yield return null;
        }

        canvasGroup.alpha = 1;

        isFading = false;

        yield return null;
    }

    /// <summary>
    /// Esconde a barra de vida
    /// </summary>
    private IEnumerator FadeOutCoroutine()
    {
        isFading = true;
        IsShowing = false;

        canvasGroup.alpha = 1;

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;

            yield return null;
        }

        canvasGroup.alpha = 0;

        isFading = false;

        yield return null;
    }
    #endregion
}
