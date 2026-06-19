using UnityEngine;

public class NPCBase : MonoBehaviour
{
    [SerializeField] private ScriptableNPC _data;
    public ScriptableNPC Data => _data;
}