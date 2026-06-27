using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("Menu Panels")]
    [SerializeField] private ActionMenuUI actionMenuPanel; 
    [SerializeField] private CommandButtonUI commandButtonPanel; 
    [SerializeField] private BattleResultUI battleResultUI;
    [SerializeField] private TurnQueueUI turnQueueUI;

    [Header("Hero Stats Panels")]
    [SerializeField] private List<HeroStatUI> heroStatPanels;

    [Header("UI Transition Settings (Fade)")]
    [SerializeField] private CanvasGroup[] battleUIGroups;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // ─── Party Setup ──────────────────────────────────────────────────────────

    public void SetupPartyUI(List<ScriptableHero> partyData, List<HeroCharBase> physicalUnits)
    {
        for (int i = 0; i < heroStatPanels.Count; i++)
        {
            if (i < partyData.Count && i < physicalUnits.Count)
            {
                heroStatPanels[i].Init(partyData[i], physicalUnits[i], physicalUnits[i].CurrentBP);
                heroStatPanels[i].ShowPanel(); 
            }
            else
            {
                heroStatPanels[i].HidePanel(); 
            }
        }
    }

    // ─── Turn Queue ───────────────────────────────────────────────────────────

    // ─── Turn Queue ───────────────────────────────────────────────────────────

    public void BuildTurnQueue(List<CharacterBase> currentRound, List<CharacterBase> nextRound)
    {
        if (turnQueueUI != null) turnQueueUI.BuildQueue(currentRound, nextRound);
    }

    public void AdvanceTurnQueue(CharacterBase actingCharacter)
    {
        if (turnQueueUI != null) turnQueueUI.AdvanceTurn(actingCharacter);
    }

    public void BeginExecutionQueueVisual()
    {
        if (turnQueueUI != null) turnQueueUI.BeginExecution();
    }

    // ─── Boost ────────────────────────────────────────────────────────────────

    public void RefreshAllBoostVisuals()
    {
        // Iterasi ActivePanels (hanya panel yang visible) — lebih efisien
        foreach (HeroStatUI panel in HeroStatUI.ActivePanels)
        {
            if (panel != null && panel.heroChar != null)
                panel.UpdateBoostVisual(); 
        }
    }

    // ─── Panel Show / Hide ────────────────────────────────────────────────────

    public void ShowCommandPanel() { if (commandButtonPanel != null) commandButtonPanel.ShowCommands(); }
    public void HideCommandPanel() { if (commandButtonPanel != null) commandButtonPanel.HideCommands(); }
    public void CloseActionMenu() { if (actionMenuPanel   != null) actionMenuPanel.CloseMenu(); }

    public void ShowVictoryScreen() 
    {
        HideAllBattleUI();
        if (battleResultUI != null) battleResultUI.ShowVictory();
    }

    public void ShowDefeatScreen() 
    {
        HideAllBattleUI();
        if (battleResultUI != null) battleResultUI.ShowDefeat();
    }

    public void HideAllBattleUI()
    {
        foreach (CanvasGroup cg in battleUIGroups)
        {
            if (cg != null) SetGroupVisible(cg, false);
        }

        // Sembunyikan semua HeroStatUI yang sedang aktif
        foreach (HeroStatUI panel in heroStatPanels)
        {
            if (panel != null) panel.HidePanel();
        }
    }
    public void ShowAllForTransition()
    {
        foreach (CanvasGroup cg in battleUIGroups)
        {
            if (cg != null) SetGroupVisible(cg, true);
        }

        foreach (HeroStatUI panel in heroStatPanels)
        {
            if (panel != null && panel.heroChar != null) panel.ShowPanel();
        }
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    private static void SetGroupVisible(CanvasGroup cg, bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}