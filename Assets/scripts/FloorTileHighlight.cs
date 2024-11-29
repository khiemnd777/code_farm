using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorTileHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [NonSerialized]
    public LabyrinthFloorController floorController;

    [SerializeField]
    BoxCollider2D _boxCollider2D;

    public BoxCollider2D boxCollider2D
    {
        get { return _boxCollider2D; }
    }

    [SerializeField]
    SpriteRenderer _tileRenderer;

    [SerializeField]
    float _fadeTransparent = .5f;

    Coroutine _colorCoroutine;

    bool AllowBuildTheFloor()
    {
        return LabyrinthSettings.isMazeMode && LabyrinthSettings.thing == LabyrinthThings.floor;
    }

    // Start is called before the first frame update
    void Start()
    {
        var initialColor = _tileRenderer.color;
        initialColor.a = 0f;
        _tileRenderer.color = initialColor;
    }

    void Update()
    {
        // if (_boxCollider2D)
        // {
        //     if (_boxCollider2D.enabled != AllowBuildTheFloor())
        //     {
        //         _boxCollider2D.enabled = AllowBuildTheFloor();
        //     }
        // }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AllowBuildTheFloor())
        {
            if (_tileRenderer && floorController)
            {
                floorController.GenerateFloor(_tileRenderer.transform);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AllowBuildTheFloor())
        {
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }
            _colorCoroutine = StartCoroutine(FadeTileColor(0f, _fadeTransparent));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (AllowBuildTheFloor())
        {
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }
            _colorCoroutine = StartCoroutine(FadeTileColor(_fadeTransparent, 0f));
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
}
