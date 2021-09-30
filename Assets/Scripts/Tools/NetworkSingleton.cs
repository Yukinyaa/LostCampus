using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : MonoSingleton<MyClassName> {}
/// </summary>
public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    //유니티, 시스템 오브젝트의 null문제 해결을 위해
    private static bool isDestroyed = false; 
    private static T instance;
    private static object m_Lock = new object();
    private const int NETWORK_TIMEOUT = 50;
    
    /// <summary>
    /// Access singleton instance through this propriety
    /// </summary>
    public static T Instance
    {
        get
        {
            if (isDestroyed)
            {
                Debug.LogWarning("[NetworkSingleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if ((object)instance == null)
                {
                    // Search for existing instance.
                    instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if ((object)instance == null)
                    {
                        CmdMakeInstance();
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        while((object)instance == null)
                        {
                            if (sw.ElapsedMilliseconds > NETWORK_TIMEOUT) break;
                        }
                        sw.Stop();
                    }
                }

                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogWarning("[NetworkSingleton] Instance '" + typeof(T) +
                   "' already exists. Destroying This");
            Destroy(this);
        }
        else
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    [Command(requiresAuthority = false)]
    private static void CmdMakeInstance()
    {
        // Need to create a new GameObject to attach the singleton to.
        var singletonObject = new GameObject();
        singletonObject.AddComponent<NetworkIdentity>();
        instance = singletonObject.AddComponent<T>();
        singletonObject.name = typeof(T).ToString() + " (Singleton)";

        // Make instance persistent.
        DontDestroyOnLoad(singletonObject);
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