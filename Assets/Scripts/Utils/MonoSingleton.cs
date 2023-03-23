using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("Singleton of " + typeof(T).Name);
                    instance = obj.AddComponent<T>();
                    instance.Init();
                }
            }
            return instance;
        }
    }

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Init();
        }
    }

    public virtual void Init()
    {

    }

}
