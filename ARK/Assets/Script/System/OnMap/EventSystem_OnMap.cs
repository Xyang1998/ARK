using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventSystem_OnMap : ISystem
{
    public List<CharacterDataStruct> selectable;
    public CharacterDataStruct selected;
    public override void Init()
    {
        selectable = new List<CharacterDataStruct>();
    }

    public override void Tick()
    {
        
    }



    /// <summary>
    /// 从预选角色中选择一个角色加入队伍
    /// </summary>
    /// <param name="structs"></param>
    public void SelectChar()
    {
        _systemMediator.uiSystemOnMap.AddSelectCharActionToQueue();
        _systemMediator.uiSystemOnMap.AddSelectCharActionToQueue();
    }

    public async UniTaskVoid NewGame()
    {
        List<CharacterDataStruct> characterDataStructs = new List<CharacterDataStruct>();
        foreach (var key in TextSystem.characterExcelLoader.GetKeys())
        {
            CharacterText text = TextSystem.characterExcelLoader.GetTextStruct(key);
            CharacterDataStruct dataStruct = TextSystem.ParseCharacterText(text);
            if (dataStruct.characterCamp == CharacterCamp.Player)
            {
                characterDataStructs.Add(dataStruct);
            }
        }
        await UniTask.Delay(1000);
        AddCharacterToList(characterDataStructs);
        SelectChar();
    }
    

    
    /// <summary>
    /// 添加一个角色到预选列表
    /// </summary>
    /// <param name="id"></param>
    public void  AddCharacterToList(List<CharacterDataStruct> datas)
    {
        foreach (var dataStruct in datas)
        {
            selectable.Add(dataStruct);
        }
    }
    


}
