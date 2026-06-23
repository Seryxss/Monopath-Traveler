using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("Party Data")]
    public List<ScriptableHero> activeParty; 

    [Header("UI Panels")]
    public List<HeroStatUI> heroStatPanels; 

    private void Start() 
    {
        InitializePartyUI();
    }

    public void InitializePartyUI()
    {
        for (int i = 0; i < heroStatPanels.Count; i++)
        {
            if (i < activeParty.Count)
            {
                heroStatPanels[i].gameObject.SetActive(true);
                
                heroStatPanels[i].Init(activeParty[i], 1); //Tempat Initialize Boost brp banyak
            }
            else
            {
                heroStatPanels[i].gameObject.SetActive(false);
            }
        }
    }
}