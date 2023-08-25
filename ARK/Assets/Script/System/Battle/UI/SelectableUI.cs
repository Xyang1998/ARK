using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 用于详细界面可选择查看的头像
/// </summary>
public class SelectableUI : MonoBehaviour,IPointerClickHandler
{
    private BaseCharacter character;
    private DetailUI detailUI;
    public Image icon;

    public void BindCharacter(BaseCharacter _character,DetailUI _detail)
    {
        character = _character;
        detailUI = _detail;
        icon.sprite = _character.CharacterDataStruct.icon;
    }

    public void UnBindCharacter()
    {
        character = null;
        icon.sprite = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        detailUI.ShowCharacterDetail(character);
        detailUI.HighlightSelect(gameObject);
    }
}
