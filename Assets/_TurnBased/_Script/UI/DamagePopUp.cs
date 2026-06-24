using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh; 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fadeDuration = 1f;

    public void Setup(string textContent, Color textColor, float xOffset = 0f, float yOffset = 0f)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        
        textMesh.text = textContent;
        textMesh.color = textColor;

        transform.position += new Vector3(xOffset, yOffset, 0);

        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        float elapsed = 0f;
        Color startColor = textMesh.color;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1.5f, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            transform.position = Vector3.Lerp(startPos, endPos, t);
            
            if (t > 0.5f)
            {
                startColor.a = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);
                textMesh.color = startColor;
            }
            yield return null;
        }
        
        Destroy(gameObject); 
    }
}