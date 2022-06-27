using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity {

    // 추가 변수 start

    public float experience    = 0f;
    public float experienceMax = 100f;
    public bool isMontaDead = false;
    private float reTime = 5;

    [Header("Respawn")]
    public float deathTime = 2;
    private float deathTimeEnd;
    public bool destroyable = false;   // jj
    public bool respawn = true;
    public float respawnTime = 2;
    private float respawnTimeEnd;

    // 추가 변수 end

    // 무의미, 오류 방지용
    public override int healthMax       { get { return 3000; } }
    public override int manaMax         { get { return 1000; } }
    public override int strikingPower   { get { return 500; } }
    public override int defensivePower  { get { return 1; } }
    public override bool CanAttack(Entity entity) {
        return  entity.health  > 0     &&
                entity        != this  &&
                this.health    > 0;
    }

    private bool inputJump   = false;

    private int nextSkill = -1;

    public int inventorySize = 50;
    public ItemList inventory = new ItemList();
    public TemplateItem[] defaultItems = null;
    public long gold = 0;

    public string[] skillbar = { "", "", "", "", "", "", "", "", "" };
    public Vector3[] SkillBarPoint = {
        new Vector3(-145.8f, -110.1f, 0f),
        new Vector3(-136f, 46.4f, 0f),
        new Vector3(-9.5f, 148.4f, 0f),
        new Vector3(146.4f, 123f, 0f)
    };
    public int[] potionbar = { -1, -1 };

    private void Start() {

        DontDestroyOnLoad(transform.gameObject);    // jj
        
        // skill test
        for (int i = 0; i < defaultSkills.Length; i++) {
            skills.Add(new Skill(defaultSkills[i]));
        }

        // inventory test
        for (int i = 0; i < inventorySize; i++) {

            // default item이 존재하는 경우
            if (i < defaultItems.Length) {
                inventory.Add(new Item(defaultItems[i]));
                continue;
            }

            inventory.Add(new Item());
        }
    }
    private void Update() {
        // state 변화는 이미 알 수 있다.
        animator.SetBool    ("WALKING"      , state == "ONLY MOVE" ||
                                              state == "MOVE AND CAST"
                                              );
        animator.SetBool    ("CASTING"      , state == "ONLY CAST" || 
                                              state == "MOVE AND CAST");
        animator.SetInteger ("currentSkill" , currentSkill);
        animator.SetBool    ("DEAD"         , state == "DEAD");
    }
    
    #region State Machine - Event
    private bool EventDied()                    { return health <= 0; }
    private bool EventJumpRequest()             { return IsGround() == true && inputJump == true; }
    private bool EventJumpFinished()            { return IsGround() == true && state == "JUMP"; }
    private bool EventOnlyCastRequest()         { return 0 <= currentSkill && currentSkill < skills.Count && IsMoving() == false; }
    private bool EventOnlyMoveRequest()         { return currentSkill == -1 && IsMoving() == true; }
    private bool EventMoveAndCastRequest()      { return 0 <= currentSkill && currentSkill < skills.Count && IsMoving() == true; }
    private bool EventCastFinished()            { return 0 <= currentSkill && currentSkill < skills.Count && skills[currentSkill].CastTimeRemaining() == 0; }
    private bool EventMoveEnd()                 { return IsMoving() == false; }
    #endregion
    #region State Machine - Event Check
    private bool IsMoving()
    {
        if (isMoving == true) return true;
        return false;
    }
    private bool IsGround()
    {
        if (grounded) {
            hitDistance = 0.35f;
        } else {
            hitDistance = 0.15f;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, Mathf.Infinity, groundLayers)) {

            var distance = Utils.ClosestDistance(collider, hitInfo.transform.GetComponent<Collider>());
            if (distance <= hitDistance) return (grounded = true);
            else                         return (grounded = false);
        }

        return true;
    }
    #endregion
    #region State Machine - command

    private HashSet<string> cmdEvents = new HashSet<string>();

    private void CmdMove()      { cmdEvents.Add("IsMoving"); }
    private bool EventMove()    { return cmdEvents.Remove("IsMoving"); }

    #endregion
    #region State Machine - Update func
    private string UpdateIDLE() {

        if(EventDied()) {
            // TODO: respawn 함수 호출
            currentSkill = nextSkill = -1;
            OnDeath();
            return "DEAD";
        }

        if (EventMove()) {
            if (EventOnlyMoveRequest()) { return "ONLY MOVE"; }
        }

        if (EventOnlyCastRequest()) {

            var skill = skills[currentSkill];
            if (CastCheckSelf(skill)    && // TODO: 함수 재정의
                CastCheckTarget(skill)  && // TODO: 함수 재정의
                CastCheckDistance(skill)) {

                SpawnStartSkillEffect(currentSkill, enemy); // TODO: 함수 오버라이딩
                skill.castTimeEnd       = Time.time + skill.castTime;
                skills[currentSkill]    = skill;

                return "ONLY CAST";
            }
        }

        //if (EventJumpRequest()) { return "JUMP"; }

        return "IDLE";
    }

    private string UpdateONLYMOVE()
    {
        if (EventDied()) {
            // TODO: respawn 함수 호출
            currentSkill = nextSkill = -1;
            return "DEAD";
        }

        if (EventMoveEnd()) {
            return "IDLE";
        }

        if (EventMoveAndCastRequest()) {

            var skill = skills[currentSkill];
            if (CastCheckSelf(skill)    && // TODO: 함수 재정의
                CastCheckTarget(skill)  && // TODO: 함수 재정의
                CastCheckDistance(skill)) {

                SpawnStartSkillEffect(currentSkill, enemy); // TODO: 함수 오버라이딩 하기
                skill.castTimeEnd = Time.time + skill.castTime;
                skills[currentSkill] = skill;

                if (skill.canMove == true) {
                    return "MOVE AND CAST";
                }
                else {
                    return "ONLY CAST";
                }
            }
        }

        if (EventJumpRequest()) { return "JUMP"; }

        return "ONLY MOVE";
    }
    private string UpdateMOVEANDCAST() {

        if (EventDied()) {
            // TODO: respawn 함수 호출
            currentSkill = nextSkill = -1;
            return "DEAD";
        }

        if (EventMoveEnd()) {
            return "ONLY CAST";
        }

        if (EventCastFinished()) {

            var skill = skills[currentSkill];
            CastSkill(skill);
            if (nextSkill != -1) {

                currentSkill = nextSkill;
                nextSkill    = -1;
            } else {
                currentSkill = -1;
            }

            return "ONLY MOVE";
        }

        return "MOVE AND CAST";
    }
    private string UpdateONLYCAST() {

        if (EventDied()) {
            // TODO: respawn 함수 호출
            currentSkill = nextSkill = -1;
        }

        if (EventMove()) {

            var skill = skills[currentSkill];
            if (skill.canMove == true) {
                return "MOVE AND CAST";
            } else {
                return "ONLY CAST";
            }
        }

        if (EventCastFinished()) {
          
            var skill = skills[currentSkill];
            CastSkill(skill);
            if (nextSkill != -1) {
                currentSkill = nextSkill;
                nextSkill    = -1;
            } else {
                currentSkill = -1;
            }
            
            return "IDLE";
        }

        return "ONLY CAST";
    }
    private string UpdateDEAD() {
        if(Time.time >= respawnTimeEnd) {
            health = healthMax / 2;
            return "IDLE";
        }
        return "DEAD";
    }
    private string UpdateJUMP()
    {
        if(EventJumpFinished()) {
            grounded = true;
            return "IDLE";
        }

        return "JUMP";
    }
    protected override string UpdateState() {
        if (state == "IDLE")            return UpdateIDLE();
        if (state == "ONLY MOVE")       return UpdateONLYMOVE();
        if (state == "MOVE AND CAST")   return UpdateMOVEANDCAST();
        if (state == "ONLY CAST")       return UpdateONLYCAST();
        if (state == "DEAD")            return UpdateDEAD();

        if (state == "JUMP") return UpdateJUMP();

        Debug.LogError(name + " -- Player::UpdateState -- No state named : " + state);
        return "IDLE";
    }
    #endregion
    #region State Machine - Update Handle func
    private bool isMoving = false;
    private void MoveHandling() {

        isMoving = false;

        #region PC
        var H           = Input.GetAxis("Horizontal");
        var V           = Input.GetAxis("Vertical");
        var velocity    = new Vector3(H, 0f, V);

        if (InputSystem.GetKey("Forward")) {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            isMoving = true;
        }
        if (InputSystem.GetKey("Back"))
        {
            transform.position += transform.forward * -1f * moveSpeed * Time.deltaTime;
            isMoving = true;
        }
        if (InputSystem.GetKey("Left"))
        {
            transform.position += transform.right * -1f * moveSpeed * Time.deltaTime;
            isMoving = true;
        }
        if (InputSystem.GetKey("Right"))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
            isMoving = true;
        }

        if (isMoving == true) {
            var nomalizedVelocity = velocity.normalized;
            transform.rotation = Quaternion.LookRotation(nomalizedVelocity);
        }
        #endregion
        #region MOBILE
        var joyStick = FindObjectOfType<UIJoyStick>();
        if (joyStick != null) {
            var moveVector = (Vector3.right * joyStick.Horizontal + Vector3.forward * joyStick.Vertical);
            if (moveVector != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(moveVector);
                transform.Translate(moveVector * joyStick.fPower * moveSpeed * Time.fixedDeltaTime, Space.World);
                isMoving = true;
            }
        }
        #endregion

        if (IsMoving() == true) { CmdMove(); }
    }
    protected void InputJumpHandling()
    {
        if(IsGround() == false) { state = "JUMP"; return; }
        inputJump = false;
        if(InputSystem.GetKey("Jump"))
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            inputJump = true;
        }
    }

    protected void InputNormalAttackHandling() { 

        if      (Input.GetKeyDown(KeyCode.Alpha1))  { currentSkill = 0;  }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  { currentSkill = 1;  }
        else if (Input.GetKeyDown(KeyCode.Alpha3))  { currentSkill = 2;  }
    }

    protected override void UpdateHandle() {

        if (state == "IDLE") {
            MoveHandling();
            //InputJumpHandling();
            InputNormalAttackHandling();
        }
        else if (state == "ONLY MOVE") {
            MoveHandling();
            //InputJumpHandling();
            InputNormalAttackHandling();
        }
        else if (state == "MOVE AND CAST") {
            MoveHandling();
            InputNormalAttackHandling();
        }
        else if (state == "ONLY CAST") {
            InputNormalAttackHandling();
        }
        else if (state == "DEAD") {
        } else if (state == "JUMP") { }
        else {
            Debug.LogError(name + " -- Player::UpdateHandle -- No state named : " + state);
        }
    }
    public override void CastSkill(Skill skill) {
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
        if (skill.startedEffectPrefab != null) {
            SpawnStartSkillEffect(currentSkill, this);
        }
        if (skill.finishedEffectPrefab != null) {
            SpawnEndSkillEffect(currentSkill, enemy);
        }

        mana                -= skill.manaCost;
        skill.coolTimeEnd    = Time.time + skill.coolTime;
        skills[currentSkill] = skill;

        } else {
            currentSkill = -1;
        }
    }
    #endregion

    // death //
    private void OnDeath()
    {

        deathTimeEnd = Time.time + deathTime;
        respawnTimeEnd = deathTimeEnd + respawnTime;

        StopBuffs();
        enemy = null;
    }

    #region skill - Learn Skill
    public void CmdLearnSkill(int skillIndex) {

        if (health > 0 &&
            skillIndex >= 0 && skillIndex < skills.Count) {

            var skill = skills[skillIndex];

            if (!skill.learned &&
                skill.nowSkillPoint >= 1) {
                skill.nowSkillPoint -= 1;
                skill.learned = true;
                skills[skillIndex] = skill;
            }
        }
    }
    #endregion
    #region skill - Upgrade Skill
    public void CmdUpgradeSkill(int skillIndex) {

        if (health > 0 &&
            skillIndex >= 0 && skillIndex < skills.Count) {

            var skill = skills[skillIndex];
            if (skill.learned &&
                skill.level < skill.maxLevel &&
                skill.nowSkillPoint >= skill.requiredSkillPoint) {

                skill.nowSkillPoint -= skill.requiredSkillPoint;
                skill.level++;
                skills[skillIndex] = skill;
            }
        }
    }
    #endregion
    #region skill - Use Skill
    public void CmdUseSkill(int skillIndex) {

        if (health > 0 &&
            skillIndex >= 0 &&
            skillIndex < skills.Count) {

            if (skills[skillIndex].learned &&
                skills[skillIndex].IsReady()) {

                if      (currentSkill == -1)            currentSkill = skillIndex;
                else if (currentSkill != skillIndex)    nextSkill = skillIndex;
            }
        }
    }
    #endregion

    #region inventory - CmdUseInventoryItem
    public void CmdUseInventoryItem (int index) {

        if (health > 0 &&
            index >= 0 && index < inventory.Count &&
            inventory[index].valid) {

            var item = inventory[index];
            if (item.category.StartsWith("Potion")) {

                health  += item.usageHealth;
                mana    += item.usageMana;

                if (item.usageDestroy) {
                    item.amount--;
                    if (item.amount == 0) item.valid = false;
                    inventory[index] = item;
                }
            }
            else if (item.category.StartsWith("Skillbook")) {

                var lastNoun             = Utils.ParseLastNoun_jj(item.name);
                Debug.Log(lastNoun);
                var skill                = skills.Find(ski => ski.name == lastNoun);
                    skill.nowSkillPoint += item.usageSkillExperiene;

                var skillIndex           = skills.FindIndex(ski => ski.name == skill.name);
                    skills[skillIndex]   = skill; 

                CmdLearnSkill(skillIndex);              // 영재 - 이 함수를 통해 스킬 배움..

                if (item.usageDestroy) {
                    item.amount--;
                    if (item.amount == 0) item.valid = false;
                    inventory[index] = item;
                }
            }
        }
    }
    #endregion
    #region inventory - CmdSelInventoryItem
    public void CmdSelInventoryItem(int index, int num, long price)
    {

        if (health > 0 &&
            index >= 0 && index < inventory.Count &&
            inventory[index].valid)
        {

            var item = inventory[index];
            if (item.category.StartsWith("Potion"))
            {
                if (item.usageDestroy)
                {
                    item.amount -= num;
                    gold += num * price;
                    if (item.amount == 0) item.valid = false;
                    inventory[index] = item;
                }
            }
            else if (item.category.StartsWith("Skillbook"))
            {
                if (item.usageDestroy)
                {
                    item.amount -= num;
                    gold += num * price;
                    if (item.amount == 0) item.valid = false;
                    inventory[index] = item;
                }
            }
        }
    }
    #endregion
    #region inventory - CmdBuyInventoryItem
    public void CmdBuyInventoryItem(int index, int num, long price)
    {

        if (health > 0 &&
            index >= 0 && index < inventory.Count &&
            inventory[index].valid)
        {

            var item = inventory[index];
            if (item.category.StartsWith("Potion"))
            {
                if (item.usageDestroy)
                {
                    item.amount += num;
                    if (item.amount == 0) item.valid = false;
                    inventory[index] = item;
                    gold -= num * price;
                }
            }
            else if (item.category.StartsWith("Skillbook"))
            {
                if (item.usageDestroy)
                {
                    item.amount += num;
                    if (item.amount == 0) item.valid = false;
                    inventory[index] = item;
                    gold -= num * price;
                }
            }
        }
    }
    #endregion
    public void CmdClearSkillBarSlot(int[] indices) {
        // indices[0] : from, indices[1] : to
        for (int i = indices[0]; i <= indices[1]; i++) {
            skillbar[i] = "";
        }
    }
    public void OnDragAndDrop_SetSkillSlot_QuickSkillSlot(int[] slotIndices) {
        // slotIndices[0] : from, slotIndices[1] : to
        skillbar[slotIndices[1]] = skills[slotIndices[0]].name;
    }
    public void OnDragAndDrop_InventorySlot_InventorySlot(int[] slotIndices) {
        // slotIndices[0] : from, slotIndices[1] : to
        if ((inventory[slotIndices[0]].valid && inventory[slotIndices[1]].valid) &&
            (inventory[slotIndices[0]].name == inventory[slotIndices[1]].name)) {
            CmdInventoryMerge(slotIndices[0], slotIndices[1]);
        } else {
            CmdSwapInventoryInventory(slotIndices[0], slotIndices[1]);
        }
    }
    public void OnDragAndDrop_InventorySlot_QuickPotionSlot(int[] slotIndices) {
        // slotIndices[0] : from, slotIndices[1] : to
        if (inventory[slotIndices[0]].category != "Potion") return;
        potionbar[slotIndices[1]] = slotIndices[0];
    }
    public void CmdInventoryMerge(int from, int to) {
        if ((from >= 0 && from < inventory.Count) &&
            (to   >= 0 && to   < inventory.Count)) {

            if (inventory[from].valid && inventory[to].valid) {

                if (inventory[from].name == inventory[to].name) {

                    var itemFrom        = inventory[from];                                              
                    var itemTo          = inventory[to];                                                
                    int stack           = Mathf.Min(itemFrom.amount + itemTo.amount, itemTo.maxStack);  
                    int put             = stack - itemFrom.amount;                                      
                        itemFrom.amount = itemTo.amount - put;                                          
                        itemTo.amount   = stack;                                                        

                    if (itemFrom.amount == 0) itemFrom.valid = false;

                    inventory[from] = itemFrom;     
                    inventory[to]   = itemTo;       
                }
            }
        }
    }
    public void CmdSwapInventoryInventory(int from, int to) {

        if ((from >= 0 && from < inventory.Count) &&
            (to   >= 0 && to   < inventory.Count)) {

            for (int i = 0; i < potionbar.Length; i++) {
                if      (potionbar[i] == from)  potionbar[i] = to;
                else if (potionbar[i] == to)    potionbar[i] = from;
            }

            var temp            = inventory[from];
                inventory[from] = inventory[to];
                inventory[to]   = temp;
        }
    }
}