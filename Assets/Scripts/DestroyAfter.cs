
using UnityEngine;

public class DestroyAfter : MonoBehaviour {

    public float time = 1;
    private void Start() { Destroy(gameObject, time); }
}
