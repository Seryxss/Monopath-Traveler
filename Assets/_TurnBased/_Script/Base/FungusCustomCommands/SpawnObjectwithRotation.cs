using UnityEngine;
using Fungus;

[CommandInfo("GameObject", 
             "Spawn Object With Flip", 
             "Spawn Object/Prefab to Scene with Position and Flip SpriteRenderer option.")]
public class SpawnObjectWithFlip : Command
{
    [Tooltip("Prefab or Object to Spawn")]
    [SerializeField] private GameObject sourceObject;

    [Tooltip("if filled, Object will copy position of this object")]
    [SerializeField] private Transform spawnAtTransform;

    [Header("Manual Coordinates (If Transform Empty)")]
    [Tooltip("Posisi manual (X, Y, Z)")]
    [SerializeField] private Vector3 customPosition;

    [Header("Sprite Flip Settings")]
    [Tooltip("Flip X (Kiri/Kanan)")]
    [SerializeField] private bool flipX = false;

    [Tooltip("Flip Y (Atas/Bawah)")]
    [SerializeField] private bool flipY = false;

    [Header("Hierarchy")]
    [Tooltip("Optional: Make New object as Child of this Transform")]
    [SerializeField] private Transform parentTransform;

    public override void OnEnter()
    {
        if (sourceObject == null)
        {
            Debug.LogWarning("[Fungus] SpawnObjectWithFlip gagal: Source Object kosong!");
            Continue();
            return;
        }

        // 1. Munculkan objeknya
        GameObject newObject = Instantiate(sourceObject);

        // 2. Atur Parent
        if (parentTransform != null)
        {
            newObject.transform.SetParent(parentTransform);
        }

        // 3. Atur Posisi
        if (spawnAtTransform != null)
        {
            newObject.transform.position = spawnAtTransform.position;
        }
        else
        {
            newObject.transform.position = customPosition;
        }

        // 4. Atur Flip X / Y pada SpriteRenderer
        // GetComponentInChildren digunakan agar tetap berfungsi meskipun SpriteRenderer ada di dalam objek anaknya
        SpriteRenderer spriteRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX;
            spriteRenderer.flipY = flipY;
        }
        else
        {
            Debug.LogWarning($"[Fungus] Objek {newObject.name} tidak memiliki komponen SpriteRenderer untuk di-flip!");
        }

        // Lanjut ke command berikutnya
        Continue();
    }

    public override string GetSummary()
    {
        if (sourceObject == null) return "Error: No object selected";
        
        // Tampilkan info flip di kotak Flowchart agar mudah dibaca
        string flipText = "";
        if (flipX) flipText += " [Flip X]";
        if (flipY) flipText += " [Flip Y]";

        string targetText = spawnAtTransform != null ? spawnAtTransform.name : "Custom Pos";
        return $"{sourceObject.name} at {targetText}{flipText}";
    }

    public override Color GetButtonColor()
    {
        // Warna Pink standar mirip dengan command GameObject bawaan Fungus
        return new Color32(235, 191, 217, 255); 
    }
}