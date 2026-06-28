using UnityEngine;

public class NPCBase : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private ScriptableNPC _data;
    public ScriptableNPC Data => _data;

    public void Interact()
    {
        if (Data != null && !string.IsNullOrEmpty(Data.fungusBlockName))
        {
            Debug.Log($"[NPC] opening Block: {Data.fungusBlockName}");
            
            DialogManager.Instance.PlayDialog(Data.fungusBlockName);
        }
        else
        {
            Debug.LogWarning("[NPC] Data NPC/Block Fungus Empty");
        }
    }

    public void ShowPrompt()
    {
        if (promptUI != null) promptUI.ShowPrompt();
    }

    public void HidePrompt()
    {
        if (promptUI != null) promptUI.HidePrompt();
    }
}