using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : MonoSingleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //유니티, 시스템 오브젝트의 null문제 해결을 위해
    private static bool isDestroyed = false; 
    private static T instance;
    //private static object m_Lock = new object();

    /// <summary>
    /// Access singleton instance through this propriety
    /// </summary>
    public static T Instance
    {
        get
        {
            if (isDestroyed)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            //스레드락 제거. 멀티스레드쓸일이 별로없기도 하고 성능 걱정
            //lock (m_Lock)
            //{
                if ((object)instance == null)
                {
                    // Search for existing instance.
                    instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if ((object)instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return instance;
           // }
        }
    }

    protected virtual void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                   "' already exists. Destroying This");
            Destroy(this);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isDestroyed = true;
    }


    protected virtual void OnDestroy()
    {
        isDestroyed = true;
    }
}