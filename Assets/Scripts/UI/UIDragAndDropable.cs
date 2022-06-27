
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDragAndDropable : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

    private Transform transformCanvas = null;
    private void Start() {

        var parent = transform.parent;
        while (parent != null) {

            var canvas = parent.GetComponent<Canvas>();
            if (canvas == null) { parent            = parent.parent;    continue;   }
            else                { transformCanvas   = canvas.transform; break;      }
        }

        if (transformCanvas == null) { Debug.LogError(name + ", UIDragAndDropable : Canvas Doesn't exist"); return; }
    }

    public  PointerEventData.InputButton    inputButton         = PointerEventData.InputButton.Left;
    public  GameObject                      Dragee              = null;
    private GameObject                      currentlyDragged    = null;

    public bool dragable = true;
    public bool dropable = true;

    [HideInInspector] public bool draggedToSlot = false;

    public void OnBeginDrag(PointerEventData data) {
        if (dragable &&
            data.button == inputButton) {
            currentlyDragged                                                    = Instantiate(Dragee, transform.position, Quaternion.identity);
            currentlyDragged.transform.GetChild(0).GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            currentlyDragged.transform.SetParent(transformCanvas, true);
            currentlyDragged.transform.localScale = Vector3.one;
            currentlyDragged.transform.SetAsLastSibling();
        }
    }
    public void OnDrag(PointerEventData data) {
        if (dragable &&
            data.button == inputButton) {
            currentlyDragged.transform.position = data.position;
        }
    }
    public void OnEndDrag(PointerEventData data) {
        Destroy(currentlyDragged);
        if (dragable &&
            data.button == inputButton) {
            if (draggedToSlot == false) {
                var player = FindObjectOfType<Player>();
                player.SendMessage("OnDragAndClear_" + tag,
                                    name.ToInt(),
                                    SendMessageOptions.DontRequireReceiver);
            }
        }
        draggedToSlot = false;
    }
    public void OnDrop(PointerEventData data) {
        if (dropable == true &&
            data.button == inputButton) {
            var dropDragable = data.pointerDrag.GetComponent<UIDragAndDropable>();
            if (dropDragable != null) {
                dropDragable.draggedToSlot = true;
                if (dropDragable != this) {
                    var player      = FindObjectOfType<Player>();
                    int from        = dropDragable.name.ToInt();
                    int to          = name.ToInt();
                    player.SendMessage("OnDragAndDrop_" + dropDragable.tag + "_" + tag,
                                        new int[] { from, to },
                                        SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
    private void OnDisable() {
        Destroy(currentlyDragged);
    }
    private void OnDestroy() {
        Destroy(currentlyDragged);
    }
}
