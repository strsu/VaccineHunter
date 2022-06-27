
using UnityEngine;

using UnityEngine.UI;

public class UISetSkill : MonoBehaviour {

    public GameObject       panel           = null;

    public UISetSkillSlot   slot            = null;
    public Transform        setSkillContent = null;

    public Button           btnBack         = null;

    private void Awake() {

        panel.SetActive(false);

        btnBack.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }

    private void Update() {
        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }

        if (panel.activeSelf == true) {
            var skills = player.skills;
            UIUtils.BalancePrefabs(slot.gameObject, skills.Count, setSkillContent);

            for (int i = 0; i < skills.Count; i++) {
                var sl          = setSkillContent.GetChild(i).GetComponent<UISetSkillSlot>();
                var skill       = skills[i];
                int skillIndex  = player.skills.FindIndex(sk => sk.name == skill.name);

                sl.icon.color               = Color.white;
                sl.icon.sprite              = skill.icon;
                sl.name.text                = skill.name;
                sl.dragAndDropable.name     = i.ToString();
                sl.dragAndDropable.dragable = true;
                sl.dragAndDropable.dropable = true;

                if      (skill.learned  == false)           sl.level.text = "lv. 0";
                else if (skill.level     < skill.maxLevel)  sl.level.text = "lv. " + skill.level;
                else if (skill.level    >= skill.maxLevel)  sl.level.text = "lv. max";

                if (skill.learned == false) {
                    slot.btnLearn.gameObject.SetActive(true);
                    slot.btnLearn.GetComponentInChildren<Text>().text = "배우기";
                    slot.btnLearn.interactable = skill.nowSkillPoint >= 1;
                    slot.btnLearn.onClick.SetListener(() => {
                        player.CmdLearnSkill(skillIndex);
                    });
                } else 
                if (skill.level < skill.maxLevel) {
                    slot.btnLearn.gameObject.SetActive(true);
                    slot.btnLearn.GetComponentInChildren<Text>().text = "스킬업";
                    slot.btnLearn.interactable = skill.nowSkillPoint >= skill.requiredSkillPoint;
                    slot.btnLearn.onClick.SetListener(() => {
                        player.CmdUpgradeSkill(skillIndex);
                    });
                } else
                if (skill.level >= skill.maxLevel) {
                    slot.btnLearn.gameObject.SetActive(false);
                }
            }
        }
    }
}
