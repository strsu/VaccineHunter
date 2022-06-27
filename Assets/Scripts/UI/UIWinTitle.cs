
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIWinTitle : MonoBehaviour,
    IDragHandler,
    IBeginDragHandler {

    public GameObject panel = null;
    public Button btnClose = null;

    private void Awake() {
        btnClose.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }

    private Vector2 prevPoint = Vector2.zero;

    public void OnBeginDrag(PointerEventData data) {
        prevPoint = data.position;
    }
    public void OnDrag(PointerEventData data)
    {
        Vector3 temp                        = data.position - prevPoint;
                panel.transform.position   += temp;
                prevPoint                   = data.position;
    }
}
