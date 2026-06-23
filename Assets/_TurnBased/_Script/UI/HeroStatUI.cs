using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))] 
public class HeroStatUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI spText;
    public TextMeshProUGUI intentText;
    public Image[] boostPoints;

    private ScriptableHero myHero;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnStatPanelClicked);
    }

    public void Init(ScriptableHero hero, int startingBoost)
    {
        myHero = hero;
        
        nameText.text = hero.heroName;
        hpText.text = hero.BaseStats.maxHp.ToString();
        spText.text = hero.BaseStats.maxSp.ToString();
        
        UpdateBoostVisual(startingBoost); 
    }

    public void SetIntentText(string newIntent)
    {
        if (intentText != null)
        {
            intentText.text = newIntent;
        }
    }

    public void UpdateBoostVisual(int currentBP)
    {
        for (int i = 0; i < boostPoints.Length; i++)
        {
            if (i < currentBP)
                boostPoints[i].color = Color.yellow;
            else
                boostPoints[i].color = Color.gray;
        }
    }
    private void OnStatPanelClicked()
    {
        if (myHero == null) return; 

        ActionMenuUI.Instance.OpenMenuForHero(myHero, this); 
    }
}