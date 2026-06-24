using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager>
{
    public static event Action<BattleState> OnPreStateChange;
    public static event Action<BattleState> OnPostStateChange;
    public BattleState State { get; private set; }
    public const int MAX_BOOST = 3;

    [Header("UI & Systems References")]
    [SerializeField] private ActionMenuUI actionMenuPanel; 
    [SerializeField] private CommandButtonUI commandButtonPanel; 
    [SerializeField] private TargetingSystem targetingSystem;
    [SerializeField] private BattleUIManager battleUIManager;
    [SerializeField] private BattleResultUI battleResultUI;
    
    [Header("Audio")]
    [SerializeField] private AudioClip battleBGM;
    [SerializeField] private AudioClip battleVictory;
    [SerializeField] private AudioClip battleDefeat;

    [Header("Stage References")]
    [SerializeField] private Transform actionCenterPoint; 
    public Vector3 ActionCenterPosition => actionCenterPoint.position;
    [SerializeField] private float moveDuration = 0.3f;
    private CharacterBase currentActiveHero;
    private Vector3 originalHeroPos;
    private HeroCharBase _heroBeingPlanned;
    private bool _isFirstTurn = true;

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
        AudioSystem.Instance.PlayMusic(battleBGM);
        
    }
    

    public void ChangeState(BattleState newState)
    {
        // Gembok pengaman (Pastikan ini masih ada)
        if (State == BattleState.Victory || State == BattleState.Defeat) return; 

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

            case BattleState.Victory:
                StartCoroutine(ShowResultUIDelayed(true)); 
                break;
            case BattleState.Defeat:
                StartCoroutine(ShowResultUIDelayed(false)); 
                break;
        }

        OnPostStateChange?.Invoke(newState);
    }

    private IEnumerator ShowResultUIDelayed(bool isVictory)
    {
        Debug.Log(isVictory ? "[BATTLE] KEMENANGAN! Menunggu UI..." : "[BATTLE] KALAH! Menunggu UI...");
        
        if (actionMenuPanel != null) actionMenuPanel.Hide();
        if (commandButtonPanel != null) commandButtonPanel.Hide();
        
        yield return new WaitForSeconds(2.0f);

        if (battleUIManager != null) battleUIManager.gameObject.SetActive(false);

        if (battleResultUI != null)
        {
            if (isVictory) {
                battleResultUI.ShowVictory();
                if (battleVictory != null) AudioSystem.Instance.PlayMusic(battleVictory);
            }
            else 
            {
                battleResultUI.ShowDefeat();
                if (battleDefeat != null) AudioSystem.Instance.PlayMusic(battleDefeat);
            }
        }
    }

    private void HandleSpawningHeroes()
    {
        CharacterManager.Instance.SpawnHeroes(); 

        List<HeroType> partyTypes = GameManager.Instance.CurrentParty;
        List<ScriptableHero> partyData = new List<ScriptableHero>();

        if (partyTypes == null || partyTypes.Count == 0)
        {
            partyData.Add(ResourceSystem.Instance.GetHero(HeroType.Warrior));
        }
        else
        {
            foreach (HeroType heroType in partyTypes)
            {
                partyData.Add(ResourceSystem.Instance.GetHero(heroType));
            }
        }

        if (battleUIManager != null)
        {
            battleUIManager.SetupPartyUI(partyData, CharacterManager.Instance.HeroesPhysics);
        }

        ChangeState(BattleState.SpawningEnemies);
    }

    private void HandleSpawningEnemies()
    {
        CharacterManager.Instance.SpawnEnemies(); 
        ChangeState(BattleState.HeroTurn);
    }

    private void HandleHeroTurn()
    {
        List<HeroCharBase> activeHeroes = GetActiveHeroes();
        if (activeHeroes == null || activeHeroes.Count == 0) return;

        if (_isFirstTurn)
        {
            _isFirstTurn = false; 
        }
        else
        {
            foreach (HeroCharBase hero in activeHeroes)
            {
                hero.CurrentBP = Mathf.Min(hero.CurrentBP + 1, 5); 
            }
            Debug.Log("[BATTLE] Putaran Baru. Semua Hero mendapat +1 BP.");
        }

        foreach (HeroCharBase hero in activeHeroes)
        {
            hero.InitializeTurnIntent(null); 
        }

        if (battleUIManager != null) battleUIManager.RefreshAllBoostVisuals();
        if (commandButtonPanel != null) commandButtonPanel.Show(); 
        
        Debug.Log("[BATTLE] Fase Perencanaan Dimulai.");
    }

    public void ExecuteAllHeroesActions()
    {
        if (State != BattleState.HeroTurn) return;

        if (actionMenuPanel != null) actionMenuPanel.Hide();
        
        if (commandButtonPanel != null) commandButtonPanel.Hide();

        StartCoroutine(ExecutionSequenceCoroutine());
    }

    private IEnumerator ExecutionSequenceCoroutine()
    {
        List<HeroCharBase> activeHeroes = GetActiveHeroes(); 

        foreach (HeroCharBase hero in activeHeroes)
        {
            if (hero == null || hero.currentHp <= 0) continue;

            bool isHeroDone = false;

            hero.ExecuteMove(() => isHeroDone = true);

            yield return new WaitUntil(() => isHeroDone);

            yield return new WaitForSeconds(0.15f);
        }

        ChangeState(BattleState.EnemyTurn);
    }
    
    private void HandleAttackExecution(CharacterBase targetEnemy)
    {
        if (State != BattleState.SelectTarget) return;

        if (_heroBeingPlanned != null)
        {
            _heroBeingPlanned.CurrentIntent.Target = targetEnemy;
            Debug.Log($"[PLANNING] {_heroBeingPlanned.gameObject.name} MENGUNCI TARGET ke {targetEnemy.gameObject.name}");
        }

        if (actionMenuPanel != null) actionMenuPanel.Hide(); 
    }

    public void StartTargetingForHero(HeroCharBase hero)
    {
        if (State != BattleState.HeroTurn && State != BattleState.SelectTarget) return;

        _heroBeingPlanned = hero;
        
        if (_heroBeingPlanned.CurrentIntent.ChosenSkill == null)
            _heroBeingPlanned.CurrentIntent.ChosenSkill = _heroBeingPlanned.CurrentIntent.BasicAttackSkill;

        ChangeState(BattleState.SelectTarget); 

        if (commandButtonPanel != null) commandButtonPanel.Hide();
        
        targetingSystem.StartTargeting(_heroBeingPlanned.CurrentIntent.Target);      
    }

    public void StopTargetingFromMenu()
    {
        if (State == BattleState.SelectTarget)
        {
            if (_heroBeingPlanned != null && targetingSystem != null)
            {
                _heroBeingPlanned.CurrentIntent.Target = targetingSystem.GetCurrentTarget();
            }
            targetingSystem.StopTargeting();
            State = BattleState.HeroTurn;

            if (commandButtonPanel != null) commandButtonPanel.Show();
        }
    }

    private void HandleCancelTargeting()
    {
        if (State != BattleState.SelectTarget) return;

        targetingSystem.StopTargeting();
        
        State = BattleState.HeroTurn; 

        if (actionMenuPanel != null) actionMenuPanel.Show(); 
        if (commandButtonPanel != null) commandButtonPanel.Show();
    }

    private void HandleEnemyTurn()
    {
        Invoke(nameof(EndEnemyTurn), 1f);
    }

    private void EndEnemyTurn()
    {
        ChangeState(BattleState.HeroTurn);
    }

    public void CheckBattleEnd()
    {
        if (CharacterManager.Instance.ActiveEnemies.Count == 0)
        {
            // Panggil secara resmi agar masuk ke Switch Case
            ChangeState(BattleState.Victory); 
            return;
        }

        // 2. Cek Kekalahan
        if (CharacterManager.Instance.ActiveHeroes.Count == 0)
        {
            ChangeState(BattleState.Defeat);
            return;
        }
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

    public List<HeroCharBase> GetActiveHeroes()
    {
        return CharacterManager.Instance.HeroesPhysics; 
    }
}
