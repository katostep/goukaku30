using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour{

    private static T instance;
    public static T Instance
    {
        get{
            if (instance == null) {
                Type t = typeof(T);

                instance = (T)FindObjectOfType (t);
                if (instance == null) {
                    Debug.LogError (t + " をAddComponentしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

}