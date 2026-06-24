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

    [Header("UI & Systems References")]
    [SerializeField] private ActionMenuUI actionMenuPanel; 
    [SerializeField] private CommandButtonUI commandButtonPanel; 
    [SerializeField] private TargetingSystem targetingSystem;
    [SerializeField] private BattleUIManager battleUIManager;
    
    [Header("Audio")]
    [SerializeField] private AudioClip battleBGM;
    [SerializeField] private AudioClip battleWin;
    [SerializeField] private AudioClip battleLost;

    [Header("Stage References")]
    [SerializeField] private Transform actionCenterPoint; 
    public Vector3 ActionCenterPosition => actionCenterPoint.position;
    [SerializeField] private float moveDuration = 0.3f;
    private CharacterBase currentActiveHero;
    private Vector3 originalHeroPos;
    private HeroCharBase _heroBeingPlanned;

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
        if (battleBGM != null)
        {
            AudioSystem.Instance.PlayMusic(battleBGM);
        }
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
                AudioSystem.Instance.PlayMusic(battleWin);
                break;
            case BattleState.Lose:
                AudioSystem.Instance.PlayMusic(battleLost);
                break;
        }

        OnPostStateChange?.Invoke(newState);
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
        foreach (HeroCharBase hero in activeHeroes)
        {
            hero.InitializeTurnIntent(null); 
        }

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

    public void StartTargetingForHero(HeroCharBase hero, ScriptableSkill chosenSkill = null)
    {
        if (State != BattleState.HeroTurn) return;

        _heroBeingPlanned = hero;
        
        if (chosenSkill != null) 
        {
            _heroBeingPlanned.CurrentIntent.ChosenSkill = chosenSkill;
        }

        if (actionMenuPanel != null) actionMenuPanel.Hide();
        if (commandButtonPanel != null) commandButtonPanel.Hide(); 
        
        ChangeState(BattleState.SelectTarget); 
        targetingSystem.StartTargeting();      
    }

    private void HandleAttackExecution(CharacterBase targetEnemy)
    {
        if (State != BattleState.SelectTarget) return;

        targetingSystem.StopTargeting(); 

        // Simpan Target ke dalam memori pahlawan yang tadi minta
        if (_heroBeingPlanned != null)
        {
            _heroBeingPlanned.CurrentIntent.Target = targetEnemy;
            Debug.Log($"[PLANNING] {_heroBeingPlanned.gameObject.name} MENGUNCI TARGET ke {targetEnemy.gameObject.name}");
        }

        // KEMBALI KE FASE PERENCANAAN (Nunggu Command Execute Global ditekan)
        ChangeState(BattleState.HeroTurn); 

        // Munculkan kembali tombol Boost & Global Execute di bawah layar
        if (commandButtonPanel != null) commandButtonPanel.Show();
    }
    private void HandleCancelTargeting()
    {
        if (State != BattleState.SelectTarget) return;

        targetingSystem.StopTargeting();
        ChangeState(BattleState.HeroTurn); 

        // Kalau batal, munculkan lagi tombol global dan menu
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
