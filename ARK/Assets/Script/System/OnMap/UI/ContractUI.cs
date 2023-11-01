using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContractUI : MonoBehaviour,IPointerClickHandler
{
    private ContractSelectView manager;
    public Image icon;
    public Image upImage;
    private bool selected=false;
    public Color upColor;
    public Color backColor;
    public Image backImage;
    public ContractTextUI textUI;

    public ContractStruct ContractStruct
    {
        get;
        private set;
    }

    public void Bind(ContractSelectView _manager,ContractStruct contractStruct)
    {
        ContractStruct = contractStruct;
        manager = _manager;
        icon.sprite = contractStruct.icon;
        backImage.color = backColor;
        upImage.color = upColor;
        upImage.enabled = false;
        backImage.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selected)
        {
            manager.SelectOne(this);
        }
        else
        {
            manager.UnSelectOne(this);
        }
        

        
    }

    public void SelectedAction()
    {
        
        backImage.enabled = true;
        selected = true;
        upImage.enabled = false;
        manager.AddOne(this).Forget();
    }

    public void UnSelectedAction()
    {
        manager.DeleteOne(this);
        selected = false;
        backImage.enabled = false;
        upImage.enabled = false;
    }

    public void Conflict()
    {
        manager.DeleteOne(this);
        selected = false;
        upImage.enabled = true;
        backImage.enabled = false;
        
    }
}
