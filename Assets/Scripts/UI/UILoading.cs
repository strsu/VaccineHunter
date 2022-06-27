
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.SceneManagement;

public class UILoading : MonoBehaviour {
    public Slider sliderGauge = null;
    public Image showImage;              // jj
    public Sprite[] image;               // jj

    public void Load(string strNextSceneName) {
        StartCoroutine(Loading(strNextSceneName));
    }

    IEnumerator Loading(string strNextSceneName) {
        AsyncOperation ao = SceneManager.LoadSceneAsync(strNextSceneName);
        ao.allowSceneActivation = false;
        showImage.sprite = image[Random.Range(0, 3)];          // jj
        while (true) {
            sliderGauge.value = ao.progress + 0.1f;
            if (sliderGauge.value >= 1f) {
                ao.allowSceneActivation = true;
                break;
            }          
            yield return null;
        }
    }
}
