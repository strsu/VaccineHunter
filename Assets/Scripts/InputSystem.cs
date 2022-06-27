
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public static class InputSystem {

    private static Dictionary<string, KeyCode> Keys = new Dictionary<string, KeyCode>();

    static InputSystem() { SetDefaultBinds(); }

    private static void SetDefaultBinds() {

        #region movement
        Keys.Add("Forward"          , KeyCode.W);
        Keys.Add("Back"             , KeyCode.S);
        Keys.Add("Left"             , KeyCode.A);
        Keys.Add("Right"            , KeyCode.D);
        Keys.Add("Jump"             , KeyCode.Space);
        #endregion
        #region UI
        Keys.Add("Menu"             , KeyCode.Escape);
        Keys.Add("MiniMap"          , KeyCode.N);
        Keys.Add("WorldMap"         , KeyCode.M);
        Keys.Add("Inventory"        , KeyCode.I);
        Keys.Add("Equipment"        , KeyCode.E);
        Keys.Add("Quest"            , KeyCode.Q);
        Keys.Add("Skill"            , KeyCode.K);
        Keys.Add("Character Info"   , KeyCode.T);
        #endregion
        #region interaction
        Keys.Add("Communication"    , KeyCode.F);
        Keys.Add("ShortCut"         , KeyCode.Tab);
        #endregion
        #region skill
        Keys.Add("Alpha1"           , KeyCode.Alpha1);
        Keys.Add("Alpha2"           , KeyCode.Alpha2);
        Keys.Add("Alpha3"           , KeyCode.Alpha3);
        Keys.Add("Alpha4"           , KeyCode.Alpha4);
        Keys.Add("Alpha5"           , KeyCode.Alpha5);
        Keys.Add("Alpha6"           , KeyCode.Alpha6);
        Keys.Add("Alpha7"           , KeyCode.Alpha7);
        Keys.Add("Alpha8"           , KeyCode.Alpha8);
        Keys.Add("Alpha9"           , KeyCode.Alpha9);
        Keys.Add("Alpha10"          , KeyCode.Alpha0);
        Keys.Add("Normal Attack"    , KeyCode.Mouse0);
        #endregion
        #region system
        Keys.Add("screen shot"      , KeyCode.F9);
        #endregion
    }

    public static float GetAxis (string key) {
        if (Keys.ContainsKey(key) == false) {
            Debug.LogError("InputManager::GetAxis -- No button named: " + key);
            return 0;
        }
        return Input.GetAxis(key);
    }
    public static bool GetKey (string key) {
        if (Keys.ContainsKey(key) == false) {
            Debug.LogError("InputManager::GetKey -- No button named: " + key);
            return false;
        }
        return Input.GetKey(Keys[key]);
    }
    public static bool GetKeyDown (string key) {
        if (Keys.ContainsKey(key) == false) {
            Debug.LogError("InputManager::GetKeyDown -- No button named: " + key);
            return false;
        }
        return Input.GetKeyDown(Keys[key]);
    }
    public static string[] GetKeyNames () {
        return Keys.Keys.ToArray();
    }
    public static string GetKeyNameForKeyString(string key) {
        if ( Keys.ContainsKey(key) == false ) {
            Debug.LogError("InputManager::GetKeyNameForKeyString -- No button named: " + key);
            return "N/A";
        }

        return Keys[key].ToString();
    }
    public static void SetButtonForKeyCode(string key, KeyCode keyCode) {

        if (Keys.ContainsValue(keyCode)) {
            var _key = Keys.FirstOrDefault(x => x.Value == keyCode).Key;
            Keys[_key] = Keys[key];
        }
        Keys[key] = keyCode;
    }
    public static KeyCode GetKeyCodeForKeyName(string key) {

        if (Keys.ContainsKey(key) == false) {
            Debug.LogError("InputManager::GetKeyCodeForeKeyName -- No button named: " + key);
            return KeyCode.None;
        }

        return Keys[key];
    }
}
