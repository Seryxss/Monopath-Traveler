using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroCharBase : CharacterBase
{
    [Header("Combat Planning")]
    [SerializeField] private ActionIntent currentIntent = new ActionIntent();
    [SerializeField] private ScriptableSkill _basicAttackSkill;
    public ScriptableSkill BasicAttackSkill => _basicAttackSkill;

    [Header("Boost State")]
    private int currentBP = 3; 
    private int allocatedBoost = 0;
    
    public int CurrentBP 
    { 
        get => currentBP; 
        set => currentBP = value; 
    }
    
    public int AllocatedBoost 
    { 
        get => allocatedBoost;
        set => allocatedBoost = value;
    }

    public ActionIntent CurrentIntent => currentIntent;

    void Start()
{
    SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    
    if (spriteRenderer != null && spriteRenderer.material != null)
    {
        // Paksa outline menjadi transparan saat karakter pertama kali spawn
        spriteRenderer.material.SetColor("_OutlineColor", Color.clear);
    }
}
    protected override void Awake()
    {
        base.Awake();
        BattleManager.OnPreStateChange += OnStateChanged;
    }

    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;
    private void OnStateChanged(BattleState newState)
    {
        if (newState == BattleState.HeroTurn) return;
    }

    public void InitializeTurnIntent(CharacterBase defaultEnemy)
    {
        List<CharacterBase> activeEnemies = CharacterManager.Instance.ActiveEnemies;
        currentIntent.ResetToDefault(activeEnemies, _basicAttackSkill);
    }

    public void ChangeBoostLevel(int newLevel)
    {
        int prevBoost = allocatedBoost;
        allocatedBoost = newLevel;

        if (BoostVFXManager.Instance != null)
        {
            BoostVFXManager.Instance.PlayBoostEffect(this, allocatedBoost, prevBoost);
        }
    }

    public virtual void ExecuteMove(Action onComplete)
    {
        StartCoroutine(AttackSequenceCoroutine(onComplete));
    }

    private IEnumerator AttackSequenceCoroutine(Action onComplete)
    {
        ScriptableSkill skill = CurrentIntent.ChosenSkill;
        if (skill == null) skill = _basicAttackSkill;

        if (skill != null && skill.spCost > 0) ConsumeSP(skill.spCost);

        if (skill.skillCategory == SkillCategory.Recovery)
            yield return StartCoroutine(ExecuteRecoverySkill(skill));
        else if (skill.skillCategory == SkillCategory.Augment)
            yield return StartCoroutine(ExecuteAugmentSkill(skill));
        else if (skill.skillCategory == SkillCategory.Elem)
            yield return StartCoroutine(ExecuteVFXSpell(skill));
        else
            yield return StartCoroutine(ExecuteOffensiveSkill(skill));

        OnAttackFinished();
        onComplete?.Invoke();
    }

    private IEnumerator ExecuteOffensiveSkill(ScriptableSkill skill)
{
    List<CharacterBase> targets = ResolveEnemyTargets(skill);
    if (targets.Count == 0) yield break;

    Vector3 centerStagePos = BattleManager.Instance.ActionCenterPosition;
    yield return StartCoroutine(MoveToPosition(centerStagePos, 0.2f)); 

    int totalHits = 1 + allocatedBoost; 

    Dictionary<CharacterBase, (int damage, DamageEffectiveness effectiveness)> damageData = new Dictionary<CharacterBase, (int, DamageEffectiveness)>();
    foreach (CharacterBase targetEnemy in targets)
    {
        if (targetEnemy == null) continue;
        DamageEffectiveness eff;
        int dmg = CalculateDamagePerHit(targetEnemy, skill, out eff);
        damageData[targetEnemy] = (dmg, eff);
    }

    for (int i = 0; i < totalHits; i++)
    {
        foreach (CharacterBase targetEnemy in targets)
        {
            if (targetEnemy == null) continue;
            if (!damageData.TryGetValue(targetEnemy, out var data)) continue;
            targetEnemy.TakeDamage(data.damage, data.effectiveness);
        }
        yield return new WaitForSeconds(0.3f);
    }

    foreach (CharacterBase targetEnemy in targets)
        if (targetEnemy is EnemyBase enemyBase) enemyBase.EvaluateDeathStatus();

    yield return new WaitForSeconds(0.2f);
    yield return StartCoroutine(MoveToPosition(_originalStandPosition, 0.2f));
}

    private IEnumerator ExecuteRecoverySkill(ScriptableSkill skill)
    {
        List<HeroCharBase> targets = ResolveHeroTargets(skill);
        int healAmount = CalculateRecoveryAmount(skill);

        foreach (HeroCharBase targetHero in targets)
        {
            if (targetHero == null) continue;
            targetHero.Heal(healAmount);
        }

        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator ExecuteVFXSpell(ScriptableSkill skill)
{
    List<CharacterBase> targets = ResolveEnemyTargets(skill);
    if (targets.Count == 0) yield break;

    List<GameObject> spawnedVFX = new List<GameObject>();

    if (skill.vfxSpawnLocation == VFXSpawnLocation.ActionCenter)
    {
        Vector3 pos = BattleManager.Instance.ActionCenterPosition;
        if (skill.vfxPrefab != null) spawnedVFX.Add(Instantiate(skill.vfxPrefab, pos, Quaternion.identity));
    }
    else
    {
        foreach (CharacterBase target in targets)
        {
            if (target == null || skill.vfxPrefab == null) continue;
            Vector3 pos = target.transform.position + new Vector3(0f, 0.5f, 0f);
            spawnedVFX.Add(Instantiate(skill.vfxPrefab, pos, Quaternion.identity));
        }
    }

    foreach (CharacterBase target in targets)
    {
        if (target == null) continue;
        DamageEffectiveness effectiveness;
        int damage = CalculateDamagePerHit(target, skill, out effectiveness);
        target.TakeDamage(damage, effectiveness);
        if (target is EnemyBase enemyBase) enemyBase.EvaluateDeathStatus();
    }

    yield return new WaitForSeconds(skill.vfxDuration);

    foreach (GameObject vfx in spawnedVFX)
        if (vfx != null) Destroy(vfx);
}
    

    private IEnumerator ExecuteAugmentSkill(ScriptableSkill skill)
    {
        List<HeroCharBase> targets = ResolveHeroTargets(skill);

        foreach (HeroCharBase targetHero in targets)
        {
            if (targetHero == null) continue;

            Debug.Log($"[AUGMENT READY] {gameObject.name} used {skill.skillName} on {targetHero.gameObject.name}.");
            yield return new WaitForSeconds(0.2f);
        }
    }

    private List<CharacterBase> ResolveEnemyTargets(ScriptableSkill skill)
    {
        List<CharacterBase> activeEnemies = CharacterManager.Instance.ActiveEnemies;
        List<CharacterBase> targets = new List<CharacterBase>();

        if (activeEnemies == null || activeEnemies.Count == 0) return targets;

        if (skill != null && skill.targetScope == TargetScope.All)
        {
            targets.AddRange(activeEnemies);
            return targets;
        }

        if (CurrentIntent.Target == null ||
            CurrentIntent.Target.currentHp <= 0 ||
            !CurrentIntent.Target.gameObject.activeInHierarchy ||
            !activeEnemies.Contains(CurrentIntent.Target))
        {
            CurrentIntent.Target = activeEnemies[0];
        }

        targets.Add(CurrentIntent.Target);
        return targets;
    }

    private List<HeroCharBase> ResolveHeroTargets(ScriptableSkill skill)
    {
        List<HeroCharBase> activeHeroes = BattleManager.Instance.GetActiveHeroes();
        List<HeroCharBase> targets = new List<HeroCharBase>();

        if (activeHeroes == null || activeHeroes.Count == 0) return targets;

        if (skill != null && skill.targetScope == TargetScope.All)
        {
            targets.AddRange(activeHeroes);
            return targets;
        }

        if (skill != null && skill.targetScope == TargetScope.Self)
        {
            targets.Add(this);
            return targets;
        }

        if (CurrentIntent.AllyTarget == null ||
            CurrentIntent.AllyTarget.currentHp <= 0 ||
            !CurrentIntent.AllyTarget.gameObject.activeInHierarchy ||
            !activeHeroes.Contains(CurrentIntent.AllyTarget))
        {
            CurrentIntent.AllyTarget = this;
        }

        targets.Add(CurrentIntent.AllyTarget);
        return targets;
    }

    private int CalculateRecoveryAmount(ScriptableSkill skill)
    {
        int skillPower = skill != null ? skill.basePower : 0;
        return Mathf.Max(1, Stats.Attack + skillPower);
    }

    public int CalculateDamagePerHit(CharacterBase targetEnemy, ScriptableSkill chosenSkill, out DamageEffectiveness effectiveness)
    {
        int attackerAtk = this.Stats.Attack; 
        int skillPower = chosenSkill != null ? chosenSkill.basePower : 0;
        float baseDamage = attackerAtk + skillPower;
        float multiplier = 1.0f;
        
        effectiveness = DamageEffectiveness.None;

        if (chosenSkill != null && chosenSkill.skillElement != null && targetEnemy is EnemyBase enemy)
        {
            SkillElement skillType = chosenSkill.skillElement.element;
            
            if (enemy.IsWeakTo(skillType)) 
            {
                multiplier = 1.5f; 
                effectiveness = DamageEffectiveness.Weak; 
                
                enemy.RevealWeakness(skillType); 
            }
            else if (enemy.IsResistantTo(skillType)) 
            {
                multiplier = 0.5f;
                effectiveness = DamageEffectiveness.Strong; 
            }
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
        return Mathf.Max(1, finalDamage); 
    }
    public override void EvaluateDeathStatus()
    {
        if (currentHp <= 0)
        {
            base.EvaluateDeathStatus();

            CharacterManager.Instance.HeroesPhysics.Remove(this);
            CharacterManager.Instance.ActiveHeroes.Remove(this);

            if (CharacterManager.Instance.HeroesPhysics.Count == 0)
            {
                BattleManager.Instance.ChangeState(BattleState.Defeat);
            }   
        }
    }

    private void OnAttackFinished()
    {
        currentBP -= allocatedBoost;
        allocatedBoost = 0;
        
        if (BoostVFXManager.Instance != null) BoostVFXManager.Instance.StopHeroEffect(this);
        
        List<CharacterBase> updatedEnemies = CharacterManager.Instance.ActiveEnemies; 
        currentIntent.ResetToDefault(updatedEnemies, _basicAttackSkill);
    }

    public void PlayVoice(AudioClip voiceClip)
    {
        if (voiceClip != null && charAudioSource != null)
        {
            charAudioSource.Stop(); 
            
            // Mainkan suara VA yang baru
            charAudioSource.clip = voiceClip;
            charAudioSource.Play();
        }
    }
}
