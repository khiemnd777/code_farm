using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotComponent : MachineComponent
{
    [SerializeField]
    Dot _dotPrefab;

    List<Dot> _instantiatedDots = new ();

    [System.Serializable]
    internal class DotArgs
    {
        public string color;
    }

    protected virtual IEnumerator Dot(string jsonArguments)
    {
        if (_dotPrefab != null)
        {
            var dotInstance = Instantiate<Dot>(_dotPrefab, transform.position, Quaternion.identity);
            _instantiatedDots.Add (dotInstance);
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

    private IEnumerator Remove(Dot dot)
    {
        var duration = 0.5f; // Duration of fade and scale out
        var elapsedTime = 0f;

        SpriteRenderer dotRenderer = dot.GetComponent<SpriteRenderer>();
        Vector3 originalScale = dot.transform.localScale;
        Color originalColor = dotRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;

            // Scale down
            dot.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            // Fade out
            var fadedColor = originalColor;
            fadedColor.a = Mathf.Lerp(originalColor.a, 0, t);
            dotRenderer.color = fadedColor;

            yield return null;
        }

        Destroy(dot);
    }

    public override IEnumerator Remove()
    {
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
