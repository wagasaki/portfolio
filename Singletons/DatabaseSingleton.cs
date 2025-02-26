using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DatabaseSingleton<T> where T : class
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Activator.CreateInstance(typeof(T)) as T;
            }
            return _instance;
        }
    }
}
