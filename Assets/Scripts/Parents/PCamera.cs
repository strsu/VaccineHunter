using UnityEngine;

public abstract class PCamera : MonoBehaviour {

    protected bool isActived = true;
    public void SetActive(bool _active) { isActived = _active; }
    public bool GetActive() { return isActived; }

    protected Transform target = null;
    public void SetTarget(Transform _target) { target = _target; }

    public abstract void Move();
}
