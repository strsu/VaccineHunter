/*
 * 로딩객체가 캔버스에 올라가는 객체라서 호출하는 씬은 캔버스가 꼭 필요하다.
 * 
 */
using UnityEngine;

using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour {

    public UILoading loading = null;
    public void Making(string strNextSceneName) {   // UISceneLoader 객체를 찾고 Making을 호출하면 씬이 넘어간다.
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) {
            GameObject  GO                          = new GameObject();
            var         cv                          = GO.AddComponent<Canvas>();
                        cv.renderMode               = RenderMode.ScreenSpaceOverlay;
            var         scaler                      = GO.AddComponent<CanvasScaler>();
                        scaler.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        scaler.referenceResolution  = new Vector2(1920f, 1080f);
                        GO.AddComponent<GraphicRaycaster>();

            canvas = GO.GetComponent<Canvas>();
        }

        GameObject  go = Instantiate(loading.gameObject);
                    go.transform.SetParent(canvas.transform);
        var         rectTransform               = go.GetComponent<RectTransform>();
                    rectTransform.anchorMin     = new Vector2(0f, 0f);
                    rectTransform.anchorMax     = new Vector2(1f, 1f);
                    rectTransform.sizeDelta     = new Vector2(0f, 0f);
                    go.transform.localPosition  = Vector3.zero;
                    go.transform.localScale     = Vector3.one;
                    go.GetComponent<UILoading>().Load(strNextSceneName);
    }
}