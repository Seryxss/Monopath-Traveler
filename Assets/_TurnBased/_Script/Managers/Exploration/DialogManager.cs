using UnityEngine;
using Fungus;
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
    
    yield return new WaitForSeconds(0.2f);

    
    yield return new WaitUntil(() => mainFlowchart.HasExecutingBlocks());

    
    yield return new WaitUntil(() => !mainFlowchart.HasExecutingBlocks());

    if (GameManager.Instance.State != GameState.InBattle)
    {
        GameManager.Instance.ChangeState(GameState.Exploring);
    }
}
}
