using UnityEngine;
using System.Collections;

public class ShakyCam : MonoBehaviour {

    Camera _camera;

    //public float shakeSpeed = 1;
    //public Vector3 shakeAmount;

    public float frequency = 3;
    public float amplitudeY = 0.5f;
    public float amplitudeX = 0.35f;
    public float moveSpeed = 1f;


    private int currentTime = 0;
    private int octaves = 50;
    private Vector3 destination = Vector3.zero;
    private Vector3 transformTmp = Vector3.zero;
    private Vector3 vel = Vector3.one;


    // Use this for initialization
    void Start () {
        _camera = GetComponent<Camera>();
        octaves = (int)((1f / frequency) * 50f);
    }


    void FixedUpdate()
    {
        // if number of frames played since last change of direction > octaves create a new destination
        if (currentTime > octaves)
        {
            currentTime = 0;

            //destination = generateRandomVector(amplitude);
            destination = new Vector3(Random.Range(-amplitudeX, amplitudeX), Random.Range(-amplitudeY, amplitudeY), 0);
        }

        // smoothly moves the object to the random destination
        transformTmp = Vector3.SmoothDamp(transformTmp, destination, ref vel, moveSpeed);


        _camera.transform.localPosition = new Vector3(transformTmp.x, transformTmp.y, 0);

        currentTime++;
    }

    //// Update is called once per frame
    //void Update () {
    //    Vector3 rnd = new Vector3(Random.Range(-shakeAmount.x, shakeAmount.x), Random.Range(-shakeAmount.y, shakeAmount.y), Random.Range(-shakeAmount.z, shakeAmount.z));
    //    _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, rnd, Time.deltaTime * shakeSpeed );
    //}

}


