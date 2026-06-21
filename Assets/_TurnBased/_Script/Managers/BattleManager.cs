using System;
using System.Collections;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public static event Action<BattleState> OnPreStateChange;
    public static event Action<BattleState> OnPostStateChange;

    public BattleState State { get; private set; }

    [Header("UI & Systems References")]
    public ActionMenuUI actionMenuPanel; 
    public TargetingSystem targetingSystem; 

    [Header("Stage References")]
    public Transform actionCenterPoint; 
    public float moveDuration = 0.3f;

    private CharacterBase currentActiveHero;
    private Vector3 originalHeroPos;

    private void OnEnable()
    {
        if (targetingSystem != null)
        {
            targetingSystem.OnTargetConfirmed += HandleAttackExecution;
            targetingSystem.OnTargetCanceled += HandleCancelTargeting;
        }
    }

    private void OnDisable()
    {
        if (targetingSystem != null)
        {
            targetingSystem.OnTargetConfirmed -= HandleAttackExecution;
            targetingSystem.OnTargetCanceled -= HandleCancelTargeting;
        }
    }

    void Start()
    {
        ChangeState(BattleState.SpawningHeroes);
    }

    public void ChangeState(BattleState newState)
    {
        if(State == newState) return;
        
        OnPreStateChange?.Invoke(newState);
        State = newState;

        switch (newState)
        {
            case BattleState.SpawningHeroes:
                HandleSpawningHeroes();
                break;
            case BattleState.SpawningEnemies:
                HandleSpawningEnemies();
                break;
            case BattleState.HeroTurn:
                HandleHeroTurn();
                break;
            case BattleState.EnemyTurn:
                HandleEnemyTurn();
                break;
            case BattleState.Win:
            case BattleState.Lose:
                break;
        }

        OnPostStateChange?.Invoke(newState);
    }

    private void HandleSpawningHeroes()
    {
        CharacterManager.Instance.SpawnHeroes(); 
        ChangeState(BattleState.SpawningEnemies);
    }

    private void HandleSpawningEnemies()
    {
        CharacterManager.Instance.SpawnEnemies(); 
        ChangeState(BattleState.HeroTurn);
    }

    private void HandleHeroTurn()
    {
        if (CharacterManager.Instance.ActiveHeroes.Count == 0) return;

        currentActiveHero = CharacterManager.Instance.ActiveHeroes[0];
        originalHeroPos = currentActiveHero.transform.position;
        
        Vector3 targetPos = actionCenterPoint.position;
        targetPos.y = originalHeroPos.y; 

        // Jalan maju, lalu munculkan Menu
        StartCoroutine(SmoothMove(currentActiveHero.transform, targetPos, () => 
        {
            if (actionMenuPanel != null) actionMenuPanel.Show();
        }));
    }

    public void OnAttackButtonTapped()
    {
        if (State != BattleState.HeroTurn) return;

        if (actionMenuPanel != null) actionMenuPanel.Hide();
        
        ChangeState(BattleState.SelectTarget); 
        targetingSystem.StartTargeting();      
    }

    private void HandleCancelTargeting()
    {
        if (State != BattleState.SelectTarget) return;

        targetingSystem.StopTargeting();
        ChangeState(BattleState.HeroTurn); 

        if (actionMenuPanel != null) actionMenuPanel.Show(); 
    }

    private void HandleAttackExecution(CharacterBase targetEnemy)
    {
        if (State != BattleState.SelectTarget) return;

        targetingSystem.StopTargeting(); 

        Debug.Log($"[BATTLE] {currentActiveHero.gameObject.name} Menyerang {targetEnemy.gameObject.name}!!");

        // Eksekusi jalan pulang
        StartCoroutine(SmoothMove(currentActiveHero.transform, originalHeroPos, () => 
        {
            ChangeState(BattleState.EnemyTurn);
        }));
    }

    private void HandleEnemyTurn()
    {
        Invoke(nameof(EndEnemyTurn), 1f);
    }

    private void EndEnemyTurn()
    {
        ChangeState(BattleState.HeroTurn);
    }

    private IEnumerator SmoothMove(Transform target, Vector3 endPos, Action onComplete)
    {
        Vector3 startPos = target.position;
        float elapsed = 0;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            target.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            yield return null;
        }
        target.position = endPos;
        onComplete?.Invoke();
    }
}