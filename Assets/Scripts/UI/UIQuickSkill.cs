
using UnityEngine;

using UnityEngine.UI;

public class UIQuickSkill : MonoBehaviour {

    public GameObject   panel   = null;
    public Transform    content = null;

    public Button btnQuick01AllClear = null;
    public Button btnQuick02AllClear = null;

    private void Update() {

        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }

        var childCount = content.childCount;
        btnQuick01AllClear.onClick.SetListener(() => {
            player.CmdClearSkillBarSlot(new int[] { 0, ((childCount - 1) / 2) });
        });
        btnQuick02AllClear.onClick.SetListener(() => {
            player.CmdClearSkillBarSlot(new int[] { (childCount / 2), (childCount - 1) });
        });

        for (int i = 0; i < content.childCount; i++) {

            var slot                    = content.GetChild(i).GetComponent<UISkillBarSlot>();
            slot.dragAndDropable.name   = i.ToString();
            int skillIndex              = player.GetSkillIndexByName(player.skillbar[i]);

            if (skillIndex != -1) {

                var skill   = player.skills[skillIndex];
                var temp    = i;
                slot.button.onClick.SetListener(() => {
                    player.CmdClearSkillBarSlot(new int[]{ temp, temp });
                });

                slot.icon.color  = Color.white;
                slot.icon.sprite = skill.icon;
                slot.coolTimeOverlay.SetActive(false);
            } else {

                player.skillbar[i]  = "";
                slot.button.onClick.RemoveAllListeners();
                slot.icon.color     = Color.clear;
                slot.icon.sprite    = null;

                slot.coolTimeOverlay.SetActive(false);
            }
        }
    }
}
