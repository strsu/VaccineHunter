
using UnityEngine;

using UnityEngine.UI;

public class UIMenu : MonoBehaviour {

    public GameObject panel = null;
    public Button btnOption = null;
    public Button btnQuit = null;

    private void Start() {
        btnOption.onClick.AddListener(() => {
            var option = FindObjectOfType<UIOption>();
            option.panel.SetActive(true);
            panel.SetActive(false);
        });
        btnQuit.onClick.AddListener(() => {
            Application.Quit();
        });
    }

    private void Update() {
        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }


    }
}
