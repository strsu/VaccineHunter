
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

[CreateAssetMenu(fileName = "New Skill", menuName = "Create Skill", order = 999)]
public class TemplateSkill : ScriptableObject {

    [Header("General")]
    public string        category     = "";   // 스킬 종류 ( ex, Buff, Attack, Heal --- )
    public bool          learnDefault = false;// 처음 들고 있는지 여부
    public bool          canMove      = false;
    public Sprite        icon         = null; // 스킬 아이콘
    public TemplateSkill relatedSkill = null; // 연계 공격

    [TextArea(0, 50)] public string toolTip = null;

    [System.Serializable]
    public struct SkillLevel {

        [Header("General")]
        public float castTime;      // 스킬 시전에 걸리는 시간
        public float coolTime;      // 스킬 재사용까지의 시간
        public int   healthCost;    // 소비 생명력
        public int   manaCost;      // 소비 마나
        public float castMaxRange;  // 시전 가능 최대거리
        public int   requiredSkillPoint; // 스킬 사용 필요 요구 스킬 포인트
        public SkillEffect finishedEffectPrefab;    // 시전 종료시 발생
        public SkillEffect startedEffectPrefab;      // 시전 시작시 발생

        [Header("Buff")]
        public float buffTime;
        public int   buffStrength; // 버프 사용시 가산되는 힘
        public int   buffCritical; // 버프 사용시 가산되는 크리티컬

        [Header("Heal")]
        public int healHealth;
        public int healMana;

        [Header("Attack")]
        public int   damage;    // 스킬 공격력
        public float castRange; // 스킬 시전 가능 범위
        public float aoeRadius; // 스킬 공격 범위
    }

    public SkillLevel[] levels = { new SkillLevel() };

    #region meta data
    private static Dictionary<string, TemplateSkill> cache = null;
    public  static Dictionary<string, TemplateSkill> dict {

        get {

            return cache ?? (cache = Resources.LoadAll<TemplateSkill>("").ToDictionary(
                skill => skill.name, skill => skill)
                );
        }
    }
    #endregion
}