using UnityEngine;
using Fungus;

public class TriggerCutscene : MonoBehaviour
{
    [Header("Event Data")]
    [SerializeField] private GameEventFlag eventFlag;

    [Header("Fungus Settings")]
    [SerializeField] private Flowchart flowchart;
    [SerializeField] private string blockName;

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
                    if (eventFlag != null) 
                    {
                        ProgressManager.Instance.StartEvent(eventFlag); 
                    }

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
