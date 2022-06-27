
using UnityEngine;

public class UILastSibling : MonoBehaviour {

    private void OnEnable() {
        transform.parent.SetAsLastSibling();
    }
}
