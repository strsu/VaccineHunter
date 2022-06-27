using UnityEngine;

public class Village : MonoBehaviour {

    public AudioClip[] bgm;
    private bool bgmChang = false;

	void Update () {
        var player = FindObjectOfType<Player>();
        if(player) {
            if (player.isMontaDead == true && bgmChang == false) {
                GetComponent<AudioSource>().clip = bgm[1];
                GetComponent<AudioSource>().Play();
                bgmChang = true;
            } else {
                //GetComponent<AudioSource>().clip = bgm[0];
                //GetComponent<AudioSource>().Play();
            }
        }
    }
}
