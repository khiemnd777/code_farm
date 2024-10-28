using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ResetController : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    public Machine machinePrefab;

    Machine _currentMachine;

    Camera _camera;

    void Start()
    {
        _currentMachine = FindAnyObjectByType<Machine>();
        _camera = FindAnyObjectByType<Camera>();
    }

    public void Spawn()
    {
        _currentMachine = Instantiate<Machine>
        (
            machinePrefab,
            new Vector3(0, 0, -1),
            Quaternion.identity
        );
        _currentMachine.name = "Block";
        _currentMachine.transform.localScale = Vector3.zero;
        StartCoroutine(SpawnEffect(_currentMachine.transform));
        StartCoroutine(MoveCameraToTarget(new Vector3(0, 0, _camera.transform.position.z)));
    }

    private IEnumerator SpawnEffect(Transform target)
    {
        float duration = .25f; // Duration of the scaling effect
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            target.localScale = new Vector3(scale, scale, scale);

            yield return null; // Wait for the next frame
        }

        target.localScale = Vector3.one;

        yield return null;

        SendCoroutineComplete(this.name, "Spawn");
    }

    private IEnumerator MoveCameraToTarget(Vector3 targetPosition)
    {
        var duration = .25f;
        var elapsedTime = 0f;

        var startingPosition = _camera.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _camera.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);

            yield return null;
        }

        _camera.transform.position = targetPosition;
    }

    public IEnumerator Remove()
    {
        var components = _currentMachine.machineComponents;
        if (components != null)
        {
            components.ForEach(component => StartCoroutine(component.Remove()));
        }

        var duration = .25f;
        var elapsedTime = 0f;

        Vector3 originalScale = _currentMachine.transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;
            _currentMachine.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(_currentMachine.gameObject);
        yield return null;
        SendCoroutineComplete(this.name, "Remove");
    }
}
