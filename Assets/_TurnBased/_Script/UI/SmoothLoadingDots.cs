using UnityEngine;
using System.Collections;

public class SmoothLoadingDots : MonoBehaviour
{
    [Tooltip("Masukkan 3 GameObject Titik (TextMeshPro) ke sini")]
    [SerializeField] private RectTransform[] dots; // <-- PERHATIKAN: Sekarang pakai RectTransform
    
    [Tooltip("Tinggi lompatan ke atas")]
    [SerializeField] private float jumpHeight = 15f; 
    
    [Tooltip("Seberapa cepat titik melompat")]
    [SerializeField] private float jumpSpeed = 5f; 
    
    [Tooltip("Jeda waktu antar titik (efek gelombang)")]
    [SerializeField] private float delayBetweenDots = 0.2f;

    private float[] originalY;
    private Coroutine bounceCoroutine;

    private void OnEnable()
    {
        if (dots == null || dots.Length == 0) return;

        // Simpan posisi Y awal
        if (originalY == null || originalY.Length != dots.Length)
        {
            originalY = new float[dots.Length];
            for (int i = 0; i < dots.Length; i++)
            {
                if (dots[i] != null) originalY[i] = dots[i].anchoredPosition.y;
            }
        }

        bounceCoroutine = StartCoroutine(BounceRoutine());
    }

    private void OnDisable()
    {
        if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
        
        // Kembalikan ke posisi semula saat UI dimatikan
        for (int i = 0; i < dots.Length; i++)
        {
            if (dots[i] != null && originalY != null)
            {
                Vector2 pos = dots[i].anchoredPosition;
                pos.y = originalY[i];
                dots[i].anchoredPosition = pos;
            }
        }
    }

    private IEnumerator BounceRoutine()
    {
        float time = 0f;
        while (true)
        {
            // Waktu berjalan dikalikan kecepatan
            time += Time.deltaTime * jumpSpeed;
            
            for (int i = 0; i < dots.Length; i++)
            {
                if (dots[i] == null) continue;


                float phaseOffset = i * delayBetweenDots; 
                
                // Menghitung pantulan murni
                float bounce = Mathf.Max(0f, Mathf.Sin(time - phaseOffset));
                
                Vector2 pos = dots[i].anchoredPosition;
                pos.y = originalY[i] + (bounce * jumpHeight);
                dots[i].anchoredPosition = pos;
            }
            yield return null;
        }
    }
}