
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public abstract class Entity : MonoBehaviour {

    [Header("General")]
    public Animator     animator    = null;
    public Rigidbody    rigidbody   = null;
    public Collider     collider    = null;

    public string state = "IDLE";

    public float moveSpeed = 3f;
    public float jumpForce = 7f;

    public bool invincible = false;

    public float     hitDistance = 0.35f;
    public LayerMask groundLayers;

    public abstract int healthMax { get; }
    private int _health = 3000;
    public int health {
        get { return Mathf.Min(_health, healthMax); }
        set { _health = Mathf.Clamp(value, 0, healthMax); }
    }

    public abstract int manaMax { get; }
    private int _mana = 10;
    public int mana {
        get { return Mathf.Min(_mana, manaMax); }
        set { _mana = Mathf.Clamp(value, 0, manaMax); }
    }

    public abstract int strikingPower { get; }  // 공격력
    public abstract int defensivePower { get; } // 방어력

    private Entity _enemy = null;
    public Entity enemy {

        get { return _enemy != null ? _enemy : null; }
        set {

            if (!value) _enemy = null;
            else        _enemy = value.GetComponent<Entity>() != null ? value.GetComponent<Entity>() : null;
        }
    }

    public bool grounded = false;
    public bool canAttack = true;

    protected abstract string UpdateState();
    protected abstract void   UpdateHandle();

    protected virtual void Awake() {

        collider  = GetComponent<Collider>();
        if (collider == null) collider = GetComponentInChildren<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }
    protected virtual void FixedUpdate() {

        UpdateHandle();
        state = UpdateState();
    }

    #region Skill Area
    public TemplateSkill[] defaultSkills;
    public SkillList skills = new SkillList();
    protected int currentSkill = -1;

    public GameObject damagePopupPrefab = null;
    private enum PopupType { Normal, Block, Critical }
    private void ShowDamagePopup(GameObject damageReceiver, PopupType popupType, int amount) {
        if (damageReceiver != null) {

            Entity receiverEntity = damageReceiver.GetComponent<Entity>();
            if (receiverEntity != null && receiverEntity.damagePopupPrefab != null) {

                var     bounds      = receiverEntity.collider.bounds;
                Vector3 position    = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

                var popup = Instantiate(receiverEntity.damagePopupPrefab, position, Quaternion.identity);
                switch(popupType) {

                    case PopupType.Normal:
                        popup.GetComponentInChildren<TextMesh>().text = amount.ToString();
                        break;

                    case PopupType.Block:
                        popup.GetComponentInChildren<TextMesh>().text = "<i>Block!</i>";
                        break;

                    case PopupType.Critical:
                        popup.GetComponentInChildren<TextMesh>().text = "Critical!\n" + amount;
                        break;
                }
            }
        }
    }

    public virtual HashSet<Entity> DealDamageAt(Entity entity, int amount, float aoeRadius = 0) {

        var entities = new HashSet<Entity>();

        entities.Add(entity);
        Collider[] colliders = Physics.OverlapSphere(entity.transform.position, aoeRadius);
        Debug.Log(colliders);
        foreach (Collider collider in colliders) {

            var candidate = collider.GetComponentInParent<Entity>();
            if (candidate == null) continue;

            if (candidate != this       &&
                CanAttack(candidate)    &&
                Vector3.Distance(entity.transform.position, candidate.transform.position) < aoeRadius)
                entities.Add(candidate);

            if (candidate == this) entities.Remove(this);
        }

        foreach (Entity ett in entities) {
            
            int damageDealt = 0;
            var popupType   = PopupType.Normal;

            if (!ett.invincible) {

                // 회피 
                if (Random.value < 0.1f) {
                    popupType = PopupType.Block;
                } else {

                    damageDealt = Mathf.Max(amount - ett.defensivePower, 1);

                    // 크리티컬
                    if (Random.value <= 0.2f) {
                        damageDealt *= 2;
                        popupType = PopupType.Critical;
                    }

                    ett.health -= damageDealt;
                }
            }

            ShowDamagePopup(ett.gameObject, popupType, damageDealt);
            ett.OnAggro(this);
        }

        return entities;
    }

    protected virtual bool IsMoving() {
        return  Input.GetAxis("Horizontal") != 0f ||
                Input.GetAxis("Vertical")   != 0f;
    }
    public void LookAtY(Vector3 position) {
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
    }
    public abstract bool CanAttack(Entity entity);
    public virtual void OnAggro(Entity entity) { }
    public virtual bool CastCheckSelf(Skill skill) {

        return  skill.IsReady() &&      // 스킬 재사용 시간 확인
                health > 0 &&           // 살아있는지 확인
                mana >= skill.manaCost; // 마나 잔여 확인
    }
    public virtual bool CastCheckTarget(Skill skill) {

        switch (skill.category) {

            case "Attack":
                if (skill.castRange == 0) return true;
                else {
                    return enemy != null && CanAttack(enemy);
                }

            case "Heal":
                return false;

            case "Buff":
                return health > 0;

            default:
                Debug.Log("not exist : " + skill.category);
                return false;
        }
    }
    public virtual bool CastCheckDistance(Skill skill) {

        switch (skill.category) {

            case "Attack":
                if (skill.castRange == 0) return true;
                else {
                    return enemy != null && Utils.ClosestDistance(collider, enemy.collider) <= skill.castRange;
                }

            case "Heal":
                return true;

            case "Buff":
                return health > 0;

            default:
                Debug.Log("not exist : " + skill.category);
                return false;
        }
    }
    public virtual void CastSkill(Skill skill) {
        /* Tool Tip
         * 스킬 모션 종료 후 발동
         */

        if (CastCheckSelf(skill) &&
            CastCheckTarget(skill)) {

            // effect 없거나 visual만 있는 경우, (데미지, 힐, 버프 등) 
            if ((skill.startedEffectPrefab  == null || skill.startedEffectPrefab.isPurelyVisual  == true) && 
                (skill.finishedEffectPrefab == null || skill.finishedEffectPrefab.isPurelyVisual == true)) {

            switch (skill.category) {

                case "Attack":
                    DealDamageAt(enemy, strikingPower + skill.damage, skill.aoeRadius);
                    break;

                case "Heal":
                    health += skill.healHealth;
                    mana   += skill.healMana;
                    break;

                case "Buff":
                    skill.buffTimeEnd = Time.time + skill.buffTime;
                    break;
                }
        }

        SpawnEndSkillEffect(currentSkill, enemy);

        mana                -= skill.manaCost;
        skill.coolTimeEnd    = Time.time + skill.coolTime;
        skills[currentSkill] = skill;

        } else {
            currentSkill = -1;
        }
    }
    public void SpawnStartSkillEffect (int skillIndex, Entity effectTarget) {

        var skill = skills[currentSkill];
        if (skill.startedEffectPrefab != null) {

            var mount       = transform.FindRecursively("EffectMount");
            var position    = mount != null ? mount.position : transform.position;
            var go          = Instantiate(skill.startedEffectPrefab.gameObject, position, Quaternion.identity);
            var effect      = go.GetComponent<SkillEffect>();

            effect.target       = enemy;
            effect.caster       = this;
            effect.skillIndex   = skillIndex;
        }
    }
    public void SpawnEndSkillEffect (int skillIndex, Entity effectTarget) {

        var skill = skills[currentSkill];
        if (skill.finishedEffectPrefab != null) {
            var mount       = transform.FindRecursively("EffectMount");
            var position    = mount != null ? mount.position : transform.position;
            var go          = Instantiate(skill.finishedEffectPrefab.gameObject, position, Quaternion.identity);
            var effect      = go.GetComponent<SkillEffect>();

            effect.target       = enemy;
            effect.caster       = this;
            effect.skillIndex   = skillIndex;
        }
    }
    #endregion

    public int GetSkillIndexByName(string name) { return skills.FindIndex(skill => skill.learned && skill.name == name); }

    public void StopBuffs() {
        for (int i = 0; i < skills.Count; i++) {
            if (skills[i].category == "Buff") {
                var skill               = skills[i];
                    skill.buffTimeEnd   = Time.time;
                    skills[i]           = skill;
            }
        }
    }
}
