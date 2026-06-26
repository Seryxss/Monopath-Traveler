using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("Menu Panels")]
    [SerializeField] private ActionMenuUI actionMenuPanel; 
    [SerializeField] private CommandButtonUI commandButtonPanel; 
    [SerializeField] private BattleResultUI battleResultUI;

    [Header("Hero Stats Panels")]
    [SerializeField] private List<HeroStatUI> heroStatPanels; 

    [Header("UI Transition Settings (Fade)")]
    [Tooltip("Masukkan semua Canvas/Panel UI yang harus hilang saat Fade (Action Menu, Stats, Command Panel, dll)")]
    [SerializeField] private GameObject[] battleUIPanels;
    
    [Tooltip("Masukkan Panel Win dan Lose ke sini agar tidak ikut tersembunyi secara paksa")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SetupPartyUI(List<ScriptableHero> partyData, List<HeroCharBase> physicalUnits)
    {
        for (int i = 0; i < heroStatPanels.Count; i++)
        {
            if (i < partyData.Count && i < physicalUnits.Count)
            {
                heroStatPanels[i].gameObject.SetActive(true);
                ScriptableHero data = partyData[i];
                HeroCharBase physicUnit = physicalUnits[i]; 
                heroStatPanels[i].Init(data, physicUnit, physicUnit.CurrentBP); 
            }
            else
            {
                heroStatPanels[i].gameObject.SetActive(false);
            }
        }
    }

    public void RefreshAllBoostVisuals()
    {
        foreach (HeroStatUI panel in heroStatPanels)
        {
            if (panel != null && panel.gameObject.activeInHierarchy && panel.physicalHero != null)
            {
                panel.UpdateBoostVisual(); 
            }
        }
    }
    public void ShowCommandPanel() { if (commandButtonPanel != null) commandButtonPanel.Show(); }
    public void HideCommandPanel() { if (commandButtonPanel != null) commandButtonPanel.Hide(); }
    
    public void HideActionMenu() { if (actionMenuPanel != null) actionMenuPanel.Hide(); }

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
        foreach (GameObject panel in battleUIPanels)
        {
            if (panel != null) panel.SetActive(false);
        }

        if (BattleManager.Instance.State != BattleState.Victory && victoryPanel != null) 
            victoryPanel.SetActive(false);
            
        if (BattleManager.Instance.State != BattleState.Defeat && defeatPanel != null) 
            defeatPanel.SetActive(false);
    }

    public void ShowUIAfterTransition()
    {
        foreach (GameObject panel in battleUIPanels)
        {
            if (panel != null) panel.SetActive(true);
        }
    }
}