using System.Collections;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public SpriteRenderer dotRenderer;

    public float _targetScale = 0.75f;
    public float _scaleDuration = 0.5f;

    Coroutine _currentScaleCoroutine;

    Transform _cachedTransform;

    void Start()
    {
        _cachedTransform = transform;
        _cachedTransform.localScale = Vector3.zero;
        _currentScaleCoroutine = StartCoroutine(ScaleUp());
    }

    IEnumerator ScaleUp()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _scaleDuration)
        {
            float progress = elapsedTime / _scaleDuration;
            float currentScale = Mathf.Lerp(0f, _targetScale, progress);
            _cachedTransform.localScale = new Vector3(currentScale, currentScale, currentScale);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cachedTransform.localScale = new Vector3(_targetScale, _targetScale, _targetScale);

        _currentScaleCoroutine = null;
    }

    public IEnumerator Hide()
    {
        if (_currentScaleCoroutine != null)
        {
            StopCoroutine(_currentScaleCoroutine);
            _currentScaleCoroutine = null;
        }

        var duration = .25f;
        var elapsedTime = 0f;

        var originalScale = _cachedTransform.localScale;
        var originalColor = dotRenderer.color;
        var fadedColor = originalColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;

            _cachedTransform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            fadedColor.a = Mathf.Lerp(originalColor.a, 0, t);
            dotRenderer.color = fadedColor;

            yield return null;
        }

        _cachedTransform.localScale = Vector3.zero;
        fadedColor.a = 0;
        dotRenderer.color = fadedColor;
    }
}
