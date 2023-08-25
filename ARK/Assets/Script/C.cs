using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;

public class C : MonoBehaviour,IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        

    }

    public  void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
    }

    
}
