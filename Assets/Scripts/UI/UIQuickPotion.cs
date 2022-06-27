
using UnityEngine;

using UnityEngine.UI;

public class UIQuickPotion : MonoBehaviour {

    public GameObject panel = null;
    public Transform content = null;

    private void Update() {
        var player = FindObjectOfType<Player>();
        if (player == null) {
            panel.SetActive(false); return;
        }

        if (panel.activeSelf == true) {
            for (int i = 0; i < player.potionbar.Length; i++) {
                var slot        = content.GetChild(i).GetComponent<UIInventorySlot>();
                    slot.dragAndDropable.name = i.ToString();
                var itemIndex   = player.potionbar[i];
                if (itemIndex == -1) {
                    slot.icon.sprite                = null;
                    slot.icon.color                 = Color.clear;
                    slot.overlay.SetActive(false);
                    slot.dragAndDropable.dragable   = false;
                    continue;
                }

                var item                    = player.inventory[itemIndex];
                    slot.icon.sprite        = item.icon;
                    slot.icon.color         = Color.white;
                    slot.overlay.SetActive(item.amount > 1);
                    slot.overlayValue.text  = item.amount.ToString();
            }
        }
    }
}
