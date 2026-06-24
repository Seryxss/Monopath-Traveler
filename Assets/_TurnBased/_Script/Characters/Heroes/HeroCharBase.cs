using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class HeroCharBase : CharacterBase
{
    [Header("Combat Planning")]
    [SerializeField] private ActionIntent currentIntent = new ActionIntent();
    private SnapToGround _snapToGround;

    [Header("Boost State")]
    private int currentBP = 2; 
    private int allocatedBoost = 0;
    public int AllocatedBoost 
    { 
        get => allocatedBoost;
        set => allocatedBoost = value;
    }

    [Header("Default Actions")]
    public ActionIntent CurrentIntent => currentIntent;
    public int CurrentBP => currentBP;
    private Vector3 _originalStandPosition;
    protected override void Awake()
    {
        base.Awake(); 
        BattleManager.OnPreStateChange += OnStateChanged;

        _snapToGround = GetComponent<SnapToGround>(); 
    }
    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;
    private void OnStateChanged(BattleState newState)
    {
        if (newState == BattleState.HeroTurn) return;
    }

    public void InitializeTurnIntent(CharacterBase defaultEnemy)
    {
        List<CharacterBase> activeEnemies = CharacterManager.Instance.ActiveEnemies;
        
        currentIntent.ResetToDefault(activeEnemies);
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
        _originalStandPosition = transform.position; 
        StartCoroutine(AttackSequenceCoroutine(onComplete));
    }

    private IEnumerator AttackSequenceCoroutine(Action onComplete)
    {
        if (CurrentIntent.Target != null)
        {
            if (CurrentIntent.Target.currentHp <= 0 || !CurrentIntent.Target.gameObject.activeInHierarchy)
            {
                CurrentIntent.Target = null;
            }
        }

        if (CurrentIntent.Target == null)
        {
            List<CharacterBase> activeEnemies = CharacterManager.Instance.ActiveEnemies;

            if (activeEnemies != null && activeEnemies.Count > 0)
            {
                CurrentIntent.Target = activeEnemies[0];
                Debug.Log($"[RETARGET] {gameObject.name} pindah target ke {CurrentIntent.Target.name}!");
            }
            else
            {
                Debug.Log($"[BATAL SERANG] Semua musuh mati. {gameObject.name} batal maju.");
                OnAttackFinished();
                onComplete?.Invoke();
                yield break; 
            }
        }

        CharacterBase targetEnemy = CurrentIntent.Target;
        ScriptableSkill skill = CurrentIntent.ChosenSkill;

        Debug.Log(BattleManager.Instance.ActionCenterPosition);

        if (targetEnemy != null)
        {
            Vector3 centerStagePos = BattleManager.Instance.ActionCenterPosition;
            // centerStagePos.z = transform.position.z; 

            yield return StartCoroutine(MoveToPosition(centerStagePos, 0.2f)); 

            int totalHits = 1 + allocatedBoost; 
            int damagePerHit = CalculateDamagePerHit(targetEnemy, skill);
            
            for (int i = 0; i < totalHits; i++)
            {
                targetEnemy.TakeDamage(damagePerHit);
                Debug.Log($"Hit {i + 1}: Memberikan {damagePerHit} damage ke {targetEnemy.gameObject.name}");
                
                yield return new WaitForSeconds(0.3f); 
            }

            if (targetEnemy is EnemyBase enemy) enemy.EvaluateDeathStatus(); 

            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(MoveToPosition(_originalStandPosition, 0.2f));
        }
        OnAttackFinished();
        onComplete?.Invoke();
    }

    public int CalculateDamagePerHit(CharacterBase targetEnemy, ScriptableSkill chosenSkill)
    {
        int attackerAtk = this.Stats.Attack; 
        
        int skillPower = chosenSkill != null ? chosenSkill.power : 0;
        float baseDamage = attackerAtk + skillPower;

        float multiplier = 1.0f;

        if (chosenSkill != null && targetEnemy is EnemyBase enemy)
        {
            DamageType skillType = chosenSkill.damageType;

            if (enemy.Weaknesses != null && enemy.Weaknesses.Contains(skillType))
            {
                multiplier = 1.5f; // Weakness = x1.5 Damage
                
                // --- BONUS UX ---
                // Anda bisa memanggil sistem UI/Audio di sini!
                // misal: DamagePopupManager.Instance.ShowWeaknessText(enemy.transform);
                // AudioSystem.Instance.PlaySFX(weaknessHitClip);
            }
            else if (enemy.Resistances != null && enemy.Resistances.Contains(skillType))
            {
                multiplier = 0.5f;
            }
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
        return Mathf.Max(1, finalDamage);
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration); 
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            if (_snapToGround != null)
            {
                _snapToGround.Snap();
            }

            yield return null;
        }

        transform.position = targetPos;
        if (_snapToGround != null) _snapToGround.Snap();
    }

    private void OnAttackFinished()
    {
        currentBP -= allocatedBoost;
        allocatedBoost = 0;
        
        if (BoostVFXManager.Instance != null)
        {
            BoostVFXManager.Instance.StopHeroEffect(this);
        }
        
        List<CharacterBase> updatedEnemies = CharacterManager.Instance.ActiveEnemies; 
        
        currentIntent.ResetToDefault(updatedEnemies);
    }
}