using UnityEngine;
using UnityEngine.EventSystems;

public enum JoystickMode { AllAxis, Horizontal, Vertical }

public class UIJoyStick : MonoBehaviour {

    public GameObject panel = null;                         // JoyStick BackGround
    [SerializeField] private RectTransform stick = null;    // JoyStick Handle
    [Range(0f, 2f)] public float handleLimit = 1f;          // JoYstick Handle Can Move Maximum Radius

    private float radius = 0f;                  // JoyStick BackGround Radius
    private Vector3 stickOrigin = Vector3.zero; // JoyStick Handle Origin Position

    public JoystickMode joystickMode = JoystickMode.AllAxis;    // JoyStick Mode
    public Vector3 axis = Vector3.zero;                         // JoyStick Direction Vector
    public float fPower = 0f;                                   // JoyStick Stick Power
    private void Start() {
       // stick = transform.Find("Stick").GetComponent<RectTransform>();
        radius = (panel.GetComponent<RectTransform>().sizeDelta.x / 2f) * FindObjectOfType<Canvas>().transform.localScale.x;
        stickOrigin = stick.position;
    }

    private void Update() {

        var player = FindObjectOfType<Player>();
        if (player == null) { panel.SetActive(false); return; }
        else panel.SetActive(true);

        if (panel.activeSelf == true) {

        }
    }

    public void DragTouch(BaseEventData BED) {

        PointerEventData PED        = (PointerEventData)BED;
        Vector2          direction  = PED.position - (Vector2)stickOrigin;
        var              sizeDeltaX = panel.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
                         axis       = (direction.magnitude > sizeDeltaX) ? direction.normalized : direction / sizeDeltaX;
                         ClampJoyStick();
                         stick.anchoredPosition = (axis * sizeDeltaX) * handleLimit;

        float   fDistance   = Vector3.Distance(PED.position, stickOrigin);
        var     tempRadius  = radius / handleLimit;
                fPower      = Mathf.Clamp(fDistance / tempRadius, 0f, handleLimit);

        /*
        // 객체 이동 세기
        PointerEventData PED = (PointerEventData)BED;
        Vector3 pTemp = PED.position;
        float fDistance = Vector3.Distance(pTemp, stickOrigin); // 스칼라

        // 객체의 방향
        axis = (pTemp - stickOrigin).normalized;

        fPower = Mathf.Clamp(fDistance / radius, 0f, 1f);   // 스칼라

        // stick이 pad를 벗어나지 못하게 처리
        if(fDistance > radius) {
            stick.position = stickOrigin + axis * radius;
        } else {
            stick.position = pTemp;
        }
        */
    }

    public void EndTouch(BaseEventData BED) {
        stick.position = stickOrigin;
        axis = Vector3.zero;
        fPower = 0f;
    }
    
    private void ClampJoyStick() {
        if (joystickMode == JoystickMode.Horizontal) {
            axis.x = 0f;
        } else 
        if (joystickMode == JoystickMode.Vertical) {
            axis.y = 0f;
        }
    }

    public float Horizontal { get { return axis.x; } }
    public float Vertical   { get { return axis.y; } }
}