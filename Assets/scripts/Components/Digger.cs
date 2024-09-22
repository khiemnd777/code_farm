using System.Collections;
using UnityEngine;

public class Digger : MachineComponent
{
    [SerializeField]
    float _enegryForDigging;

    public override void RegisterVariables()
    {
        print("Register dig function");
        machine.RegisterVariableOrFunction("dig", new System.Func<IEnumerator>(Dig), true);
    }

    IEnumerator Dig()
    {
        print("Dig");
        var startingScale = Vector3.one;
        var finalScale = Vector3.one * .75f;
        var elapsedTime = 0f;

        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime / .25f;
            machine.transform.localScale = Vector3.Lerp(startingScale, finalScale, elapsedTime);
            yield return null;
        }

        machine.transform.localScale = Vector3.one;

        if (!machine.ConsumeEnergy(_enegryForDigging))
        {
            machine.Stop();
            yield break;
        }

        yield return new WaitForSeconds(.25f);
    }
}
