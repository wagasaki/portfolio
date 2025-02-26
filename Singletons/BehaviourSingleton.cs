using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if(_instance==null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;

                if(_instance == null) _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            }
            return _instance;
        }
    }
}

