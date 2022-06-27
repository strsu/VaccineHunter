
using UnityEngine;

public abstract class SkillEffect : MonoBehaviour {

    public bool isPurelyVisual = true;

    private GameObject _caster = null;
    public Entity caster {

        get { return _caster != null ? _caster.GetComponent<Entity>() : null; }
        set { _caster = value != null ? value.gameObject : null; }
    }

    private GameObject _target = null;
    public Entity target {

        get { return _target != null ? _target.GetComponent<Entity>() : null; }
        set { _target = value != null ? value.gameObject : null; }
    }

    [HideInInspector]
    public int skillIndex;
    public Skill sourceSkill { get { return caster.skills[skillIndex]; } }
}