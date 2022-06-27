
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextMeshCopyText : MonoBehaviour {

    public TextMesh source = null;
    private void Update() { GetComponent<TextMesh>().text = source.text; }
}
