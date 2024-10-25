using System.Collections;
using UnityEngine;

public class DotComponent : MachineComponent
{
    [SerializeField]
    Dot _dotPrefab;

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
}
