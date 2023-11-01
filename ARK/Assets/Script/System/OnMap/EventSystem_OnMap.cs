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
        _systemMediator.uiSystemOnMap.AddContractSelectActionToQueue();
        _systemMediator.uiSystemOnMap.AddSelectCharActionToQueue();
        _systemMediator.uiSystemOnMap.AddSelectCharActionToQueue();
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
        selectable = new List<CharacterDataStruct>(characterDataStructs);
        SelectChar();
    }
    

    



}
