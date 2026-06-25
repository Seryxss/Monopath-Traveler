using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Button))] 
public class HeroStatUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI spText;
    [SerializeField] private TextMeshProUGUI intentText;
    [SerializeField] private Image[] boostPoints;

    [Header("HP Bar Visuals")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image spFillImage;

    public static readonly List<HeroStatUI> ActivePanels = new List<HeroStatUI>();
    private void OnEnable() => ActivePanels.Add(this);  
    private void OnDisable() => ActivePanels.Remove(this); 
    public int currentBP { get; private set; } 
    public string IntentTextValue => intentText != null ? intentText.text : "";
    [SerializeField] private int allocatedBoost = 0;
    public int AllocatedBoost 
    { 
        get => allocatedBoost;
        set => allocatedBoost = value;
    }
    public HeroCharBase physicalHero { get; private set; }
    private ScriptableHero myHero;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnStatPanelClicked);
    }

    public void Init(ScriptableHero heroData, HeroCharBase heroUnit, int startingBoost)
    {
        myHero = heroData;
        physicalHero = heroUnit;
        
        currentBP = startingBoost; 
        physicalHero.AllocatedBoost = 0;
        
        nameText.text = heroData.heroName;
        hpText.text = heroData.BaseStats.maxHp.ToString();
        spText.text = heroData.BaseStats.maxSp.ToString();

        if (physicalHero != null)
        {
            physicalHero.OnHealthChanged += UpdateHPVisuals;
            physicalHero.OnSpChanged += UpdateSPVisuals;
            
            UpdateHPVisuals(physicalHero.currentHp, physicalHero.Stats.maxHp);
            UpdateSPVisuals(physicalHero.currentSp, physicalHero.Stats.maxSp);
        }
        
        UpdateBoostVisual(); 
    }

    public void SetIntentText(string newIntent)
    {
        if (intentText != null) intentText.text = newIntent;
    }
    private void OnDestroy()
    {
        if (physicalHero != null)
        {
            physicalHero.OnHealthChanged -= UpdateHPVisuals;
        }
    }
    public void UpdateBoostVisual()
    {
        int actualBP = physicalHero.CurrentBP; 
        int usedBoost = physicalHero.AllocatedBoost;

        int remainingBP = actualBP - usedBoost;

        for (int i = 0; i < boostPoints.Length; i++)
        {
            if (i < remainingBP) boostPoints[i].color = Color.yellow;
            else boostPoints[i].color = Color.gray;
        }
    }

    private void OnStatPanelClicked()
    {
        if (myHero == null) return; 
        
        ActionMenuUI.Instance.OpenMenuForHero(myHero, this); 
    }

    private void UpdateHPVisuals(int currentHp, int maxHp)
    {
        if (hpText != null)
        {
            hpText.text = currentHp.ToString(); 
        }

        if (hpFillImage != null)
        {
            float healthPercentage = (float)currentHp / maxHp; 
            hpFillImage.fillAmount = healthPercentage; 
            
            if (healthPercentage > 0.5f) 
            {
                hpFillImage.color = Color.green; 
            }
            else if (healthPercentage > 0.25f) 
            {
                hpFillImage.color = new Color(0.8f, 0.7f, 0f); 
            }
            else 
            {
                hpFillImage.color = Color.red; 
            }
        }
    }

    private void UpdateSPVisuals(int currentSp, int maxSp)
    {
        if (spText != null) spText.text = currentSp.ToString(); 
        
        if (spFillImage != null) 
        {
            spFillImage.fillAmount = (float)currentSp / maxSp; 
        }
    }
}
