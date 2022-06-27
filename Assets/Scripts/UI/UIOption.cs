using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour {

    public GameObject panel = null;
    public Button btnSave = null;
    public Button btnCancle = null;

    public Slider bgm = null;
    public Slider efm = null;
    public Slider ex = null;

    void Start () {
        var player = FindObjectOfType<Player>();
        bgm.value = SingleTon.st.exSM.bgmVolume;
        efm.value = SingleTon.st.exSM.efmVolume;
         ex.value = player.experience / 100f;
        btnSave.onClick.AddListener(() => {
            SingleTon.st.exSM.bgmVolume = bgm.value;
            SingleTon.st.exSM.efmVolume = efm.value;
            if (player == null) { panel.SetActive(false); return; }
            else { player.experience = ex.value*100; }
            panel.SetActive(false);
        });
        btnCancle.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }
	
	void Update () {
        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }
    }
}
