using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DotComponent : MachineComponent
{
    [SerializeField]
    Dot _dotPrefab;

    List<Dot> _instantiatedDots = new();

    [System.Serializable]
    internal class DotArgs
    {
        public string color;
    }

    Transform _transformCached;

    void Start()
    {
        _transformCached = machine.transform;
    }

    protected virtual IEnumerator Dot(string jsonArguments)
    {
        if (_dotPrefab != null)
        {
            var dotInstance = Instantiate<Dot>(_dotPrefab, transform.position, Quaternion.identity);
            _instantiatedDots.Add(dotInstance);
            var currentPosition = dotInstance.transform.position;
            currentPosition.z = 0f;
            currentPosition.x = Mathf.Round(currentPosition.x);
            currentPosition.y = Mathf.Round(currentPosition.y);
            dotInstance.transform.position = currentPosition;

            var color = "#FFFFFF";

            if (!string.IsNullOrEmpty(jsonArguments))
            {
                DotArgs args = JsonUtility.FromJson<DotArgs>(jsonArguments);
                color = args.color;
            }

            if (string.IsNullOrEmpty(color))
            {
                color = "#FFFFFF";
            }

            if (ColorUtility.TryParseHtmlString(color, out Color parsedColor))
            {
                dotInstance.dotRenderer.color = parsedColor;
            }
            else
            {
                dotInstance.dotRenderer.color = Color.white;
            }

            yield return null;
        }
        SendCoroutineComplete(this.name, "Dot");
    }

    public IEnumerator RemoveAtCurrentPosition()
    {
        if (_instantiatedDots == null || _instantiatedDots.Count == 0)
        {
            SendCoroutineComplete(this.name, "RemoveAtCurrentPosition");
            yield break;
        }
        var dot = _instantiatedDots.FirstOrDefault(
            d => Utility.ArePositionsEqual(_transformCached.position, d.transform.position)
        );
        if (dot != null)
        {
            yield return StartCoroutine(Remove(dot));
        }
        yield return null;
        SendCoroutineComplete(this.name, "RemoveAtCurrentPosition");
    }

    private IEnumerator Remove(Dot dot)
    {
        var duration = .25f;
        var elapsedTime = 0f;

        var dotRenderer = dot.dotRenderer;
        var originalScale = dot.transform.localScale;
        var originalColor = dotRenderer.color;
        var fadedColor = originalColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;

            // Scale down
            dot.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            // Fade out
            fadedColor.a = Mathf.Lerp(originalColor.a, 0, t);
            dotRenderer.color = fadedColor;

            yield return null;
        }

        Destroy(dot.gameObject);
    }

    public override IEnumerator Remove()
    {
        if (_instantiatedDots == null || _instantiatedDots.Count == 0)
        {
            SendCoroutineComplete(this.name, "RemoveAllDots");
            yield break;
        }
        foreach (var dot in _instantiatedDots)
        {
            if (dot != null)
            {
                StartCoroutine(Remove(dot));
            }
        }

        _instantiatedDots.Clear();

        yield return null;

        SendCoroutineComplete(this.name, "RemoveAllDots");
    }
}
