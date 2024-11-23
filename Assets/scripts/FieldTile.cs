using System.Collections;
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

    Coroutine _tooltipMoveCoroutine;

    Coroutine _tooltipTextColorCoroutine;

    public EdgeHighlighter[] edgeHighlighters;
    public FloorTileHighlight floorHighlight;

    [SerializeField]
    BoxCollider2D _boxCollider2D;

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

    void Update()
    {
        if (_boxCollider2D)
        {
            if (_boxCollider2D.enabled != !LabyrinthSettings.isMazeMode)
            {
                _boxCollider2D.enabled = !LabyrinthSettings.isMazeMode;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!LabyrinthSettings.isMazeMode)
        {
            _tooltipText.gameObject.SetActive(true);

            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }
            _colorCoroutine = StartCoroutine(FadeTileColor(0f, _fadeTransparent));

            if (_tooltipMoveCoroutine != null)
            {
                StopCoroutine(_tooltipMoveCoroutine);
            }
            _tooltipMoveCoroutine = StartCoroutine(MoveTooltipY(0f, 0.644f));

            if (_tooltipTextColorCoroutine != null)
            {
                StopCoroutine(_tooltipTextColorCoroutine);
            }
            _tooltipTextColorCoroutine = StartCoroutine(FadeTooltipTextColor(0f, 1f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!LabyrinthSettings.isMazeMode)
        {
            _tooltipText.gameObject.SetActive(false);

            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }
            _colorCoroutine = StartCoroutine(FadeTileColor(_fadeTransparent, 0f));

            if (_tooltipMoveCoroutine != null)
            {
                StopCoroutine(_tooltipMoveCoroutine);
            }
            _tooltipMoveCoroutine = StartCoroutine(MoveTooltipY(0.644f, 0f));

            if (_tooltipTextColorCoroutine != null)
            {
                StopCoroutine(_tooltipTextColorCoroutine);
            }
            _tooltipTextColorCoroutine = StartCoroutine(FadeTooltipTextColor(1f, 0f));
        }
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
        float duration = 0.35f;

        startAlpha = currentColor.a;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            currentColor.a = newAlpha;
            _tooltipText.color = currentColor;

            yield return null;
        }

        currentColor.a = endAlpha;
        _tooltipText.color = currentColor;
    }
}
