using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DotComponent : MachineComponent
{
    [SerializeField]
    Dot _dotPrefab;

    List<Dot> _instantiatedDots = new();

    public List<Dot> instantiatedDots
    {
        get { return _instantiatedDots; }
    }

    [System.Serializable]
    internal class DotArgs
    {
        public string color;
    }

    [System.Serializable]
    internal class DotAndRemoveArgs
    {
        public string color;
        public float? seconds;
    }

    Transform _transformCached;

    void Start()
    {
        _transformCached = machine.transform;
    }

    protected virtual IEnumerator Dot(string jsonArguments)
    {
        if (!machine.isRunning)
        {
            yield return null;
            SendCoroutineComplete(this.name, "Dot");
            yield break;
        }
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
        }
        yield return null;
        SendCoroutineComplete(this.name, "Dot");
    }

    protected virtual IEnumerator DotAndRemove(string jsonArguments)
    {
        if (!machine.isRunning)
        {
            yield return null;
            SendCoroutineComplete(this.name, "DotAndRemove");
            yield break;
        }
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
            var seconds = 1.5f;

            if (!string.IsNullOrEmpty(jsonArguments))
            {
                DotAndRemoveArgs args = JsonUtility.FromJson<DotAndRemoveArgs>(jsonArguments);
                color = args.color;
                if (args.seconds != null)
                {
                    seconds = args.seconds.GetValueOrDefault();
                }
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
            SendCoroutineComplete(this.name, "DotAndRemove");
            yield return new WaitForSeconds(seconds);
            _instantiatedDots.RemoveAll(d => dotInstance.GetInstanceID() == d.GetInstanceID());
            StartCoroutine(Remove(dotInstance));
            yield break;
        }
        yield return null;
        SendCoroutineComplete(this.name, "DotAndRemove");
    }

    public IEnumerator RemoveAtCurrentPosition()
    {
        yield return null;
        SendCoroutineComplete(this.name, "RemoveAtCurrentPosition");
        if (_instantiatedDots == null || _instantiatedDots.Count == 0)
        {
            yield break;
        }
        var dot = _instantiatedDots.FirstOrDefault(
            d => Utility.ArePositionsEqual(_transformCached.position, d.transform.position)
        );
        if (dot != null)
        {
            _instantiatedDots.RemoveAll(d => dot.GetInstanceID() == d.GetInstanceID());
            yield return StartCoroutine(Remove(dot));
        }
    }

    private IEnumerator Remove(Dot dot)
    {
        yield return StartCoroutine(dot.Hide());

        Destroy(dot.gameObject);
    }

    public override IEnumerator Remove()
    {
        if (_instantiatedDots == null || _instantiatedDots.Count == 0)
        {
            yield return null;
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

    public bool DotAhead(Transform machineTransform)
    {
        var position = machineTransform.position;
        return _instantiatedDots.Any(dot =>
        {
            return Utility.ArePositionsEqual(position, dot.transform.position);
        });
    }
}
