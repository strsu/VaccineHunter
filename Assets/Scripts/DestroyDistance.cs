
using UnityEngine;

public class DestroyDistance : MonoBehaviour {

    public float limit = 10f;
    private Vector3 origin = Vector3.zero;
    private void Start() {
        origin = transform.position;
    }
    private void Update() {
        var distance = Vector3.Distance(origin, transform.position);
        if (distance > limit) {
            Destroy(gameObject);
        }
    }
}
