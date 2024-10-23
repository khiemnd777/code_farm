using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    TMP_Text _tooltipText;

    [SerializeField]
    SpriteRenderer _tileRenderer;

    [SerializeField]
    float _fadeTransparent = .5f;

    public Field field;

    Coroutine _colorCoroutine;

    Coroutine tooltipMoveCoroutine;

    Coroutine tooltipTextColorCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _tooltipText.text = $"({field.x}, {field.y})";
        _tooltipText.gameObject.SetActive(false);

        var initialColor = _tileRenderer.color;
        initialColor.a = 0f;
        _tileRenderer.color = initialColor;

        _tooltipText.rectTransform.localPosition = new Vector3
        (
            _tooltipText.rectTransform.localPosition.x,
            0f,
            _tooltipText.rectTransform.localPosition.z
        );

        Color tooltipInitialColor = _tooltipText.color;
        tooltipInitialColor.a = 0f;
        _tooltipText.color = tooltipInitialColor;
    }

    public void OnPointerMove(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipText.gameObject.SetActive(true);

        if (_colorCoroutine != null)
        {
            StopCoroutine(_colorCoroutine);
        }
        _colorCoroutine = StartCoroutine(FadeTileColor(0f, _fadeTransparent));

        if (tooltipMoveCoroutine != null)
        {
            StopCoroutine(tooltipMoveCoroutine); // Stop any ongoing tooltip movement
        }
        tooltipMoveCoroutine = StartCoroutine(MoveTooltipY(0f, 0.644f));

        if (tooltipTextColorCoroutine != null)
        {
            StopCoroutine(tooltipTextColorCoroutine); // Stop any ongoing tooltip text fade-out
        }
        tooltipTextColorCoroutine = StartCoroutine(FadeTooltipTextColor(0f, 1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipText.gameObject.SetActive(false);

        if (_colorCoroutine != null)
        {
            StopCoroutine(_colorCoroutine);
        }
        _colorCoroutine = StartCoroutine(FadeTileColor(_fadeTransparent, 0f));

        if (tooltipMoveCoroutine != null)
        {
            StopCoroutine(tooltipMoveCoroutine); // Stop any ongoing tooltip movement
        }
        tooltipMoveCoroutine = StartCoroutine(MoveTooltipY(0.644f, 0f));

        if (tooltipTextColorCoroutine != null)
        {
            StopCoroutine(tooltipTextColorCoroutine); // Stop any ongoing tooltip text fade-in
        }
        tooltipTextColorCoroutine = StartCoroutine(FadeTooltipTextColor(1f, 0f));
    }

    IEnumerator FadeTileColor(float startAlpha, float endAlpha)
    {
        Color currentColor = _tileRenderer.color;
        float elapsedTime = 0f;
        float duration = 0.35f;

        startAlpha = currentColor.a;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            currentColor.a = newAlpha;
            _tileRenderer.color = currentColor;

            yield return null;
        }

        currentColor.a = endAlpha;
        _tileRenderer.color = currentColor;
    }

    IEnumerator MoveTooltipY(float startY, float endY)
    {
        float elapsedTime = 0f;
        float duration = 0.125f;
        Vector3 startPos = _tooltipText.rectTransform.localPosition;
        startPos.y = startY;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float newY = Mathf.Lerp(startY, endY, t);

            Vector3 newPos = _tooltipText.rectTransform.localPosition;
            newPos.y = newY;
            _tooltipText.rectTransform.localPosition = newPos;

            yield return null;
        }

        Vector3 finalPos = _tooltipText.rectTransform.localPosition;
        finalPos.y = endY;
        _tooltipText.rectTransform.localPosition = finalPos;
    }

    IEnumerator FadeTooltipTextColor(float startAlpha, float endAlpha)
    {
        Color currentColor = _tooltipText.color;
        float elapsedTime = 0f;
        float duration = 0.35f; // Duration of the fade

        startAlpha = currentColor.a; // Get current alpha as the starting point

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp between the start and end alpha values
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            // Apply the new alpha to the current color
            currentColor.a = newAlpha;
            _tooltipText.color = currentColor;

            yield return null;
        }

        // Ensure the final alpha is exactly the target value
        currentColor.a = endAlpha;
        _tooltipText.color = currentColor;
    }
}
