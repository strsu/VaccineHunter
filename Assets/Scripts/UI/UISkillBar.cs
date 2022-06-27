
using UnityEngine;

using UnityEngine.UI;

public class UISkillBar : MonoBehaviour {

    public GameObject panel = null;

    public UISkillBarSlot slot = null;
    public Transform content = null;

    public Button btnChangeSlot = null;
    public UISkillBarSlot normalAttackSlot = null;

    public Text txtQuickNumber = null;

    private bool changed = false;

    private void Awake() {
        
        btnChangeSlot.onClick.AddListener(() => {
            changed = !changed;
        });

        panel.SetActive(false);
    }

    private void Update() {

        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }
        else                { panel.SetActive(true); }

        if (panel.activeSelf == true) {

            for (int i = content.childCount; i < player.SkillBarPoint.Length; i++) {
                var go = Instantiate(slot);
                go.transform.SetParent(content);
                go.transform.localPosition  = player.SkillBarPoint[i];
                go.transform.localScale     = Vector3.one;
            }

            for (int i = content.childCount - 1; i >= player.SkillBarPoint.Length; i--) {
                Destroy(content.GetChild(i).gameObject);
            }

            var norSkill                     = player.skills[0];
                normalAttackSlot.icon.color  = Color.white;
                normalAttackSlot.icon.sprite = norSkill.icon;

            normalAttackSlot.button.onClick.SetListener(() => {
                if (norSkill.learned         == true &&
                    norSkill.IsReady()       == true &&
                    UIUtils.AnyInputActive() == false) {
                    player.CmdUseSkill(0);
                }
            });

            float   norCT = norSkill.CooldownRemaining() / norSkill.coolTime;
                    normalAttackSlot.coolTimeOverlay.SetActive(norCT > 0f);
                    normalAttackSlot.coolTimeValue.fillAmount = norCT;

            for (int i = 0; i < content.childCount; i++) {

                var barSlot                          = content.GetChild(i).GetComponent<UISkillBarSlot>();
                    barSlot.dragAndDropable.name     = i.ToString();
                    barSlot.dragAndDropable.dragable = false;
                    barSlot.dragAndDropable.dropable = false;
                txtQuickNumber.text                  = changed == false ? "Q1" : "Q2";
                int offset                           = changed == false ? 0 : (player.skillbar.Length / 2);
                int skillIndex                       = player.GetSkillIndexByName(player.skillbar[offset + i]);
                if (skillIndex != -1) {

                    var skill = player.skills[skillIndex];
                    barSlot.button.onClick.SetListener(() => {

                        if (skill.learned   == true &&
                            skill.IsReady() == true &&
                            UIUtils.AnyInputActive() == false) {
                            player.CmdUseSkill(skillIndex);
                        }
                    });

                    barSlot.icon.color     = Color.white;
                    barSlot.icon.sprite    = skill.icon;

                    float cd = skill.CooldownRemaining() / skill.coolTime;
                    barSlot.coolTimeOverlay.SetActive(cd > 0f);
                    barSlot.coolTimeValue.fillAmount = cd;
                } else {

                    barSlot.button.onClick.RemoveAllListeners();

                    barSlot.icon.color     = Color.clear;
                    barSlot.icon.sprite    = null;
                    barSlot.coolTimeOverlay.SetActive(false); 
                }
            }
        }
    }
}
