using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;
    [Header("UI Panels")]
    [SerializeField] private List<HeroStatUI> heroStatPanels; 
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
                
                // Init UI dengan data, fisik, dan baterai BP bawaan dari fisik
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
}
