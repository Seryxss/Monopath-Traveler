using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    public float offset = 0.1f;
    public LayerMask groundLayer;

    public void Snap()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f, groundLayer))
        {
            // Pindahkan posisi kaki tepat di titik hit
            transform.position = new Vector3(transform.position.x, hit.point.y + offset, transform.position.z);
            Debug.Log("Snapping");
        }
    }
}