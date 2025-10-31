using System.Collections;
using UnityEngine;


public class DestroyCapture : MonoBehaviour
{
    public WagEvent OnDestroyEvent = new WagEvent("On Destroy");
    
    void OnDestroy()
    {
        OnDestroyEvent.Invoke();
        OnDestroyEvent.RemoveAllListeners();
    }
}