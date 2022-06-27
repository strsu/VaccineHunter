
using UnityEngine;

using UnityEngine.UI;

public class UIInventory : MonoBehaviour {

    public  GameObject      panel   = null;
    public  Transform       content = null;
    public  UIInventorySlot slot    = null;

    public  Text    txtGold = null;
    public  Button  btnBack = null;

    public Button btnUse = null;
    public GameObject potionQuickPanel = null;
    public GameObject descriptionViewer = null;

    private int             descriptionIndex = -1;
    public  UIInventorySlot descriptionSlot  = null;
    public  Text            descriptionText  = null;
    private void Awake() {
        panel.SetActive(false);
        btnBack.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }
    private void Update() {
        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }

        if (InputSystem.GetKeyDown("Inventory")) {
            panel.SetActive(!panel.activeSelf);
        }

        if (panel.activeSelf == true) {
            UIUtils.BalancePrefabs(slot.gameObject, player.inventory.Count, content);
            for (int i = 0; i < player.inventory.Count; i++) {
                var item = player.inventory[i];
                var slot = content.GetChild(i).GetComponent<UIInventorySlot>();

                txtGold.text                    = "GOLD : " + player.gold.ToString();
                slot.dragAndDropable.name       = i.ToString();
                slot.dragAndDropable.dropable   = true;

                if (item.valid == true) {
                    int tempI = i;
                    slot.button.onClick.SetListener(() => {
                        descriptionIndex = tempI;
                    });

                    slot.icon.color         = Color.white;
                    slot.icon.sprite        = item.icon;
                    slot.overlay.SetActive(item.amount > 1);
                    slot.overlayValue.text  = item.amount.ToString();
                    slot.dragAndDropable.dragable = true;
                } else {
                    slot.button.onClick.RemoveAllListeners();
                    slot.icon.color               = Color.clear;
                    slot.icon.sprite              = null;
                    slot.overlay.SetActive(false);
                    slot.dragAndDropable.dragable = false;
                }
            }

            if (descriptionIndex != -1) {
                var item = player.inventory[descriptionIndex];

                if (item.valid == true) {
                    descriptionViewer.SetActive(true);
                    descriptionSlot.icon.sprite     = item.icon;
                    descriptionSlot.icon.color      = Color.white;
                    descriptionSlot.button.enabled  = false;
                    descriptionText.text            = item.ToolTip();

                    if (item.category == "Skillbook") {
                        potionQuickPanel.SetActive(false);
                        btnUse.gameObject.SetActive(true);
                        btnUse.onClick.SetListener(() => {
                            player.CmdUseInventoryItem(descriptionIndex);
                        });
                    } else
                    if (item.category == "Potion") {
                        potionQuickPanel.SetActive(true);
                        btnUse.gameObject.SetActive(false);
                        btnUse.onClick.RemoveAllListeners();
                    }
                } else {
                    descriptionSlot.icon.sprite = null;
                    descriptionSlot.icon.color  = Color.clear;
                    descriptionText.text        = null;
                    descriptionIndex            = -1;

                    descriptionViewer.SetActive(false);
                    potionQuickPanel.SetActive(false);
                    btnUse.gameObject.SetActive(false);
                    btnUse.onClick.RemoveAllListeners();
                }
            }
        } else {
            descriptionSlot.icon.color  = Color.clear;
            descriptionText.text        = null;
            descriptionIndex            = -1;

            descriptionViewer.SetActive(false);
            potionQuickPanel.SetActive(false);
            btnUse.gameObject.SetActive(false);
            btnUse.onClick.RemoveAllListeners();
        }
    }
}
