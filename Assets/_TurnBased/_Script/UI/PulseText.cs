using UnityEngine;
using TMPro;

public class PulseText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    public float speed = 2f; // Kecepatan denyut
    public float minAlpha = 0.3f; // Transparansi paling redup
    public float maxAlpha = 1f; // Transparansi paling terang

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (textMesh != null)
        {
            Color color = textMesh.color;
            // Menghasilkan nilai yang naik turun secara halus menggunakan gelombang Sinus
            color.a = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            textMesh.color = color;
        }
    }
}