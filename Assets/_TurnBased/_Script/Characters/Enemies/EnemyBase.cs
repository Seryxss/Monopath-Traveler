using UnityEngine;
using System.Collections; // Untuk coroutine (jeda waktu)

public class EnemyBase : CharacterBase
{
    private void Awake() => BattleManager.OnPreStateChange += OnStateChanged;

    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;

    private void OnStateChanged(BattleState newState)
    {
        
        if (newState == BattleState.EnemyTurn)
        {
            StartCoroutine(ExecuteAITurn());
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
}