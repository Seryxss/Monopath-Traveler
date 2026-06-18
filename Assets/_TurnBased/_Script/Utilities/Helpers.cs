using UnityEngine;


//Static class for general helpful methos
public static class Helpers
{

    //Destroy all child objects of this transform
    //ex transform.DestroyChilderns()
    public static void DestroyChilderns(this Transform t)
    {
        foreach(Transform child in t)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
