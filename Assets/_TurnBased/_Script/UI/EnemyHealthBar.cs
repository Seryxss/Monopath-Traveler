using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Masukkan komponen Image yang menjadi isi (fill) darah musuh")]
    [SerializeField] private Image hpFillImage;
    
    [Header("Behavior")]
    [Tooltip("Matikan bar jika darah penuh?")]
    [SerializeField] private bool hideWhenFull = false;

    private CharacterBase _character;
    private Canvas _canvas;

    private void Awake()
    {
        // Cari komponen CharacterBase di parent (GenericEnemy)
        _character = GetComponentInParent<CharacterBase>();
        _canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        if (_character != null)
        {
            // Daftarkan fungsi UpdateBar ke sistem Event milik karakter
            _character.OnHealthChanged += UpdateBar;
        }
    }

    private void OnDisable()
    {
        if (_character != null)
        {
            // Cabut pendaftaran saat musuh mati/hilang agar tidak error memory leak
            _character.OnHealthChanged -= UpdateBar;
        }
    }

    private void UpdateBar(int currentHp, int maxHp)
    {
        if (hpFillImage == null || maxHp <= 0) return;

        float healthPercentage = (float)currentHp / maxHp;
        hpFillImage.fillAmount = healthPercentage;

        // --- LOGIKA PERUBAHAN WARNA ---
        if (healthPercentage > 0.5f) 
        {
            hpFillImage.color = Color.green; // Di atas 50%: Hijau Normal
        }
        else if (healthPercentage > 0.25f) 
        {
            // 26% - 50%: Kuning Agak Gelap (Dark Yellowish / Orange-ish)
            hpFillImage.color = new Color(0.8f, 0.7f, 0f); 
        }
        else 
        {
            hpFillImage.color = Color.red; // 25% ke bawah: Merah
        }

        // Opsional: Sembunyikan HP Bar jika darahnya masih penuh (100%)
        if (hideWhenFull && _canvas != null)
        {
            _canvas.enabled = healthPercentage < 1.0f;
        }
    }
}