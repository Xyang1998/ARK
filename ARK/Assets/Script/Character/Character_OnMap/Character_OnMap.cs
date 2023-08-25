using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_OnMap 
{
    /// <summary>
    /// 角色状态数据（HP等等。）
    /// </summary>
    public BaseCharacterState BaseCharacterState
    {
        get;
        private set;
    }

    /// <summary>
    /// 用于保存角色描述等等
    /// </summary>
    public CharacterDataStruct CharacterDataStruct
    {
        get;
        private set;
    }

    private CharacterUI_OnMap characterUIOnMap;

    public Character_OnMap(BaseCharacterState state, CharacterDataStruct dataStruct)
    {
        BaseCharacterState = state;
        CharacterDataStruct = dataStruct;
    }

    public void BindUI(CharacterUI_OnMap ui)
    {
        characterUIOnMap = ui;
    }
    

}
