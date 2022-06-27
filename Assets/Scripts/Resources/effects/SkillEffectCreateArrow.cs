
using UnityEngine;

public class SkillEffectCreateArrow : SkillEffect {

	[System.Serializable]
    public class EffectProperty {

        [HideInInspector] public RuntimeAnimatorController  TargetAnimation = null;
        [HideInInspector] public Transform                  BonePosition    = null;
        [HideInInspector] public Transform                  BoneRotation    = null;
        [HideInInspector] public GameObject                 CurrentInstance = null;
                          public float                      DestroyTime     = 10f;
                          public GameObject                 Prefab          = null;
    }

    public float CreateTimeEffect1  = 0.0f;  // buff effect
    public float CreateTimeEffect2  = 0.22f; // bow  effect
    public float CreateTimeEffect3  = 3.03f; // shot effect

    public float destroyTime        = 10f;   // create (this) 객체의 제거 시간

    public EffectProperty effect1 = null; private bool createdeffect1 = false;
    public EffectProperty effect2 = null; private bool createdeffect2 = false;
    public EffectProperty effect3 = null; private bool createdeffect3 = false;

    public GameObject Center    = null; // 시전 캐릭터 중심
    public GameObject Bow       = null; // 시전 캐릭터 활
    public GameObject ArrowPref = null; // 화살 프리팹

    private GameObject Arrow;
}
