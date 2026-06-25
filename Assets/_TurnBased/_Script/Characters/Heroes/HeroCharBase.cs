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
        if (CurrentIntent.Target != null)
        {
            if (CurrentIntent.Target.currentHp <= 0 || !CurrentIntent.Target.gameObject.activeInHierarchy)
                CurrentIntent.Target = null;
        }

        if (CurrentIntent.Target == null)
        {
            List<CharacterBase> activeEnemies = CharacterManager.Instance.ActiveEnemies;
            if (activeEnemies != null && activeEnemies.Count > 0)
                CurrentIntent.Target = activeEnemies[0];
            else
            {
                OnAttackFinished();
                onComplete?.Invoke();
                yield break; 
            }
        }

        CharacterBase targetEnemy = CurrentIntent.Target;
        ScriptableSkill skill = CurrentIntent.ChosenSkill;

        if (targetEnemy != null)
        {
            Vector3 centerStagePos = BattleManager.Instance.ActionCenterPosition;
            yield return StartCoroutine(MoveToPosition(centerStagePos, 0.2f)); 

            if (skill != null && skill.spCost > 0)
            {
                ConsumeSP(skill.spCost);
            }

            int totalHits = 1 + allocatedBoost; 

            DamageEffectiveness actualEffectiveness;

            int damagePerHit = CalculateDamagePerHit(targetEnemy, skill, out actualEffectiveness);

            for (int i = 0; i < totalHits; i++)
            {
                targetEnemy.TakeDamage(damagePerHit, actualEffectiveness);
                
                yield return new WaitForSeconds(0.3f); 
            }

            if (targetEnemy is EnemyBase enemyBase) enemyBase.EvaluateDeathStatus();

            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(MoveToPosition(_originalStandPosition, 0.2f));
        }
        OnAttackFinished();
        onComplete?.Invoke();
    }

    public int CalculateDamagePerHit(CharacterBase targetEnemy, ScriptableSkill chosenSkill, out DamageEffectiveness effectiveness)
    {
        int attackerAtk = this.Stats.Attack; 
        int skillPower = chosenSkill != null ? chosenSkill.power : 0;
        float baseDamage = attackerAtk + skillPower;
        float multiplier = 1.0f;
        
        // Set status default menjadi None (Normal)
        effectiveness = DamageEffectiveness.None;

        if (chosenSkill != null && targetEnemy is EnemyBase enemy)
        {
            DamageType skillType = chosenSkill.damageType;
            
            // Cek data asli dari musuh
            if (enemy.Weaknesses != null && enemy.Weaknesses.Contains(skillType)) 
            {
                multiplier = 1.5f; 
                effectiveness = DamageEffectiveness.Weak; // Laporkan status WEAK
            }
            else if (enemy.Resistances != null && enemy.Resistances.Contains(skillType)) 
            {
                multiplier = 0.5f;
                effectiveness = DamageEffectiveness.Strong; // Laporkan status RESIST
            }
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
        return Mathf.Max(1, finalDamage); // Minimal damage adalah 1
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
                Debug.Log("=================================");
                Debug.Log("[BATTLE LOSE] SEMUA HERO TELAH TUMBANG!");
                Debug.Log("=================================");
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
}