using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class LabyrinthFloor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [NonSerialized]
    public LabyrinthFloorController floorController;

    [SerializeField]
    SpriteRenderer _floorSprite;

    float _edgeHighlightTargetScaleMultiplier = 1.2f;
    float _edgeHighlightAnimationDuration = 0.2f;

    Coroutine _highlightScaleCoroutine;

    bool AllowBuildTheFloor()
    {
        return LabyrinthSettings.isMazeMode && LabyrinthSettings.thing == LabyrinthThings.floor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AllowBuildTheFloor())
        {
            if (_floorSprite)
            {
                if (_highlightScaleCoroutine != null) StopCoroutine(_highlightScaleCoroutine);

                _highlightScaleCoroutine = StartCoroutine(AnimateScale(_floorSprite.transform, Vector3.one, Vector3.one * _edgeHighlightTargetScaleMultiplier, 0f));
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (AllowBuildTheFloor())
        {
            if (_floorSprite)
            {
                if (_highlightScaleCoroutine != null) StopCoroutine(_highlightScaleCoroutine);

                _highlightScaleCoroutine = StartCoroutine(AnimateScale(_floorSprite.transform, Vector3.one * _edgeHighlightTargetScaleMultiplier, Vector3.one, _edgeHighlightAnimationDuration));
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AllowBuildTheFloor())
        {
            if (_floorSprite)
            {
                StartCoroutine(AnimateScaleDelete(_floorSprite.transform, Vector3.one * _edgeHighlightTargetScaleMultiplier, Vector3.zero, _edgeHighlightAnimationDuration));
            }
        }
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

    private IEnumerator AnimateScaleDelete(Transform targetTransform, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            targetTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        targetTransform.localScale = toScale;

        yield return null;

        if (floorController)
        {
            floorController.RemoveFloor(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
