using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandButtonUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject _panelHolder;
    [SerializeField] private Button _boostAllButton;
    [SerializeField] private Button _attackButton;

    private bool _isMaxBoostActive = false;

    private void Awake()
    {
        if (_boostAllButton != null)
        {
            _boostAllButton.onClick.AddListener(OnBoostAllClicked);
        }

        if (_attackButton != null)
        {
            _attackButton.onClick.AddListener(OnExecuteClicked);
        }
    }

    private void OnBoostAllClicked()
    {
        _isMaxBoostActive = !_isMaxBoostActive;

        List<HeroCharBase> partyMembers = BattleManager.Instance.GetActiveHeroes();
        if (partyMembers == null) return;

        foreach (HeroCharBase hero in partyMembers)
        {
            int prevBoost = hero.AllocatedBoost;

            if (_isMaxBoostActive) hero.AllocatedBoost = Mathf.Min(hero.CurrentBP, 3);
            else hero.AllocatedBoost = 0;

            hero.CurrentIntent.BoostAmount = hero.AllocatedBoost;

            if (BoostVFXManager.Instance != null)
            {
                BoostVFXManager.Instance.PlayBoostEffect(hero, hero.AllocatedBoost, prevBoost);
            }
        }

        if (BattleUIManager.Instance != null)
        {
            BattleUIManager.Instance.RefreshAllBoostVisuals();
        }
    }

    public void RefreshBoostAllButtonState()
    {
        if (_boostAllButton == null) return;

        bool canAnyoneBoost = false;
        List<HeroCharBase> activeHeroes = BattleManager.Instance.GetActiveHeroes();

        foreach (HeroCharBase hero in activeHeroes)
        {
            if (hero.AllocatedBoost < BattleManager.MAX_BOOST && hero.AllocatedBoost < hero.CurrentBP)
            {
                canAnyoneBoost = true;
                break;
            }
        }
        _boostAllButton.interactable = canAnyoneBoost;
    }

    private void OnExecuteClicked()
    {
        if (_attackButton != null) _attackButton.interactable = false;

        BattleManager.Instance.ExecuteAllHeroesActions();
    }

    public void Show()
    {
        _isMaxBoostActive = false; 
        
        if (_attackButton != null) _attackButton.interactable = true;
        
        if (_panelHolder != null) _panelHolder.SetActive(true);
        else gameObject.SetActive(true); 
    }
    

    public void Hide()
    {
        if (_panelHolder != null) _panelHolder.SetActive(false);
        else gameObject.SetActive(false);
    }
}