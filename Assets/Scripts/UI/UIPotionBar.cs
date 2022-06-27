
using UnityEngine;

public class UIPotionBar : MonoBehaviour {

    public GameObject panel = null;
    public Transform content = null;

    public UIInventorySlot slot = null;

    private void Update() {

        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }
        else                { panel.SetActive(true); }

        if (panel.activeSelf == true) {
            UIUtils.BalancePrefabs(slot.gameObject, player.potionbar.Length, content);
            for (int i = 0; i < content.childCount; i++) {
                var itemIndex = player.potionbar[i];

                if (itemIndex != -1) {
                    var item = player.inventory[itemIndex];
                    var slot = content.GetChild(i).GetComponent<UIInventorySlot>();

                    if (item.valid == true) {
                        int tempI = i;
                        slot.button.onClick.SetListener(() => {
                            player.CmdUseInventoryItem(itemIndex);
                        });

                        slot.icon.color = Color.white;
                        slot.icon.sprite = item.icon;
                        slot.overlay.SetActive(item.amount > 1);
                        slot.overlayValue.text = item.amount.ToString();
                        slot.dragAndDropable.dragable = false;
                        slot.dragAndDropable.dropable = false;
                    } else {
                        slot.button.interactable = false;
                    }
                } else {
                    slot.button.onClick.RemoveAllListeners();
                    slot.icon.color     = Color.clear;
                    slot.icon.sprite    = null;
                    slot.overlay.SetActive(false);
                }
            }
        }
    }
}
