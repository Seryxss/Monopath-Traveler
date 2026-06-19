using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "Game/NPC")]
public class ScriptableNPC : ScriptableObject
{
    public string npcName;
    public Sprite portrait;
    public NPCBase Prefab;

    [Header("Fungus Integration")]
    [Tooltip("Nama Block di Fungus Flowchart yang akan dipanggil (misal: DialogGuru)")]
    public string fungusBlockName = "Start"; 
}