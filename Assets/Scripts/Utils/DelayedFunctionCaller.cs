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
        // use WaitForFixedUpdate for determinism
        int delayTimeThousands = Mathf.RoundToInt(delayTime * 1000);
        int fixedDeltaTimeThousands = Mathf.RoundToInt(Time.fixedDeltaTime * 1000);
        int frameCount = delayTimeThousands / fixedDeltaTimeThousands;
        for (int i = 0; i < frameCount; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        targetFunction();
    }
}
