
using UnityEngine;

using System;
using System.Linq;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public partial class Monster : Entity {

    [Header("Components")]
    public NavMeshAgent agent = null;

    [Header("Text Meshes")]
    public TextMesh nameOverlay = null;
    public TextMesh hpOverlay   = null;

    [Header("Gold")]
    [SerializeField] private long _gold = 0;
    public long gold {
        get { return _gold; }
        set { _gold = Math.Max(value, 0); }
    }

    [Header("Inventory")]
    public ItemList inventory = new ItemList();

    [Header("Health")]
    [SerializeField]
    private             int _healthMax = 1;
    public  override    int healthMax       { get { return _healthMax; } }

    [Header("Mana")]
    [SerializeField]
    private             int _manaMax = 0;
    public  override    int manaMax         { get { return _manaMax; } }

    [Header("Damage")]
    [SerializeField]
    private             int _strikingPower = 2;
    public  override    int strikingPower {
        get {
            return _strikingPower;
        }
    }

    [Header("Defense")]
    [SerializeField]
    private             int _defensivePower = 1;
    public  override    int defensivePower  { get { return _defensivePower; } }

    [Header("Movement")]
    [Range(0, 1)]
    public float moveProbability    = 0.1f;
    public float moveDistance       = 10f;
    public float followDistance     = 20f;

    [Header("Experience Reward")]
    public long rewardExperience        = 10;
    public long rewardSkillExperience   = 2;

    [Header("Loot")]
    public int              lootGoldMin = 0;
    public int              lootGoldMax = 10;
    public ItemDropChance[] dropChances = null;

    [Header("Respawn")]
    public  float deathTime      = 30f;
    private float deathTimeEnd;
    public  bool  destroyable    = false;   // jj
    public  bool  respawn        = true;
    public  float respawnTime    = 10f;
    private float respawnTimeEnd;

    private Vector3 startPosition;

    public bool haveHitting = false;

    private bool experienceFlag = true;

    // behavior//////////////////////////////////////////////////////////////////////////////////////////////////////////////
    protected override void Awake() {
        base.Awake();
        startPosition = transform.position;
    }
    private void Start() {

        health  = healthMax;
        mana    = manaMax;

        foreach (var template in defaultSkills) skills.Add(new Skill(template));
    }
    private void LateUpdate() {

        var player = FindObjectOfType<Player>();

        if(destroyable && health <= 0) {    // jj - 몬스터가 죽으면 파괴
            Destroy(this);
        }

        if(experienceFlag && health <= 0 && player.experience < player.experienceMax) {// jj - 몬스터가 죽으면 딱 한번만 숙련도를 올려줘야함    
            if(player.experienceMax - player.experience < 10) {
                player.experience = player.experienceMax;
            }
            player.experience += 10f;
            experienceFlag = false;
        }

        if(health > 0) {
            experienceFlag = true;
        }

        if (nameOverlay != null) {
            nameOverlay.gameObject.SetActive(true);
            nameOverlay.text = name;
        }
        if (hpOverlay != null) {
            hpOverlay.gameObject.SetActive(true);
            hpOverlay.text = health + " / " + healthMax;
        }

        animator.SetBool    ("MOVING"       , state == "MOVING" && agent.velocity != Vector3.zero);
        animator.SetBool    ("CASTING"      , state == "CASTING");
        animator.SetInteger ("currentSkill" , currentSkill);
        animator.SetBool    ("DEAD"         , state == "DEAD");
    }
    private void OnDrawGizmos() {
        var startHelp       = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color    = Color.yellow;
            Gizmos.DrawWireSphere(startHelp, moveDistance);

            Gizmos.color    = Color.gray;
            Gizmos.DrawWireSphere(startHelp, followDistance);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // 상태 기계 - event 정의 ////////////////////////////////////////////////////////////////////////////////////////////////
    private bool EventDied()                 { return health == 0; }
    private bool EventDeathTimeElapsed()     { return state == "DEAD" && Time.time >= deathTimeEnd; }
    private bool EventRespawnTimeElapsed()   { return state == "DEAD" && respawn && Time.time >= respawnTimeEnd; }
    private bool EventTargetDisappeared()    { return enemy == null; }
    private bool EventTargetDied()           { return enemy != null && enemy.health == 0; }
    private bool EventTargetTooFarToAttack() {
        return  enemy != null &&
                currentSkill >= 0 && 
                currentSkill <  skills.Count &&
                !CastCheckDistance(skills[currentSkill]);
    }
    private bool EventTargetTooFarToFollow() {
        return  enemy != null &&
                Vector3.Distance(startPosition, enemy.collider.ClosestPointOnBounds(transform.position)) > followDistance;
    }
    private bool EventAggro()               { return enemy != null && enemy.health > 0; }
    private bool EventSkillRequest()        { return currentSkill >= 0 && currentSkill < skills.Count; }
    private bool EventSkillFinished() {
        return  currentSkill >= 0 &&
                currentSkill <  skills.Count &&
                skills[currentSkill].CastTimeRemaining() <= 0f;
    }
    private bool EventMoveEnd()             { return state == "MOVING" && !IsMoving(); }
    private bool EventMoveRandomly()        { return Random.value <= moveProbability * Time.deltaTime; }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // state machine - update ///////////////////////////////////////////////////////////////////////////////////////////////
    private string Update_IDLE() {
        
        if (EventDied()) {
            OnDeath();
            currentSkill = -1;
            return "DEAD";
        }
        
        if (EventTargetDied()) {
            enemy        = null;
            currentSkill = -1;
            return "IDLE";
        }

        if (EventTargetTooFarToFollow()) {
            enemy                   = null;
            currentSkill            = -1;
            agent.stoppingDistance  = 0;
            agent.destination       = startPosition;
            return "MOVING";
        }

        if (EventTargetTooFarToAttack()) {
            agent.stoppingDistance  = CurrentCastRange() * 0.8f;
            agent.destination       = enemy.collider.ClosestPointOnBounds(transform.position);
            return "MOVING";
        }

        if (EventSkillRequest()) {
            var skill = skills[currentSkill];
            if (CastCheckSelf(skill) && 
                CastCheckTarget(skill)) {

                SpawnSkillStartEffect(currentSkill, transform.forward);
                skill.castTimeEnd    = Time.time + skill.castTime;
                skills[currentSkill] = skill;
                return "CASTING";
            } else {
                enemy        = null;
                currentSkill = -1;
                return "IDLE";
            }
        }

        if (EventAggro()) {
            if (skills.Count > 0) currentSkill = UseSkillSelection();
            return "IDLE";
        }

        if (EventMoveRandomly()) {
            var circle2D                = Random.insideUnitCircle * moveDistance;
                agent.stoppingDistance  = 0;
                agent.destination       = startPosition + new Vector3(circle2D.x, 0, circle2D.y);
            return "MOVING";
        }

        if (EventDeathTimeElapsed())    { }
        if (EventRespawnTimeElapsed())  { }
        if (EventMoveEnd())             { }
        if (EventSkillFinished())       { }
        if (EventTargetDisappeared())   { }

        return "IDLE";
    }
    private string Update_MOVING() {
        if (EventDied()) {
            OnDeath();
            currentSkill = -1;
            agent.ResetPath();
            return "DEAD";
        }

        if (EventMoveEnd()) {
            return "IDLE";
        }

        if (EventTargetDied()) {
            enemy           = null;
            currentSkill    = -1;
            agent.ResetPath();
            return "IDLE";
        }

        if (EventTargetTooFarToFollow()) {
            enemy                   = null;
            currentSkill            = -1;
            agent.stoppingDistance  = 0;
            agent.destination       = startPosition;
            return "MOVING";
        }

        if(EventTargetTooFarToAttack()) {
            agent.stoppingDistance  = CurrentCastRange() * 0.8f;
            agent.destination       = enemy.collider.ClosestPointOnBounds(transform.position);
            return "MOVING";
        }

        if (EventAggro()) {
            if (skills.Count > 0) currentSkill = UseSkillSelection();
            agent.ResetPath();
            return "IDLE";
        }

        if (EventDeathTimeElapsed())    { }
        if (EventRespawnTimeElapsed())  { }
        if (EventSkillFinished())       { }
        if (EventTargetDisappeared())   { }
        if (EventSkillRequest())        { }
        if (EventMoveRandomly())        { }

        return "MOVING";
    }
    private string Update_CASTING() {
        if (enemy) LookAtY(enemy.transform.position);

        if (EventDied()) {
            OnDeath();
            currentSkill = -1;
            return "DEAD";
        }

        if (EventTargetDisappeared()) {
            currentSkill = -1;
            enemy        = null;
            return "IDLE";
        }

        if (EventTargetDied()) {
            currentSkill = -1;
            enemy        = null;
            return "IDLE";
        }

        if (EventSkillFinished()) {
            CastSkill(skills[currentSkill]);

            if (enemy.health == 0) enemy = null;

            currentSkill = -1;
            return "IDLE";
        }

        if (EventDeathTimeElapsed())     { }
        if (EventRespawnTimeElapsed())   { }
        if (EventMoveEnd())              { }
        if (EventTargetTooFarToFollow()) { }
        if (EventTargetTooFarToAttack()) { }
        if (EventAggro())                { }
        if (EventSkillRequest())         { }
        if (EventMoveRandomly())         { }

        return "CASTING";
    }

    private string Update_DEAD() {

        if (EventRespawnTimeElapsed()) {
            gold = 0;
            inventory.Clear();
            agent.Warp(startPosition);
            Revive();
            return "IDLE";
        }

        if (EventDeathTimeElapsed())     { }
        if (EventSkillRequest())         { }
        if (EventSkillFinished())        { }
        if (EventMoveEnd())              { }
        if (EventTargetDisappeared())    { }
        if (EventTargetTooFarToFollow()) { }
        if (EventTargetTooFarToAttack()) { }
        if (EventAggro())                { }
        if (EventMoveRandomly())         { }
        if (EventDied())                 { }

        return "DEAD";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // life update //////////////////////////////////////////////////////////////////////////////////////////////////////////
    protected override string   UpdateState() {
        if (state == "IDLE")    return Update_IDLE();
        if (state == "MOVING")  return Update_MOVING();
        if (state == "CASTING") return Update_CASTING();
        if (state == "DEAD")    return Update_DEAD();

        Debug.LogError("정의되지 않은 상태 : " + state);
        return "IDLE";
    }
    protected override void UpdateHandle() {
       
        if (state == "CASTING") {
            if (enemy) LookAtY(enemy.transform.position);
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // fucntion /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // select use skill //
    private int UseSkillSelection() {

        if (skills.Count <= 0) {
            Debug.LogError("Dont Have Skill");
            return -1;
        } else {
            if (health <= healthMax * 0.5f) {
                if (skills.Exists(s => s.category == "Heal")) {

                    var heal = skills.Find(s => s.category == "Heal");
                    if (heal.IsReady() &&
                        mana >= heal.manaCost) {

                        return skills.IndexOf(heal);
                    }   
                }
            }
            
            if (skills.Exists(s => s.category == "Buff")) {

                var buffs = skills.FindAll(s => s.category == "Buff");
                foreach (Skill buff in buffs) {
                    if (buff.IsReady() &&
                        mana >= buff.manaCost) {

                        return skills.IndexOf(buff);
                    }
                }
            }

            if (skills.Exists(s => s.category == "Attack")) {

                var attacks = skills.FindAll(s => s.category == "Attack");
                var count   = attacks.Count;
                for (int i = 0; i < 5; i++) {
                    var rand = Random.Range(0, count);
                    var attack = attacks[rand];
                    if (attack.IsReady() &&
                        mana >= attack.manaCost) {

                        return skills.IndexOf(attack);
                    }
                }
            }
        }

        return -1;
    }

    // override IsMoving() //
    protected override bool IsMoving() {
        return  agent.pathPending ||
                agent.velocity != Vector3.zero ||
                agent.remainingDistance > agent.stoppingDistance;
    }

    // aggro //
    public override void OnAggro(Entity entity) {

        if (entity != null && CanAttack(entity)) {

            if (enemy == null) {
                enemy = entity;
            } else {
                float oldDistance = Vector3.Distance(transform.position, enemy.transform.position);
                float newDistance = Vector3.Distance(transform.position, entity.transform.position);
                if (newDistance < oldDistance * 0.8f) enemy = entity;
            }
        }
    }

    // loot //
    public bool HasLoot() {
        return gold > 0 || inventory.Any(item => item.valid);
    }

    // death //
    private void OnDeath() {

        deathTimeEnd    = Time.time + deathTime;
        respawnTimeEnd  = deathTimeEnd + respawnTime;

        StopBuffs();
        enemy = null;

        gold = Random.Range(min: lootGoldMin, max: lootGoldMax);

        foreach (ItemDropChance itemChance in dropChances) {

            if (Random.value <= itemChance.probability)
                inventory.Add(new Item(itemChance.template));
        }
    }

    // skills //
    public override bool    CanAttack(Entity entity) {
        return  health            > 0 &&
                entity.health     > 0 &&
                entity           != this &&
                (entity.GetType() == typeof(Player));
    }
    public float CurrentCastRange() {
        return (currentSkill <= 0 &&
                currentSkill <  skills.Count) ? skills[currentSkill].castRange : 0;
    }
    public void SpawnSkillStartEffect(int skillIndex, Vector3 _direction) {

        var skill = skills[skillIndex];
        if (skill.startedEffectPrefab != null) {

            var mount       = transform.FindRecursively("EffectMount");
            var position    = mount != null ? mount.position : transform.position;
            var go          = Instantiate(skill.startedEffectPrefab, position, skill.startedEffectPrefab.transform.rotation);
            var effect      = go.GetComponent<SkillEffect>();

            effect.target       = enemy;
            effect.caster       = this;
            effect.skillIndex   = skillIndex;
        }
    }
    // revive //
    public void Revive(float healthPercentage = 1) {
        health = Mathf.RoundToInt(healthMax * healthPercentage);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}