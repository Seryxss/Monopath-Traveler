using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class EnemyBase : CharacterBase
{
    [Header("Elemental Affinities")]
    [SerializeField] private List<DamageType> _weaknesses;
    [SerializeField] private List<DamageType> _resistances;

    public List<DamageType> Weaknesses => _weaknesses;
    public List<DamageType> Resistances => _resistances;

    protected override void Awake() 
    {
        base.Awake(); 
        BattleManager.OnPreStateChange += OnStateChanged;
    }

    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;

    private void OnStateChanged(BattleState newState)
    {
        if (!gameObject.activeInHierarchy || currentHp <= 0) return;

        if (newState == BattleState.EnemyTurn)
        {
            StartCoroutine(ExecuteAITurn());
        }
    }

    public override void EvaluateDeathStatus()
    {
        base.EvaluateDeathStatus(); 

        if (currentHp <= 0)
        {
            gameObject.SetActive(false); 
            
            if (CharacterManager.Instance.ActiveEnemies.Contains(this))
            {
                CharacterManager.Instance.ActiveEnemies.Remove(this);
            }
            BattleManager.Instance.CheckBattleEnd();
        }
    }

    private IEnumerator ExecuteAITurn()
    {
        Debug.Log($"{gameObject.name} sedang memikirkan target...");
        
        List<HeroCharBase> activeHeroes = BattleManager.Instance.GetActiveHeroes();
        if (activeHeroes == null || activeHeroes.Count == 0) yield break;

        HeroCharBase targetHero = activeHeroes[Random.Range(0, activeHeroes.Count)];

        Vector3 centerStagePos = BattleManager.Instance.ActionCenterPosition;

        yield return StartCoroutine(MoveToPosition(centerStagePos, 0.2f));

        yield return new WaitForSeconds(0.3f); 

        int damage = Stats.Attack;
        targetHero.TakeDamage(damage);
        Debug.Log($"[ENEMY ATTACK] {gameObject.name} menyerang {targetHero.gameObject.name} dengan {damage} damage!");

        targetHero.EvaluateDeathStatus();

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveToPosition(_originalStandPosition, 0.2f));

        Debug.Log($"Giliran {gameObject.name} selesai.");
        
        BattleManager.Instance.ChangeState(BattleState.HeroTurn); 
    }
}