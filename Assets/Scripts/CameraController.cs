using UnityEngine;

public class CameraController : PCamera {

    public bool x, y, z = false;    // target의 position 중 추적할 좌표

    public override void Move() {
        var player = FindObjectOfType<Player>();
        if (player == null) { return; }

        target = player.transform;

        transform.position = new Vector3(
            (x ? target.position.x : transform.position.x),
            (y ? target.position.y : transform.position.y),
            (z ? target.position.z : transform.position.z));
    }
    private void Update() {
        Move();
    }
}
