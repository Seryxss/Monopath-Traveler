using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class EnemyBase : CharacterBase
{
    [Header("Elemental Affinities")]
    [SerializeField] private List<DamageType> _weaknesses;
    [SerializeField] private List<DamageType> _resistances;

    public List<DamageType> Weaknesses => _weaknesses;
    public List<DamageType> Resistances => _resistances;
    private void Awake(){
        base.Awake();
        BattleManager.OnPreStateChange += OnStateChanged;
    }
    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;

    private void OnStateChanged(BattleState newState)
    {
        
        if (newState == BattleState.EnemyTurn)
        {
            StartCoroutine(ExecuteAITurn());
        }
    }

    public void EvaluateDeathStatus()
    {
        if (this.currentHp <= 0) 
        {
            Die();
        }
    }

    private IEnumerator ExecuteAITurn()
    {
        // 1. Beri jeda sedikit agar tidak terlalu cepat (seolah-olah musuh sedang berpikir)
        Debug.Log($"{gameObject.name} sedang bersiap menyerang...");
        yield return new WaitForSeconds(1.5f); 

        // 2. Pilih Hero secara acak untuk diserang
        // (Nantinya kita ambil list hero dari BattleCharacterManager)
        Debug.Log($"{gameObject.name} menyerang Hero dengan kekuatan {Stats.Attack}!");

        // 3. Mainkan animasi attack (jika ada)
        // anim.SetTrigger("Attack");
        
        yield return new WaitForSeconds(1f); // Tunggu animasi selesai

        // 4. Setelah selesai menyerang, kembalikan giliran ke Hero
        // (CATATAN: Di game utuhnya, BattleManager yang mengatur ini setelah SEMUA musuh selesai jalan)
        Debug.Log($"Giliran {gameObject.name} selesai.");
        
        // Untuk MVP, kita langsung lempar balik gilirannya
        BattleManager.Instance.ChangeState(BattleState.HeroTurn); 
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} MATI!");
            
        // --- TAMBAHKAN BARIS INI WAJIB ---
        // Hapus musuh ini dari daftar agar tidak di-target lagi oleh hero berikutnya
        CharacterManager.Instance.ActiveEnemies.Remove(this);
        // ---------------------------------

        // Sembunyikan atau hancurkan objeknya
        Destroy(gameObject); // Atau Destroy(gameObject);
    }
}