using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyWeaknessUI : MonoBehaviour
{
    [Header("UI Setup")]
    [Tooltip("Parent GameObject yang memiliki komponen Horizontal Layout Group")]
    [SerializeField] private Transform iconContainer; 
    [Tooltip("Prefab UI Image kosong untuk spawn ikon")]
    [SerializeField] private GameObject iconPrefab;

    [Header("Visuals")]
    [Tooltip("Gambar 'Tanda Tanya' untuk kelemahan yang belum ditebak")]
    [SerializeField] private Sprite unknownWeaknessSprite; 

    private EnemyBase _enemy;
    private List<Image> spawnedIcons = new List<Image>();

    private void Awake()
    {
        _enemy = GetComponentInParent<EnemyBase>();
    }

    private void OnEnable()
    {
        if (_enemy != null)
        {
            // Dengarkan event ketika kelemahan baru ditebak
            _enemy.OnWeaknessDiscovered += RefreshIcons;
        }
    }

    private void OnDisable()
    {
        if (_enemy != null)
        {
            _enemy.OnWeaknessDiscovered -= RefreshIcons;
        }
    }

    // Kamu bisa memanggil fungsi ini dari EnemyHealthBar saat pertama kali inisialisasi
    public void SetupUI()
    {
        if (_enemy == null || _enemy.Weaknesses == null) return;

        // Bersihkan ikon lama
        foreach (Transform child in iconContainer) Destroy(child.gameObject);
        spawnedIcons.Clear();

        // Spawn ikon sebanyak jumlah kelemahan musuh
        foreach (ScriptableElement weakElement in _enemy.Weaknesses)
        {
            if (weakElement != null)
            {
                GameObject iconObj = Instantiate(iconPrefab, iconContainer);
                WeaknessIcon iconScript = iconObj.GetComponent<WeaknessIcon>();
spawnedIcons.Add(iconScript.iconImage);
            }
        }

        RefreshIcons(); 
    }

    private void RefreshIcons()
    {
        if (_enemy == null) return;

        // Cocokkan setiap ikon dengan data kelemahan
        for (int i = 0; i < _enemy.Weaknesses.Count; i++)
        {
            ScriptableElement currentElement = _enemy.Weaknesses[i];
            Image currentIcon = spawnedIcons[i];

            // Cek apakah elemen ini sudah pernah ditemukan pemain?
            if (_enemy.DiscoveredWeaknesses.Contains(currentElement.element))
            {
                currentIcon.sprite = currentElement.elementIcon;
            }
            else
            {
                currentIcon.sprite = unknownWeaknessSprite;
                // currentIcon.color = Color.gray; 
            }
        }
    }
}