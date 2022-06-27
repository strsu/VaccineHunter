
using UnityEngine;

[System.Serializable]
public class ItemDropChance {

                    public TemplateItem template;
    [Range(0, 1)]   public float        probability;
                    public int          minCount;
                    public int          maxCount;
}
