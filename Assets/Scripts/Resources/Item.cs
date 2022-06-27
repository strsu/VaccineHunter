
using UnityEngine;
using System.Collections.Generic;

using System.Text;

[System.Serializable]
public struct Item {

    public string name;
    public bool valid;
    public int amount;

    public Item(TemplateItem template, int amount = 1) {

        name = template.name;
        this.amount = amount;
        valid = true;
    }

    public bool TemplateExists() {
        return name != null && TemplateItem.dict.ContainsKey(name);
    }

    public TemplateItem template { get { return TemplateItem.dict[name]; } }

    public string category { get { return template.category; } }
    public int maxStack { get { return template.maxStack; } }
    public long buyPrice { get { return template.buyPrice; } }
    public long sellPrice { get { return template.sellPrice; } }
    public bool sellable { get { return template.sellable; } }
    public int minLevel { get { return template.minLevel; } }
    public bool destroyable { get { return template.destroyable; } }
    public Sprite icon { get { return template.icon; } }

    public bool usageDestroy { get { return template.usageDestroy; } }
    public int usageHealth { get { return template.usageHealth; } }
    public int usageMana { get { return template.usageMana; } }
    public int usageSkillExperiene { get { return template.usageSkillExperiene; } }

    public string ToolTip() {

        var tip = new StringBuilder(template.toolTip);

        tip.Replace("{NAME}", name);
        tip.Replace("{CATEGORY}", category);
        tip.Replace("{MINLEVEL}", minLevel.ToString());
        tip.Replace("{BUYPRICE}", buyPrice.ToString());
        tip.Replace("{SELLPRICE}", sellPrice.ToString());
        tip.Replace("{SELLABLE}", sellable == true ? "판매가능" : "판매불가");
        tip.Replace("{DESTROYABLE}", destroyable == true ? "파괴가능" : "파괴불가");

        tip.Replace("{USAGEHEALTH}", usageHealth.ToString());
        tip.Replace("{USAGEMANA}", usageMana.ToString());
        tip.Replace("{USAGESKILLEXPERIENCE}", usageSkillExperiene.ToString());

        return tip.ToString();
    }
}

public class ItemList : List<Item> { }