using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class ScriptableSkill : ScriptableObject
{
    

    [Header("General Info")]
    public string skillName;
    public int spCost;
    [TextArea(3, 5)] public string description;

    [Header("Skill Properties")]
    public SkillCategory skillCategory = SkillCategory.None; 
    public TargetScope targetScope;

    [Header("Effect Value")]
    public int basePower;
    public ScriptableElement skillElement;

    [Header("VFX Spell Settings")]
    public GameObject vfxPrefab;
    public float vfxDuration = 1.5f;
    public VFXSpawnLocation vfxSpawnLocation = VFXSpawnLocation.ActionCenter;

    [Header("Optional Secondary Effects")]
    public bool hasEnfeeblingEffect;
    public bool hasAugmentEffect;

    
}