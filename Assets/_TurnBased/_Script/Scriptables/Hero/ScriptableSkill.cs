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

    [Header("Camera Shake")]
    public bool doCameraShake = false;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.15f;

    [Header("Effect Value")]
    public int basePower;
    public ScriptableElement skillElement;

    [Header("VFX Spell Settings (used when Skill Category = Elem)")]
    public GameObject castVfxPrefab;          
    public float castVfxDuration = 0.6f;     
    public GameObject vfxPrefab;            
    public VFXSpawnLocation vfxSpawnLocation = VFXSpawnLocation.ActionCenter;  
    public AudioClip castSound;
    public AudioClip VFXSound;       

    [Header("Melee Slash VFX (used for Phys attacks)")]
    public GameObject swingVfxPrefab;       
    public GameObject slashVfxPrefab;       
    public GameObject sparkVfxPrefab;
    public AudioClip attackNormal;
    public AudioClip attackWeakness;   
    public float slashImpactDelay = 0.15f;  
    public float vfxDuration = 1.5f;
    public float vfxImpactDelay = 0.7f;

    [Header("Optional Secondary Effects")]
    public bool hasEnfeeblingEffect;
    public bool hasAugmentEffect;

    
}