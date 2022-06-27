using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class quest1 : MonoBehaviour {

    public GameObject panel = null;
    public GameObject target = null;
    public Button btnCancle = null;
    public TemplateItem defaultItems = null;
    private bool vac = false;
    private bool Receivevac = false;

    void Start () {
        btnCancle.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }
	
	// Update is called once per frame
	void Update () {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) { panel.SetActive(false); return; }
        else {
            if(panel.activeSelf == true && Receivevac == false) {
                vac = true;
                Receivevac = true;
                var players = FindObjectOfType<Player>();
                players.inventory.Add(new Item(defaultItems));
                players.isMontaDead = true;
                return;
            }
        }
    }
}
