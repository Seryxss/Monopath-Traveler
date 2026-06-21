using UnityEngine;
using Fungus;

public class TriggerCutscene : MonoBehaviour
{
    [Header("Event Data")]
    public GameEventFlag eventFlag;

    [Header("Fungus Settings")]
    public Flowchart flowchart;
    public string blockName;

    private void Start()
    {
        if (ProgressManager.Instance != null && eventFlag != null)
        {
            if (ProgressManager.Instance.IsEventCompleted(eventFlag))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (eventFlag != null)
            {
                if (!ProgressManager.Instance.IsEventCompleted(eventFlag))
                {
                    ProgressManager.Instance.SetEventStatus(eventFlag, EventStatus.InProgress);

                    if (flowchart != null && flowchart.HasBlock(blockName))
                    {
                        flowchart.ExecuteBlock(blockName);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}