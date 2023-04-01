using System.Collections;
using UnityEngine;

public class Digger : Machine
{
  protected override void RegisterVariables()
  {
    RegisterVariableOrFunction("dig", new System.Func<IEnumerator>(Dig), true);
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
      transform.localScale = Vector3.Lerp(startingScale, finalScale, elapsedTime);
      yield return null;
    }

    transform.localScale = Vector3.one;
    yield return new WaitForSeconds(.25f);
  }
}
