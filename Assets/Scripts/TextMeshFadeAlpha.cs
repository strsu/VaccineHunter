
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextMeshFadeAlpha : MonoBehaviour {

    public  float delay     = 0f;
    public  float duration  = 1f;
    private float perSecond = 0f;
    private float startTime = 0f;

    private void Start() {
        perSecond = GetComponent<TextMesh>().color.a / duration;
        startTime = Time.time + delay;
    }
    private void Update() {
        if (Time.time >= startTime) {
            var col                             = GetComponent<TextMesh>().color;
                col.a                          -= perSecond * Time.deltaTime;
                GetComponent<TextMesh>().color  = col;
        }
    }
}
