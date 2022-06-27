
using System.Collections.Generic;
using UnityEngine;

using System.Text;

[System.Serializable]
public partial struct Skill {

    public string name;

    public bool  learned;       // 스킬 습득 여부
    public int   level;         // 스킬 레벨
    public int   nowSkillPoint; // 스킬이 가지고 있는 스킬 포인트
    public float castTimeEnd;   // 시전 종료 시간
    public float coolTimeEnd;   // 재사용 종료 시간
    public float buffTimeEnd;   // 버프 종료 시간
	
    public Skill (TemplateSkill template) {

        name            = template.name;
        learned         = template.learnDefault;
        level           = 1;
        nowSkillPoint   = 0;
        castTimeEnd     = coolTimeEnd = buffTimeEnd = 0;
    }

    public bool             TemplateExists()     { return TemplateSkill.dict.ContainsKey(name); }
    public TemplateSkill    template             { get { return TemplateSkill.dict[name]; } }      // 스킬 사전에 있는 name이란 스킬 호출
                                                 
    public string           category             { get { return template.category; } }
    public Sprite           icon                 { get { return template.icon; } }

    public bool             canMove              { get { return template.canMove; } }
                                                 
    public float            castTime             { get { return template.levels[level - 1].castTime; } }
    public float            coolTime             { get { return template.levels[level - 1].coolTime; } }
    public int              healthCost           { get { return template.levels[level - 1].healthCost; } }
    public int              manaCost             { get { return template.levels[level - 1].manaCost; } }
    public float            castMaxRange         { get { return template.levels[level - 1].castMaxRange; } }
    public int              requiredSkillPoint   { get { return template.levels[level - 1].requiredSkillPoint; } }
                                                  
    public float            buffTime             { get { return template.levels[level - 1].buffTime; } }
    public int              buffStrength         { get { return template.levels[level - 1].buffStrength; } }
    public int              buffCritical         { get { return template.levels[level - 1].buffCritical; } }
                                                 
    public int              healHealth           { get { return template.levels[level - 1].healHealth; } }
    public int              healMana             { get { return template.levels[level - 1].healMana; } }
                                                 
    public int              damage               { get { return template.levels[level - 1].damage; } }
    public float            aoeRadius            { get { return template.levels[level - 1].aoeRadius; } }
    public float            castRange            { get { return template.levels[level - 1].castRange; } }
                                                 
    public int              maxLevel             { get { return template.levels.Length; } }
                                                 
    public SkillEffect      startedEffectPrefab  { get { return template.levels[level - 1].startedEffectPrefab; } }
    public SkillEffect      finishedEffectPrefab { get { return template.levels[level - 1].finishedEffectPrefab; } }

    public float            CastTimeRemaining()  { return Time.time >= castTimeEnd ? 0 : castTimeEnd - Time.time; }
    public float            CooldownRemaining()  { return Time.time >= coolTimeEnd ? 0 : coolTimeEnd - Time.time; }
    public float            BuffTimeRemaining()  { return Time.time >= buffTimeEnd ? 0 : buffTimeEnd - Time.time; }
    public bool             IsReady()            { return CooldownRemaining() == 0; }

    public string ToolTip() {

        var tip = new StringBuilder(template.toolTip);

        tip.Replace("{NAME}", name);
        tip.Replace("{CATEGORY}", category);
        tip.Replace("{LEVEL}", learned == true ? level.ToString() : (level - 1).ToString());
        tip.Replace("{NOWSKILLPOINT}", nowSkillPoint.ToString());
        tip.Replace("{REQUIREDSKILLPOINT}", learned == true ? template.levels[level - 1].requiredSkillPoint.ToString() : "1");

        tip.Replace("{BUFFSTRENGTH}", template.levels[level - 1].buffStrength.ToString());
        tip.Replace("{BUFFCRITICAL}", template.levels[level - 1].buffCritical.ToString());

        tip.Replace("{HEALHEALTH}", template.levels[level - 1].healHealth.ToString());
        tip.Replace("{HEALMANA}", template.levels[level - 1].healMana.ToString());

        tip.Replace("{DAMAGE}", template.levels[level - 1].damage.ToString());
        tip.Replace("{AOERADIUS}", template.levels[level - 1].aoeRadius.ToString());
        tip.Replace("{CASTRANGE}", template.levels[level - 1].castRange.ToString());

        return tip.ToString();
    }
}

public class SkillList : List<Skill> { }