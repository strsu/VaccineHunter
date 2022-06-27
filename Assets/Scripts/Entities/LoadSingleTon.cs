using UnityEngine;
using System.Linq;

public class LoadSingleTon : MonoBehaviour {

    [SerializeField] private Transform defaulter = null;
    [SerializeField] private string targetTag = "Player";
    private Player pl;
    private CameraController cc;

    [System.Serializable]
    private class Socket
    {
        public Transform start = null;
    }

    private void Start()
    {
        pl = FindObjectOfType<Player>();
        pl.transform.position = transform.position;

        cc = FindObjectOfType<CameraController>();
        cc.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
}
