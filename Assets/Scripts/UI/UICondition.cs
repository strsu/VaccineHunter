
using UnityEngine;

using UnityEngine.UI;

public class UICondition : MonoBehaviour {

    public GameObject   panel               = null;

    public Slider       sliderHealth        = null;
    public Text         textHealth          = null;

    public Slider       sliderMana          = null;
    public Text         textMana            = null;

    public Slider       sliderExperience    = null;
    public Text         textExperience      = null;

    private void Awake() {

        panel.SetActive(false);
    }

    private void Update() {

        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }
        else                { panel.SetActive(true); }

        sliderHealth.value  = player.health / (float)player.healthMax;
        textHealth.text     = player.health + " / " + player.healthMax;

        sliderMana.value    = player.mana   / (float)player.manaMax;
        textMana.text       = player.mana   + " / " + player.manaMax;

        sliderExperience.value = player.experience / (float)player.experienceMax;
        textExperience.text = player.experience + " / " + player.experienceMax;
    }
}
