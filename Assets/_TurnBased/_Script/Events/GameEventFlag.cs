using UnityEngine;

// 2. Buat blueprint untuk item event-nya
[CreateAssetMenu(fileName = "NewEventFlag", menuName = "Game/Event Flag")]
public class GameEventFlag : ScriptableObject
{
    [Tooltip("Short Description of event (opsional)")]
    [TextArea]
    public string description;
    
}