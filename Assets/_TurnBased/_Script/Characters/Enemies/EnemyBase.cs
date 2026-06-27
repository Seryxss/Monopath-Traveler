using UnityEngine;
using System.Collections; 
using System.Collections.Generic;   
using System; // Tambahkan ini untuk event Action

public class EnemyBase : CharacterBase
{
    // UBAH: Sekarang menggunakan ScriptableElement agar UI bisa baca gambar ikonnya
    private List<ScriptableElement> _weaknesses = new List<ScriptableElement>();
    private List<ScriptableElement> _resistances = new List<ScriptableElement>();

    // FITUR BARU: Menyimpan elemen apa saja yang sudah berhasil ditebak pemain
    public List<SkillElement> DiscoveredWeaknesses { get; private set; } = new List<SkillElement>();
    
    // Event agar UI Health Bar tahu kapan harus me-refresh ikon "?" menjadi ikon asli
    public event Action OnWeaknessDiscovered; 

    public List<ScriptableElement> Weaknesses => _weaknesses;
    public List<ScriptableElement> Resistances => _resistances;

    protected override void Awake() 
    {
        base.Awake(); 
        BattleManager.OnPreStateChange += OnStateChanged;
    }

    // UBAH: Parameter sekarang menerima ScriptableElement
    public void SetElementalAffinities(List<ScriptableElement> weakData, List<ScriptableElement> resistData)
    {
        if (weakData != null) _weaknesses = new List<ScriptableElement>(weakData);
        if (resistData != null) _resistances = new List<ScriptableElement>(resistData);
    }

    // FITUR BARU: Cek apakah musuh lemah terhadap elemen (Enum) tertentu
    public bool IsWeakTo(SkillElement element)
    {
        foreach (var weak in _weaknesses)
        {
            if (weak != null && weak.element == element) return true;
        }
        return false;
    }

    // FITUR BARU: Cek apakah musuh kebal terhadap elemen tertentu
    public bool IsResistantTo(SkillElement element)
    {
        foreach (var resist in _resistances)
        {
            if (resist != null && resist.element == element) return true;
        }
        return false;
    }

    // FITUR BARU: Panggil ini jika hero berhasil menyerang dengan elemen yang tepat
    public void RevealWeakness(SkillElement element)
    {
        if (!DiscoveredWeaknesses.Contains(element))
        {
            DiscoveredWeaknesses.Add(element);
            OnWeaknessDiscovered?.Invoke(); // Suruh UI berubah dari "?" ke ikon elemen
        }
    }

    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;

    private void OnStateChanged(BattleState newState)
    {
        if (!gameObject.activeInHierarchy || currentHp <= 0) return;
    }

    public void ExecuteTurn(System.Action onComplete)
    {
        StartCoroutine(ExecuteAITurnCoroutine(onComplete));
    }

    public override void EvaluateDeathStatus()
    {
        base.EvaluateDeathStatus(); 

        if (currentHp <= 0)
        {
            gameObject.SetActive(false); 
            
            if (CharacterManager.Instance.ActiveEnemies.Contains(this))
            {
                CharacterManager.Instance.ActiveEnemies.Remove(this);
            }
            BattleManager.Instance.CheckBattleEnd();
        }
    }

    private IEnumerator ExecuteAITurnCoroutine(System.Action onComplete)
    {
        // ... (Logika AI musuhmu biarkan persis seperti sebelumnya) ...
        Debug.Log($"{gameObject.name} sedang memikirkan target...");
        
        List<HeroCharBase> activeHeroes = BattleManager.Instance.GetActiveHeroes();
        if (activeHeroes == null || activeHeroes.Count == 0) 
        {
            onComplete?.Invoke();
            yield break;
        }

        HeroCharBase targetHero = activeHeroes[UnityEngine.Random.Range(0, activeHeroes.Count)];
        Vector3 centerStagePos = BattleManager.Instance.ActionCenterPosition;

        yield return StartCoroutine(MoveToPosition(centerStagePos, 0.2f));
        yield return new WaitForSeconds(0.3f); 

        int damage = Stats.Attack;
        targetHero.TakeDamage(damage);
        Debug.Log($"[ENEMY ATTACK] {gameObject.name} menyerang {targetHero.gameObject.name} dengan {damage} damage!");

        targetHero.EvaluateDeathStatus();

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveToPosition(_originalStandPosition, 0.2f));

        Debug.Log($"Giliran {gameObject.name} selesai.");
        
        onComplete?.Invoke();
    }
}