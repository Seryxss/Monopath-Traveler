using UnityEngine;
using UnityEngine.EventSystems; // Wajib untuk sistem Hover
using TMPro; // Gunakan ini jika memakai TextMeshPro

public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Skill Info")]
    public string skillName = "Attack";
    [TextArea] public string skillDescription = "Menyerang satu target dengan senjata fisik.";

    [Header("UI References")]
    [Tooltip("Tarik teks deskripsi yang ada di Canvas ke sini")]
    public TextMeshProUGUI descriptionTextUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionTextUI != null)
        {
            descriptionTextUI.text = $"<b>{skillName}</b>\n{skillDescription}";
        }
    }

    // Fungsi ini otomatis terpanggil saat Mouse keluar dari area tombol
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionTextUI != null)
        {
            descriptionTextUI.text = ""; // Kosongkan teks
        }
    }
}