using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CDUI : MonoBehaviour,IPointerClickHandler
{
    private int index;
    public Text CDName;


    public void Init(int index_,string name)
    {
        index = index_;
        CDName.text = name.Replace(".mp3","");

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SystemMediator.Instance.uiSystemOnMap.DjManager.PlayDJ(index).Forget();
    }
    
}
