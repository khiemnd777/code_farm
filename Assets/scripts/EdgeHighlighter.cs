using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EdgeHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    BoxCollider2D _boxCollider2D;

    public BoxCollider2D boxCollider2D
    {
        get { return _boxCollider2D; }
    }

    [NonSerialized]
    public ObstacleSegmentController obstacleSegmentController;

    Color _edgeHighlightOriginalColor = Color.white;
    Color _edgeHighlightHighlightColor = Color.yellow;

    float _edgeHighlightTargetScaleMultiplier = 1.2f;
    float _edgeHighlightAnimationDuration = 0.3f;

    Coroutine _edgeHighlightColorCoroutine;
    Coroutine _edgeHighlightScaleCoroutine;

    SpriteRenderer _edgeSprite;

    // Start is called before the first frame update
    void Start()
    {
        _edgeSprite = this.GetComponent<SpriteRenderer>();
        _edgeHighlightOriginalColor = _edgeSprite.color;
    }

    void Update()
    {
        // if (_boxCollider2D)
        // {
        //     if (_boxCollider2D.enabled != AllowBuildTheWall())
        //     {
        //         _boxCollider2D.enabled = AllowBuildTheWall();
        //     }
        // }
    }

    bool AllowBuildTheWall()
    {
        return LabyrinthSettings.isMazeMode && LabyrinthSettings.thing == LabyrinthThings.wall;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AllowBuildTheWall())
        {
            if (_edgeSprite != null)
            {
                if (_edgeHighlightColorCoroutine != null)
                {
                    StopCoroutine(_edgeHighlightColorCoroutine);
                }
                if (_edgeHighlightScaleCoroutine != null)
                {
                    StopCoroutine(_edgeHighlightScaleCoroutine);
                }
                _edgeHighlightColorCoroutine = StartCoroutine(AnimateColor(_edgeSprite, _edgeHighlightOriginalColor, _edgeHighlightHighlightColor, 0f));
                _edgeHighlightScaleCoroutine = StartCoroutine(AnimateScale(_edgeSprite.transform, new Vector3(1.03f, 0.12f, 0f), new Vector3(1.03f, 0.2f, 0f) * _edgeHighlightTargetScaleMultiplier, 0f));
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (AllowBuildTheWall())
        {
            if (_edgeSprite != null)
            {
                if (_edgeHighlightColorCoroutine != null) StopCoroutine(_edgeHighlightColorCoroutine);
                if (_edgeHighlightScaleCoroutine != null) StopCoroutine(_edgeHighlightScaleCoroutine);

                _edgeHighlightColorCoroutine = StartCoroutine(AnimateColor(_edgeSprite, _edgeHighlightHighlightColor, _edgeHighlightOriginalColor, _edgeHighlightAnimationDuration));
                _edgeHighlightScaleCoroutine = StartCoroutine(AnimateScale(_edgeSprite.transform, new Vector3(1.03f, 0.2f, 0f) * _edgeHighlightTargetScaleMultiplier, new Vector3(1.03f, 0.12f, 0f), _edgeHighlightAnimationDuration));
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AllowBuildTheWall())
        {
            if (_edgeSprite && obstacleSegmentController)
            {
                obstacleSegmentController.GenerateObstacle(_edgeSprite.transform);
            }
        }
    }

    private IEnumerator AnimateColor(SpriteRenderer spriteRenderer, Color fromColor, Color toColor, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spriteRenderer.color = Color.Lerp(fromColor, toColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = toColor;
    }

    private IEnumerator AnimateScale(Transform targetTransform, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            targetTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        targetTransform.localScale = toScale;
    }
}
