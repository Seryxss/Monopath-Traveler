using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; 

// Tambahkan ISelectHandler dan IDeselectHandler untuk support Keyboard/Gamepad!
public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Skill Info")]
    [SerializeField] private TextMeshProUGUI typeText;    
    [SerializeField] private TextMeshProUGUI nameText;    
    [SerializeField] private TextMeshProUGUI spText;      

    [Header("Description Box")]
    [SerializeField] private GameObject descriptionBox;       
    [SerializeField] private TextMeshProUGUI descriptionText; 

    private ScriptableSkill mySkill; 

    public void Setup(ScriptableSkill skill, int currentSp)
    {
        mySkill = skill;
        if (descriptionBox != null) descriptionBox.SetActive(false);
        if (skill == null) return;

        if (typeText != null) typeText.text = $"{skill.targetType} / {skill.damageType}";
        if (nameText != null) nameText.text = skill.skillName;
        if (descriptionText != null) descriptionText.text = skill.description; 

        if (spText != null)
        {
            if (skill.spCost <= 0) 
            {
                spText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                spText.transform.parent.gameObject.SetActive(true);
                spText.text = $"SP {skill.spCost}";

                if (currentSp < skill.spCost) 
                {
                    spText.color = Color.red;
                }
                else 
                {
                    spText.color = Color.white;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Langsung munculkan deskripsi saat di-hover
        if (mySkill != null && descriptionBox != null) descriptionBox.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // MUTLAK: Matikan deskripsi saat mouse keluar agar tidak ada 2 box yang nyala!
        if (descriptionBox != null) descriptionBox.SetActive(false); 
    }

    // --- INTERAKSI KEYBOARD / GAMEPAD ---
    public void OnSelect(BaseEventData eventData)
    {
        // if (mySkill != null && descriptionBox != null) descriptionBox.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (descriptionBox != null) descriptionBox.SetActive(false);
    }
}