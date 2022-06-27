
using UnityEngine;

using UnityEngine.UI;

public class UISkill : MonoBehaviour {

    public GameObject panel = null;
    public Transform content = null; // 슬롯이 생길 부모 객체
    public UISkillSlot slotPrefab = null;

    public Player player = null; // TODO: 나중에 삭제 (테스트용)

    public void Awake() {
        panel.SetActive(false);
    }

    public void Update() {
        // TODO: Player 흭득
        // 정책을 정해야함

        if (player == null) { panel.SetActive(false); return; }

        var pl = player.GetComponent<Player>();
        if (pl == null) { panel.SetActive(false); return; }

        if (InputSystem.GetKeyDown("Skill")) { panel.SetActive(!panel.activeSelf); }

        if (panel.activeSelf == true) {
            UIUtils.BalancePrefabs(slotPrefab.gameObject, player.skills.Count, content);
            for (int i = 0; i < player.skills.Count; i++) {

                var slot        = content.GetChild(i).GetComponent<UISkillSlot>();
                var skill       = player.skills[i];

                int skillIndex  = player.skills.FindIndex(ski => ski.name == skill.name);

                slot.btnIcon.onClick.SetListener(() => {
                    if (skill.learned && skill.IsReady()) {
                        player.CmdUseSkill(skillIndex);
                    }
                });

                slot.skillIcon.color = Color.white;
                slot.skillIcon.sprite = skill.icon;
                slot.discription.text = skill.ToolTip();

                if (skill.learned == false) {

                    slot.btnLearn.gameObject.SetActive(true);
                    slot.btnLearn.GetComponentInChildren<Text>().text = "Learn";
                    slot.btnLearn.interactable = skill.nowSkillPoint >= 1;
                    slot.btnLearn.onClick.SetListener(() => {
                        player.CmdLearnSkill(skillIndex);
                    });
                } else 
                if (skill.level < skill.maxLevel) {

                    slot.btnLearn.gameObject.SetActive(true);
                    slot.btnLearn.GetComponentInChildren<Text>().text = "Upgrade";
                    slot.btnLearn.interactable = skill.nowSkillPoint >= skill.requiredSkillPoint;
                    slot.btnLearn.onClick.SetListener(() => {
                        player.CmdUpgradeSkill(skillIndex);
                    });
                } else {
                    slot.btnLearn.gameObject.SetActive(false);
                }
            }
        }
    }
}
