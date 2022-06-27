
using UnityEngine;

public class AggroArea : MonoBehaviour {
    public Transform    root        = null;
    public string       strTarget   = "Player";
    public float        areaRange   = 5f;
    public Vector3      offset      = Vector3.zero;
    private void Update() {

        var colliders = Physics.OverlapSphere(transform.position + offset, areaRange);
        foreach (Collider collider in colliders) {
            if (collider.tag.Equals(strTarget)) {
                root.SendMessage("OnAggro", collider.GetComponent<Entity>());
                break;
            }
        }
    }
    private void OnDrawGizmos() {
        var startHelp       = transform.position;
            Gizmos.color    = Color.red;
            Gizmos.DrawWireSphere(transform.position + offset, areaRange);
    }
}
