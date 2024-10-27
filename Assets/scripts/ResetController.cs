using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ResetController : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    public Machine machinePrefab;

    Machine _currentMachine;

    void Start()
    {
        _currentMachine = FindAnyObjectByType<Machine>();
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
    }

    private IEnumerator SpawnEffect(Transform target)
    {
        float duration = 1.0f; // Duration of the scaling effect
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

    public IEnumerator Reset()
    {
        var components = _currentMachine.machineComponents;
        if (components != null)
        {
            components.ForEach(component => StartCoroutine(component.Remove()));
        }

        var duration = 0.5f; // Duration of fade and scale out
        var elapsedTime = 0f;

        Vector3 originalScale = this.transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;
            this.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(_currentMachine.gameObject);

        yield return null;

        SendCoroutineComplete(this.name, "Remove");
    }
}
