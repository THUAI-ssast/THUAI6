using UnityEngine;
using System.Collections;

public class DelayedFunctionCaller : MonoSingleton<DelayedFunctionCaller>
{
    public static void CallAfter(float delayTime, System.Action targetFunction)
    {
        Instance.StartCoroutine(DelayedCall(delayTime, targetFunction));
    }

    private static IEnumerator DelayedCall(float delayTime, System.Action targetFunction)
    {
        yield return new WaitForSeconds(delayTime); // Depends on the time scale. That is, game time but not real time.
        targetFunction();
    }
}
