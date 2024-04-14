using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Reflection;

public static class ZapoHelpers
{
    public static Transform TransformByName(Transform xform, string name)
    {
        foreach (Transform child in xform)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                var result = TransformByName(child, name);
                if (result)
                {
                    return result;
                }
            }
        }
        return null;
    }

    public static void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
#if UNITY_EDITOR
    public static void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));

        System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    public static void SetSelectedGameObject(GameObject target)
    {
        Selection.activeGameObject = target;
    }
#else
    public static void ClearLog() { }
    public static void SetSelectedGameObject(GameObject target) { }
#endif
}