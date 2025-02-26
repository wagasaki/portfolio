using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class SingleSceneSingleton<T> : StaticInstance<T> where T : SingleSceneSingleton<T>
{
    protected override void Awake()
    {
        if(Instance !=null)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}

public abstract class MultiSceneSingleton<T> : StaticInstance<T> where T : MultiSceneSingleton<T>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}



#region 사용 예시
//public class ObjectPoolingManager : MultiSceneSingleton<ObjectPoolingManager>
//{
//    // ...

//    protected override void Awake()
//    {
//        base.Awake();
//        // ...
//    }
//}
#endregion