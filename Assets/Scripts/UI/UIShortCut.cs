
using UnityEngine;

using UnityEngine.UI;

public class UIShortCut : MonoBehaviour {

    public GameObject   panel       = null;

    public Button       btnMenu     = null;
    public GameObject   goMenu      = null;

    public Button       btnSkill    = null;
    public GameObject   goSkill     = null;

    public Button       btnInven    = null;
    public GameObject   goIven      = null;

    private void Awake() {

        goMenu.SetActive(false);
        btnMenu.onClick.AddListener(() => {
            goMenu.SetActive(!goMenu.activeSelf);
        });

        goSkill.SetActive(false);
        btnSkill.onClick.AddListener(() => {
            goSkill.SetActive(true);
        });

        goIven.SetActive(false);
        btnInven.onClick.AddListener(() => {
            goIven.SetActive(true);
        });

        panel.SetActive(false);
    }

    private void Update() {

        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }
        else                { panel.SetActive(true); }
    }
}
