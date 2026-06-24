using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class ScriptableSkill : ScriptableObject
{
    public string skillName;
    [TextArea(3, 5)] public string description;

    [Header("Skill Details")]
    public TargetType targetType;
    public DamageType damageType = DamageType.None;
    public int spCost;
    public int power;
}