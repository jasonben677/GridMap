using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    private static readonly object _locker = new object();
    private static T _instance;

    public static T GetInstance()
    {
        if (_instance == null)
        {
            lock (_locker)
            { 
                _instance = FindObjectOfType(typeof(T)) as T;

                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }

                if (Application.isPlaying) DontDestroyOnLoad(_instance);
            }
        }

        return _instance;
    }
}
