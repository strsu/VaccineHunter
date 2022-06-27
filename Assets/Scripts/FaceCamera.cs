
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public string camTag = "PlayerCamera";
    private void LateUpdate() {
        var cam = GameObject.FindGameObjectWithTag(camTag);
        if (cam != null) transform.forward = cam.transform.forward;
        else { cam = FindObjectOfType<Camera>().gameObject; }
    }

	private void Awake()                { enabled = true; }
    private void OnBecameVisible()      { enabled = true; }
    private void OnBecameInvisible()    { enabled = false; }
}
