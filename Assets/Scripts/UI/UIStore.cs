
using UnityEngine;

using UnityEngine.UI;

public class UIStore : MonoBehaviour
{

    public GameObject panel = null;
    public Transform content_player = null;
    public Transform content_store = null;
    public UIInventorySlot slot = null;

    public Text txtGold = null;
    public Button btnBack = null;
    public Slider slider = null;

    public Button btnBuy = null;
    public Button btnSel = null;
    public GameObject descriptionViewer = null;

    public int inventorySize = 48;
    public ItemList store = new ItemList();
    public TemplateItem[] defaultItems = null;

    private int descriptionIndex = -1;
    private int descriptionIndexforStore = -1;
    public UIInventorySlot descriptionSlot = null;
    public Text descriptionText = null;
    public Text count           = null;

    private bool flag = true;

    private void Awake()
    {
        panel.SetActive(false);
        btnBack.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }

    private void start() {
        
    }

    private void Update()
    {
        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }

        if (InputSystem.GetKeyDown("Inventory"))
        {
            panel.SetActive(!panel.activeSelf);
        }
        if(flag) {
            for (int i = 0; i < inventorySize; i++)
            {
                // default item이 존재하는 경우
                if (i < defaultItems.Length)
                {
                    store.Add(new Item(defaultItems[i]));
                    var item = store[i];
                    item.amount = 9999;
                    store[i] = item;
                    continue;
                }

                store.Add(new Item());
            }
            flag = false;
        }
        if (panel.activeSelf == true)
        {
            int tmp = (int)(slider.value * 10);
            count.text = tmp.ToString() + "개";

            UIUtils.BalancePrefabs(slot.gameObject, player.inventory.Count, content_player);
            UIUtils.BalancePrefabs(slot.gameObject, inventorySize, content_store);
            /////////////// store
            for (int i = 0; i < store.Count; i++)
            {
                var item = store[i];
                var slot = content_store.GetChild(i).GetComponent<UIInventorySlot>();
                
                slot.dragAndDropable.name = i.ToString();
                slot.dragAndDropable.dropable = true;

                if (item.valid == true)
                {
                    int tempI = i;
                    slot.button.onClick.SetListener(() => {
                        descriptionIndexforStore = tempI;
                    });

                    slot.icon.color = Color.white;
                    slot.icon.sprite = item.icon;
                    slot.overlay.SetActive(item.amount > 1);
                    slot.overlayValue.text = item.amount.ToString();
                    slot.dragAndDropable.dragable = true;
                }
                else
                {
                    slot.button.onClick.RemoveAllListeners();
                    slot.icon.color = Color.clear;
                    slot.icon.sprite = null;
                    slot.overlay.SetActive(false);
                    slot.dragAndDropable.dragable = false;
                }
            } // end for

            if (descriptionIndexforStore != -1)
            {
                var item = store[descriptionIndexforStore];

                if (item.valid == true)
                {
                    descriptionViewer.SetActive(true);
                    descriptionSlot.icon.sprite = item.icon;
                    descriptionSlot.icon.color = Color.white;
                    descriptionSlot.button.enabled = false;
                    descriptionText.text = item.ToolTip();

                    if (item.category == "Skillbook")
                    {
                        btnBuy.gameObject.SetActive(true);
                        btnBuy.onClick.SetListener(() => {
                            player.CmdBuyInventoryItem(descriptionIndexforStore, 1, item.buyPrice);
                        });
                    }
                    else
                    if (item.category == "Potion")
                    {
                        btnBuy.gameObject.SetActive(true);
                        int potionNum = (int)(slider.value * 10);
                        Debug.Log(potionNum);
                        if (player.gold >= potionNum * item.buyPrice) {
                            btnBuy.onClick.SetListener(() => {
                                player.CmdBuyInventoryItem(descriptionIndexforStore, potionNum, item.buyPrice);
                            });
                        } else {
                            btnBuy.onClick.RemoveAllListeners();
                        }   
                    }
                }
                else
                {
                    descriptionSlot.icon.sprite = null;
                    descriptionSlot.icon.color = Color.clear;
                    descriptionText.text = null;
                    descriptionIndexforStore = -1;

                    descriptionViewer.SetActive(false);
                    btnBuy.gameObject.SetActive(false);
                    btnBuy.onClick.RemoveAllListeners();
                }
            }

            ///////////////////////플레이어
            for (int i = 0; i < player.inventory.Count; i++)
            {
                var item = player.inventory[i];
                var slot = content_player.GetChild(i).GetComponent<UIInventorySlot>();

                txtGold.text = "GOLD : " + player.gold.ToString();
                slot.dragAndDropable.name = i.ToString();
                slot.dragAndDropable.dropable = true;

                if (item.valid == true)
                {
                    int tempI = i;
                    slot.button.onClick.SetListener(() => {
                        descriptionIndex = tempI;
                    });

                    slot.icon.color = Color.white;
                    slot.icon.sprite = item.icon;
                    slot.overlay.SetActive(item.amount > 1);
                    slot.overlayValue.text = item.amount.ToString();
                    slot.dragAndDropable.dragable = true;
                }
                else
                {
                    slot.button.onClick.RemoveAllListeners();
                    slot.icon.color = Color.clear;
                    slot.icon.sprite = null;
                    slot.overlay.SetActive(false);
                    slot.dragAndDropable.dragable = false;
                }
            } // end for

            if (descriptionIndex != -1)
            {
                var item = player.inventory[descriptionIndex];

                if (item.valid == true)
                {
                    descriptionViewer.SetActive(true);
                    descriptionSlot.icon.sprite = item.icon;
                    descriptionSlot.icon.color = Color.white;
                    descriptionSlot.button.enabled = false;
                    descriptionText.text = item.ToolTip();

                    if (item.category == "Skillbook")
                    {
                        btnSel.gameObject.SetActive(true);
                        btnSel.onClick.SetListener(() => {
                            player.CmdSelInventoryItem(descriptionIndex, 1, item.sellPrice);
                        });
                    }
                    else
                    if (item.category == "Potion")
                    {
                        btnSel.gameObject.SetActive(true);
                        int potionNum = (int)(slider.value * 10);
                        if (item.amount <= potionNum) {
                            btnBuy.onClick.SetListener(() => {
                                player.CmdSelInventoryItem(descriptionIndexforStore, potionNum, item.sellPrice);
                            });
                        } else {
                            btnBuy.onClick.RemoveAllListeners();
                        }
                    }
                }
                else
                {
                    descriptionSlot.icon.sprite = null;
                    descriptionSlot.icon.color = Color.clear;
                    descriptionText.text = null;
                    descriptionIndex = -1;

                    descriptionViewer.SetActive(false);
                    btnSel.gameObject.SetActive(false);
                    btnSel.onClick.RemoveAllListeners();
                }
            }
        } // end - panel.activeSelf == true
        else
        {
            descriptionSlot.icon.color = Color.clear;
            descriptionText.text = null;
            descriptionIndex = -1;

            descriptionViewer.SetActive(false);
            btnSel.gameObject.SetActive(false);
            btnSel.onClick.RemoveAllListeners();
        }
    }
}
