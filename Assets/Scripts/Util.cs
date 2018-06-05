using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static void SetActive(MonoBehaviour obj, bool active)
    {
        if (obj != null && obj.gameObject != null)
        {
            obj.gameObject.SetActive(active);
        }
        else
        {
            Debug.Log("null object in Util.SetActive");
        }        
    }
}