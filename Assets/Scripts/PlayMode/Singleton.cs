﻿using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Component
{

    protected static T _instance;

    public static T Instance { get { return _instance; } }

    protected virtual void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
    }
}
