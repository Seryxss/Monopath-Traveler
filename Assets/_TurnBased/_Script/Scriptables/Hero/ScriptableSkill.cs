using UnityEngine;

// Enum untuk mempermudah pilihan di Inspector
public enum TargetType { Single, All, Random, Self }
public enum DamageType { Sword, Axe, Bow, Fire, Ice, Light, Heal, Buff }

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class ScriptableSkill : ScriptableObject
{
    public string skillName;
    [TextArea(3, 5)] public string description;

    [Header("Skill Details")]
    public TargetType targetType;
    public DamageType damageType;
    public int spCost;
    public int power; // Kekuatan jurus (Potency)
}