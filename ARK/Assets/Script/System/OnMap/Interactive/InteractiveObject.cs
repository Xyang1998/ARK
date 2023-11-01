using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public virtual void Interact()
    {
        
    }
    
    /// <summary>
    /// 靠近物品时
    /// </summary>
    public virtual void TriggerEnter()
    {
        
    }
    /// <summary>
    /// 远离物品时
    /// </summary>
    public virtual void TriggerExit()
    {
        
    }
}
