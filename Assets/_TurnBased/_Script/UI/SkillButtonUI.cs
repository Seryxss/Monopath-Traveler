using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; 

public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Skill Info")]
    [SerializeField] private TextMeshProUGUI typeText;    
    [SerializeField] private TextMeshProUGUI nameText;    
    [SerializeField] private TextMeshProUGUI spText;      

    [Header("Description Box")]
    [SerializeField] private GameObject descriptionBox;       
    [SerializeField] private TextMeshProUGUI descriptionText; 

    private ScriptableSkill mySkill; 

    public void Setup(ScriptableSkill skill)
    {
        mySkill = skill;

        // Pastikan deskripsi mati saat tombol baru dicetak
        if (descriptionBox != null) descriptionBox.SetActive(false);

        if (skill == null) return;

        if (typeText != null) typeText.text = $"{skill.targetType} / {skill.damageType}";
        if (nameText != null) nameText.text = skill.skillName;
        if (descriptionText != null) descriptionText.text = skill.description; 

        if (spText != null)
        {
            if (skill.spCost <= 0) spText.transform.parent.gameObject.SetActive(false);
            else
            {
                spText.transform.parent.gameObject.SetActive(true);
                spText.text = $"SP {skill.spCost}";
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. Munculkan deskripsi
        if (mySkill != null && descriptionBox != null)
        {
            descriptionBox.SetActive(true); 
        }
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    // --- MURNI HANYA SAAT MOUSE KELUAR ---
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionBox != null)
        {
            descriptionBox.SetActive(false); 
        }
    }
}
