
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillEffectArrow : SkillEffect {

    public GameObject effectOnCollision = null;     // 충돌 했을 경우의 effect -- 피가 튄다, 이펙트 폭발이 일어난다 등

    public float speed = 10;            // 움직이는 속도
    public float colliderRadius = 1f;   // collider 생성시 radius
    private bool isCollided = false;    // 충돌 발생 여부
    private float arriveTime = 5f;      // 화살 생존시간

    public Vector3 colliderOffset = Vector3.zero;   // 위치 offset

    private SphereCollider spCollider = null;   // 생성한 collider

    private void Start() {

        var center = caster.transform.FindRecursively("WeaponCenter");
        if (center == null) {
            Debug.LogError(name + " -- SkillEffectArrow.cs : can't find center");
            Destroy(gameObject);
            return;
        }
        transform.position = center.position;
        transform.rotation = caster.transform.rotation;

        spCollider              = gameObject.AddComponent<SphereCollider>();
        spCollider.radius       = colliderRadius;
        spCollider.isTrigger    = true;
        spCollider.center       = colliderOffset;

        //GetCollision();
    }

    private void Update() {
        if (isCollided == true) {
            Destroy(gameObject);
            return;
        }
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // public void GetCollision();
    public void OnTriggerEnter(Collider collider) {

        if (isPurelyVisual == false &&
            collider.tag.Equals("Monster")) {

            int damage = caster.strikingPower;
            caster.DealDamageAt(collider.GetComponent<Entity>(), damage, sourceSkill.aoeRadius);
        }

        transform.SetParent(collider.transform);
        if (effectOnCollision != null) {
            GameObject  go                      = Instantiate(effectOnCollision, collider.transform);
                        go.transform.position   = transform.position;
        }

        isCollided          = true;
        spCollider.enabled  = false;
        if (GetComponent<Rigidbody>() != null) {
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
