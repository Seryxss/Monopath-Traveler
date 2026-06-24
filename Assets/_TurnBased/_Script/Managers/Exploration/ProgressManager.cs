using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : PersistentSingleton<ProgressManager>
{
    private Dictionary<GameEventFlag, EventStatus> eventProgress = new Dictionary<GameEventFlag, EventStatus>();
    public GameEventFlag CurrentActiveEvent { get; private set; }

    public void StartEvent(GameEventFlag flag)
    {
        if (flag != null)
        {
            CurrentActiveEvent = flag; // Jadikan ini event yang sedang aktif
            SetEventStatus(flag, EventStatus.InProgress);
            Debug.Log($"[ProgressManager] Memulai event & mencatat aktif: {flag.name}");
        }
    }
    public void CompleteActiveEvent()
    {
        if (CurrentActiveEvent != null)
        {
            MarkCompleted(CurrentActiveEvent); // Ubah statusnya jadi Completed
            CurrentActiveEvent = null; // Kosongkan penanda karena sudah selesai
        }
    }
    // Read Status
    public EventStatus GetEventStatus(GameEventFlag flag)
    {
        // If Not in Dictionary, then not started
        if (!eventProgress.ContainsKey(flag))
        {
            return EventStatus.NotStarted;
        }
        return eventProgress[flag];
    }

    // Change Status
    public void SetEventStatus(GameEventFlag flag, EventStatus newStatus)
    {
        if (eventProgress.ContainsKey(flag))
        {
            eventProgress[flag] = newStatus;
        }
        else
        {
            eventProgress.Add(flag, newStatus);
        }
        
        Debug.Log($"[ProgressManager] Event {flag.name} diubah menjadi {newStatus}");
    }

    // Check If finish or not
    public bool IsEventCompleted(GameEventFlag flag)
    {
        return GetEventStatus(flag) == EventStatus.Completed;
    }
    
    public void MarkCompleted(GameEventFlag flag)
    {
        if (flag != null)
        {
            SetEventStatus(flag, EventStatus.Completed);
            Debug.Log($"[ProgressManager] Event '{flag.name}' resmi diselesaikan.");
        }
    }
    public void ResetAllProgress()
    {
        eventProgress.Clear();
        CurrentActiveEvent = null; 
        
        Debug.Log("[ProgressManager] Semua memori progress telah di-reset ke 0!");
    }
}