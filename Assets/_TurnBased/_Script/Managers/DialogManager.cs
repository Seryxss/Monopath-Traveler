using UnityEngine;
using Fungus; // Wajib untuk mengakses Flowchart Fungus
using System.Collections;

public class DialogManager : Singleton<DialogManager>
{
    public Flowchart mainFlowchart;

    public void PlayDialog(string blockName)
    {
        if (mainFlowchart == null)
        {
            Debug.LogError("DialogManager: Flowchart Fungus Not in Inspector!");
            return;
        }

        if (!mainFlowchart.HasExecutingBlocks())
        {
            Debug.Log($"DialogManager: Start Dialog '{blockName}'. State to InDialog.");

            GameManager.Instance.ChangeState(GameState.InDialog);
            mainFlowchart.ExecuteBlock(blockName);
            StartCoroutine(WatchDialog());
        }
        else
        {
            Debug.LogWarning("DialogManager: Fungus Executing Blocks");
        }
    }

    private IEnumerator WatchDialog()
    {
        yield return null; //Wait 1 frame

        while (mainFlowchart.HasExecutingBlocks())
        {
            yield return null;
        }

        GameManager.Instance.ChangeState(GameState.Exploring);
    }
}