
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine.EventSystems;

using System.Text.RegularExpressions;

public class Utils {

	public static float ClosestDistance(Collider from, Collider to) {

        return Vector3.Distance(from.ClosestPointOnBounds(to.transform.position),
                                to.ClosestPointOnBounds(from.transform.position));
    }
    public static bool IsNullOrWhiteSpace(string value) {

        return String.IsNullOrEmpty(value) || value.Trim().Length == 0;
    }
    public static bool IsInLayerMask(int layer, LayerMask layermask) {

        return layermask == (layermask | (1 << layer));
    }
    public static string ParseFirstNoun(string text) {

        var matches = new Regex(@"([A-Z][a-z]*)").Matches(text);
        return matches.Count > 0 ? matches[0].Value : "";
    }
    public static string ParseLastNoun(string text) {

        var matches = new Regex(@"([A-Z][a-z]*)").Matches(text);
        return matches.Count > 0 ? matches[matches.Count - 1].Value : "";
    }

    public static string ParseLastNoun_jj(string text)
    {
        String[] matches = text.Split(' ');
        return matches[matches.Length-1];
    }

    static Dictionary<KeyValuePair<Type, string>, MethodInfo[]> lookup = new Dictionary<KeyValuePair<Type, string>, MethodInfo[]>();
    public static MethodInfo[] GetMethodsByPrefix(Type type, string methodPrefix) {
        
        var key = new KeyValuePair<Type, string>(type, methodPrefix);
        if (!lookup.ContainsKey(key)) {

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                              .Where(m => m.Name.StartsWith(methodPrefix))
                              .ToArray();

            lookup[key] = methods;
        }

        return lookup[key];
    }

    public static bool IsCursorOverUserInterface() {

        if (EventSystem.current.IsPointerOverGameObject()) return true;

        for (int i = 0; i < Input.touchCount; i++)
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId)) return true;

        return GUIUtility.hotControl != 0;
    }

    public static void InvokeMany(Type type, object onObject, string methodPrefix, params object[] args) {

        foreach (var method in GetMethodsByPrefix(type, methodPrefix)) method.Invoke(onObject, args.ToArray());
    }
}