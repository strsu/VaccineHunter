using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour {
    public void startGame() {
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
