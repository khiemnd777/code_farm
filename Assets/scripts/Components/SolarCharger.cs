using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarCharger : MachineComponent
{
    public override void RegisterVariables()
    {
        print("Register charge function");
        machine.RegisterVariableOrFunction("charge", new System.Func<IEnumerator>(Charge), true);
    }

    IEnumerator Charge()
    {
        print("Charging");
        yield return null;
    }
}
