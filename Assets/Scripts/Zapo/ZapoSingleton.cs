using UnityEngine;

public class ZapoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    protected static bool appShuttingDown = false;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (appShuttingDown)
                    return null;

                _instance = (T)FindFirstObjectByType(typeof(T));
            }

            return _instance;
        }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }
    public virtual void OnDestroy()
    {
        if (this == _instance)
            _instance = null;
    }
    void OnApplicationQuit()
    {
        appShuttingDown = true;
    }
}