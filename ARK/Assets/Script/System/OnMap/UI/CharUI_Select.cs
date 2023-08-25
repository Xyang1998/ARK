using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharUI_Select : MonoBehaviour,IPointerClickHandler
{
    public CharacterDataStruct dataStruct;
    public Image portrait;
    public Image stars;
    public Image charClass;
    public Text CName;
    public Text recruitCost;
    public Outline outline;
    private CharSelectView charSelectView;

    public void Init(CharacterDataStruct _dataStruct,CharSelectView _charSelectView)
    {
        charSelectView = _charSelectView;
        dataStruct = _dataStruct;
        portrait.sprite =
            Resources.Load<Sprite>($"UI/UIImage/CharacterPortrait/{dataStruct.name}_{dataStruct.id}/Portrait");
        stars.sprite=Resources.Load<Sprite>($"UI/UIImage/Stars/star_{dataStruct.stars}");
        charClass.sprite=Resources.Load<Sprite>($"UI/UIImage/Class/{dataStruct.characterClass}");
        CName.text = dataStruct.CName;
        int costNum = CostDef.costDict[dataStruct.stars];
        recruitCost.text = costNum.ToString();
        if (SystemMediator.Instance.teamState.Gold < costNum)
        {
            recruitCost.color=Color.red;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SystemMediator.Instance.eventSystemOnMap.selected = dataStruct;
        charSelectView.SelectCharGO(this);

    }

    public void Select()
    {
        outline.enabled = true;
    }

    public void UnSelect()
    {
        outline.enabled = false;
    }

}
