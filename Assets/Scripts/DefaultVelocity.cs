
using UnityEngine;

public class DefaultVelocity : MonoBehaviour {

    public Vector3 velocity;
    private void Start() { GetComponent<Rigidbody>().velocity = velocity; }
}
