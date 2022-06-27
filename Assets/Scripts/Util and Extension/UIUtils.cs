
using UnityEngine;

using UnityEngine.UI;
using System.Linq;

public class UIUtils {

	public static bool AnyInputActive() {

        return Selectable.allSelectables.Any(
            sel => sel is InputField && ((InputField)sel).isFocused);
    }

    public static void BalancePrefabs(GameObject prefab, int amount, Transform parent) {

        for (int i = parent.childCount; i < amount; i++) {
            var go = GameObject.Instantiate(prefab);
            go.transform.SetParent(parent, false);
        }

        for (int i = parent.childCount - 1; i >= amount; i--) {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }
}
