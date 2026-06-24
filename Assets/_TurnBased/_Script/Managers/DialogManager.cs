using UnityEngine;
using Fungus; // Wajib untuk mengakses Flowchart Fungus
using System.Collections;

public class DialogManager : Singleton<DialogManager>
{
    [SerializeField] private Flowchart mainFlowchart;

    public void PlayDialog(string blockName)
    {
        if (mainFlowchart == null)
        {

            return;
        }

        if (!mainFlowchart.HasExecutingBlocks())
        {
            GameManager.Instance.ChangeState(GameState.InDialog);
            mainFlowchart.ExecuteBlock(blockName);
            StartCoroutine(WatchDialog());
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
