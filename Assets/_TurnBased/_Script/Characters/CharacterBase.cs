using System.Collections;
using UnityEngine;
using System;

public class CharacterBase : MonoBehaviour
{
    public Stats Stats { get; private set; }
    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnSpChanged;
    
    [Header("Current Status")]
    public int currentHp { get; protected set; } 
    public int currentSp { get; protected set; }
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject damagePopupPrefab; 

    protected Animator _animator;

    public AudioSource charAudioSource { get; protected set; }
    protected Vector3 _originalStandPosition;
    protected SnapToGround _snapToGround;
    
    protected virtual void Awake()
    {
        charAudioSource = gameObject.AddComponent<AudioSource>();
        charAudioSource.playOnAwake = false;
        charAudioSource.spatialBlend = 0f;

        _originalStandPosition = transform.position;
        _snapToGround = GetComponent<SnapToGround>();
        
        
        //_animator = GetComponent<Animator>();
    }

    public virtual void InitUnitData(ScriptableBaseCharacter data) 
    {
        Stats = data.BaseStats;
        currentHp = Stats.maxHp;
        currentSp = Stats.maxSp;

        // -> EKSEKUSI ANIMATOR OVERRIDE SYSTEM
        // if (_animator != null && data.animatorOverride != null)
        // {
        //     _animator.runtimeAnimatorController = data.animatorOverride;
        // }

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && data.DefaultSprite != null)
        {
            sr.sprite = data.DefaultSprite;
        }

        StartCoroutine(UpdateColliderSize());

        OnHealthChanged?.Invoke(currentHp, Stats.maxHp);
        OnSpChanged?.Invoke(currentSp, Stats.maxSp);
    }

    public virtual void SetStats(Stats stats)
    {
        Stats = stats;
        currentHp = stats.maxHp;
        currentSp = stats.maxSp;
        OnHealthChanged?.Invoke(currentHp, Stats.maxHp);
        OnSpChanged?.Invoke(currentSp, Stats.maxSp);
    }

    private IEnumerator UpdateColliderSize()
    {
        yield return new WaitForEndOfFrame(); 

        Collider col = GetComponent<Collider>(); 
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        if (col != null && sr != null && sr.sprite != null)
        {
            Vector2 spriteSize = sr.sprite.bounds.size;
            
            if (col is SphereCollider sphereCol) // Unity 3D menggunakan SphereCollider untuk lingkaran
            {
                sphereCol.radius = Mathf.Max(spriteSize.x, spriteSize.y) * 0.5f;
            }
        }
    }

    public void RegenerateSP(int amount)
    {
        currentSp = Mathf.Min(currentSp + amount, Stats.maxSp);
        
        OnSpChanged?.Invoke(currentSp, Stats.maxSp);
    }

    public virtual void TakeDamage(int damage, DamageEffectiveness effectiveness = DamageEffectiveness.None)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        OnHealthChanged?.Invoke(currentHp, Stats.maxHp);

        // 3. LOGIKA MEMUNCULKAN 2 POP-UP
        if (damagePopupPrefab != null)
        {
            Vector3 basePos = transform.position + new Vector3(0, 1.5f, 0);

            // -> POP-UP PERTAMA: Angka Damage (Warna Putih, digeser sedikit ke kiri)
            GameObject damageObj = Instantiate(damagePopupPrefab, basePos, Quaternion.identity);
            DamagePopup damagePopup = damageObj.GetComponent<DamagePopup>();
            if (damagePopup != null) 
            {
                // Format: Teks, Warna, X offset, Y offset
                damagePopup.Setup(damage.ToString(), Color.white, -0.4f, 0f); 
            }

            // -> POP-UP KEDUA: Teks Status Kelemahan (Hanya muncul jika bukan "None")
            if (effectiveness != DamageEffectiveness.None)
            {
                GameObject effectObj = Instantiate(damagePopupPrefab, basePos, Quaternion.identity);
                DamagePopup effectPopup = effectObj.GetComponent<DamagePopup>();
                
                if (effectPopup != null)
                {
                    if (effectiveness == DamageEffectiveness.Weak)
                    {
                        // Jika musuh lemah: Teks "WEAK!", Merah, digeser ke kanan & sedikit lebih tinggi
                        effectPopup.Setup("WEAK!", Color.red, 0.5f, 0.4f); 
                    }
                    else if (effectiveness == DamageEffectiveness.Strong)
                    {
                        // Jika musuh kebal: Teks "RESIST", Abu-abu, digeser ke kanan & sedikit lebih tinggi
                        effectPopup.Setup("RESIST", Color.gray, 0.5f, 0.4f);
                    }
                }
            }
        }
    }

    public virtual void ConsumeSP(int cost)
    {
        currentSp -= cost;
        if (currentSp < 0) currentSp = 0;

        Debug.Log($"[BATTLE] {gameObject.name} memakai {cost} SP! Sisa SP: {currentSp}/{Stats.maxSp}");
        
        // Panggil event agar UI SP Bar bergerak turun!
        OnSpChanged?.Invoke(currentSp, Stats.maxSp);
    }

    public virtual void EvaluateDeathStatus()
    {
        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    protected IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration); 
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            if (_snapToGround != null) _snapToGround.Snap();

            yield return null;
        }

        transform.position = targetPos;
        if (_snapToGround != null) _snapToGround.Snap();
    }
}