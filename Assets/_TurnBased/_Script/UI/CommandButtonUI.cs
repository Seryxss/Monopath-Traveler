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

    private void OnExecuteClicked()
    {
        if (_attackButton != null) _attackButton.interactable = false;

        BattleManager.Instance.ExecuteAllHeroesActions();
    }

    // --- FUNGSI BARU UNTUK BATTLE MANAGER ---

    public void Show()
    {
        // 1. Reset saklar boost ke mati setiap kali giliran baru mulai
        _isMaxBoostActive = false; 
        
        // 2. Pastikan tombol attack bisa dipencet lagi
        if (_attackButton != null) _attackButton.interactable = true;
        
        // 3. Munculkan panel
        if (_panelHolder != null) _panelHolder.SetActive(true);
        else gameObject.SetActive(true); 
    }
    

    public void Hide()
    {
        if (_panelHolder != null) _panelHolder.SetActive(false);
        else gameObject.SetActive(false);
    }
}