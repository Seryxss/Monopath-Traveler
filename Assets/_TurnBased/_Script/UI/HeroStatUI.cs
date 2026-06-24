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
        
        UpdateBoostVisual(); 
    }

    public void SetIntentText(string newIntent)
    {
        if (intentText != null) intentText.text = newIntent;
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
}
