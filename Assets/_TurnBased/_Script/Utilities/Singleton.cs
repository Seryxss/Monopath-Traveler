using System.Collections;
using UnityEngine;


//Static instance thats basically a singleton, but instead of destroying any new version, it overrides the current instance
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

//Basic Singleton that will destroy any new version created, leaving the original instance intact
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}

//Persistent Singleton is Technically a singleton that will survive through scene. Ex: audioSourced, persistent data or state.
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
