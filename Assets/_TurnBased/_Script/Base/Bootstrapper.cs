using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        // Search Prefab named "[SYSTEM_MANAGERS]" in Resources Folder
        GameObject managerPrefab = Resources.Load<GameObject>("[SYSTEM_MANAGERS]");

        if (managerPrefab != null)
        {
            //Spawn
            GameObject managerClone = Object.Instantiate(managerPrefab);
            
            managerClone.name = "[SYSTEM_MANAGERS]";
            
            Object.DontDestroyOnLoad(managerClone);
            
        }
    }
}