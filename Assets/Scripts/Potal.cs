using UnityEngine;

public class Potal : MonoBehaviour {
    
    public UISceneLoader loader = null;
    public string SceneName = null;

    void Start() {
        if (SceneName == "Monta") {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void Update() {
        if (SceneName == "Monta") {
            var player = FindObjectOfType<Player>();
            if (player != null && (player.experience == player.experienceMax)) {
                gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag.Equals("Player") == true) {

            //SingleTon.st.temp = GameObject.FindGameObjectWithTag("Player");
            
            if (SceneName == "Monta") {
                var player = FindObjectOfType<Player>();
                if (player.experience == player.experienceMax) {
                    loader.Making(SceneName);
                }
            } else {
                loader.Making(SceneName);
            }
        }
        

    }
}
