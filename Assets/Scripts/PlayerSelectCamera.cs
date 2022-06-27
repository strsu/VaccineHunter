using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectCamera : MonoBehaviour {

    public Camera camera = null;

    private Vector3 arissaPo = new Vector3(-0.7684889f, 4.245045f, -0.07632095f);
    private Vector3 arissaRo = new Vector3(40.565f, -77.177f, 0f);

    public int[] characterId;

    void Start() {
        camera.transform.position = new Vector3(-camera.transform.position.x, -camera.transform.position.y, -camera.transform.position.z);
        camera.transform.Rotate(new Vector3(-camera.transform.rotation.x, -camera.transform.rotation.y, -camera.transform.rotation.z));
        camera.transform.position = arissaPo;
        camera.transform.Rotate(arissaRo);
    }
}
