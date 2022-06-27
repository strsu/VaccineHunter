

using System;   // Int32....
using UnityEngine;

using UnityEngine.Events;

public static class Extensions {

    public static int ToInt(this string value, int errVal = 0) {

        Int32.TryParse(value, out errVal);
        return errVal;
    }
    public static long ToLong(this string value, long errVal = 0) {

        Int64.TryParse(value, out errVal);
        return errVal;
    }
    public static void SetListener(this UnityEvent uEvent, UnityAction call) {
        uEvent.RemoveAllListeners();
        uEvent.AddListener(call);
    }
    public static void SetListener<T>(this UnityEvent<T> uEvent, UnityAction<T> call) {
        uEvent.RemoveAllListeners();
        uEvent.AddListener(call);
    }
    public static Transform FindRecursively(this Transform transform, string name) {

        return Array.Find(transform.GetComponentsInChildren<Transform>(true),
                          t => t.name == name);
    }
}