using UnityEngine;

using UnityEngine.UI;
public class rotator : MonoBehaviour {

	public float speed;
    public GameObject panel = null;
    public Text t = null;
    private bool tc = false;
    public Vector3 direction = Vector3.zero;// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(direction * Time.deltaTime * speed, Space.World);
        var player = FindObjectOfType<Player>();
        if (tc == false && player && t && player.isMontaDead) {
            t.text = "감사합니다! 용사님!\n덕분에 몬스터 바이러스로 부터 해방되었습니다.";
            tc = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player") == true) {
            panel.SetActive(true);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag.Equals("Player") == true) {
            panel.SetActive(false);
        }
    }
}
