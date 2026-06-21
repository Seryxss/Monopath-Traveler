using UnityEngine;

public class TargetHighlight : MonoBehaviour
{
    private Renderer _renderer;
    private Color _originalColor;

    void Awake()
    {
        // Mencari komponen Renderer di objek ini atau anaknya
        _renderer = GetComponentInChildren<Renderer>();
        
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (_renderer == null) return;

        if (isHighlighted)
        {
            _renderer.material.color =  _originalColor * 1.5f; // (Coba ganti jadi Color.yellow atau warna lain sesuai selera)
        }
        else
        {
            _renderer.material.color = _originalColor;
        }
    }
}