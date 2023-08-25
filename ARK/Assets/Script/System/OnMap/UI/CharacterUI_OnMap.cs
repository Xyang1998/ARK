using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI_OnMap : BaseUI
{
    public Image icon;
    public Image characterClass;
    public override void Init(CharacterStateData data, CharacterDataStruct dataStruct)
    {
        base.Init(data, dataStruct);
        icon.sprite = dataStruct.icon;
        characterClass.sprite = ClassIcons.GetClassIcon(dataStruct.characterClass);
    }
}
