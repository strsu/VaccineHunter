
using UnityEngine;
using System.Collections.Generic;

using System.Linq;

[CreateAssetMenu(fileName = "New Item", menuName = "Create Item", order = 999)]
public class TemplateItem : ScriptableObject {

    [Header("General")]
    [TextArea(1, 50)] public string toolTip = null;

    [Header("Base")]
    public string category;
    public int maxStack;
    public long buyPrice;
    public long sellPrice;
    public int minLevel;
    public bool sellable;
    public bool destroyable;
    public Sprite icon;

    [Header("Potion")]
    public bool usageDestroy;
    public int usageHealth;
    public int usageMana;
    public int usageSkillExperiene;

    private static Dictionary<string, TemplateItem> cache = null;
    public static Dictionary<string, TemplateItem> dict
    {

        get
        {
            return cache ?? (cache = Resources.LoadAll<TemplateItem>("").ToDictionary(
                             item => item.name, item => item)
                             );
        }
    }
    private void OnValidate()
    {

        if (sellPrice > buyPrice)
        {
            long temp = sellPrice;
            sellPrice = buyPrice;
            buyPrice = temp;
        }
    }
}
