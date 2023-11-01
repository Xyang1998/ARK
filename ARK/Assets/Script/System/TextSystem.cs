using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSystem : ISystem
{
    public static ExcelLoader<SkillText> skillExcelLoader;
    public static ExcelLoader<BuffText> buffExcelLoader;
    public static ExcelLoader<CharacterText> characterExcelLoader;
    public static EnemySettingTextLoader enemySettingTextLoader;
    public static ExcelLoader<ContractText> contractExcelLoader;

    public override void Init()
    {
        skillExcelLoader = new ExcelLoader<SkillText>();
        buffExcelLoader = new ExcelLoader<BuffText>();
        characterExcelLoader = new ExcelLoader<CharacterText>();
        enemySettingTextLoader = new EnemySettingTextLoader();
        contractExcelLoader = new ExcelLoader<ContractText>();
        skillExcelLoader.LoadFromPath("/Resources/Text/SkillDesc/SkillDescription.xls");
        characterExcelLoader.LoadFromPath("/Resources/Text/CharDesc/CharacterDescription.xls");
        buffExcelLoader.LoadFromPath("/Resources/Text/BuffDesc/BuffDescription.xls");
        enemySettingTextLoader.LoadFromPath("/Resources/Text/EnemySetting/EnemiesSetting.xls");
        contractExcelLoader.LoadFromPath("/Resources/Text/Contracts/Contracts.xls");

    }

    public override void Tick()
    {
      
    }

    public void LoadSkillExcel()
    {
        
    }
    
    public static CharacterDataStruct ParseCharacterText(CharacterText text)
    {
        CharacterDataStruct characterDataStruct;
        characterDataStruct.id = text.id;
        characterDataStruct.name = text.name;
        characterDataStruct.CName = text.CName;
        characterDataStruct.characterCamp = Enum.Parse<CharacterCamp>(text.camp);
        characterDataStruct.stars = int.Parse(text.stars);
        characterDataStruct.characterClass = Enum.Parse<CharacterClass>(text.characterClass);
        characterDataStruct.description = text.description;
        string[] strings=text.skillIDs.Split("#");
        int[] ids = new int[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            ids[i] = int.Parse(strings[i]);
        }
        characterDataStruct.skillIDs = ids;
        characterDataStruct.icon = Resources.Load<Sprite>($"UI/UIImage/CharacterIcon/{characterDataStruct.name}_{characterDataStruct.id}");
        return characterDataStruct;
    }
    
    public static ContractStruct ParseContractText(ContractText text)
    {
        ContractStruct contractStruct;
        contractStruct.id = text.id;
        contractStruct.iconName = text.iconName;
        contractStruct.name = text.name;
        contractStruct.group = int.Parse(text.group);
        contractStruct.description = text.description;
        contractStruct.level = int.Parse(text.level);
        if (text.conflictIDs.ToLower().Equals("null"))
        {
            contractStruct.conflictIDs = Array.Empty<int>();
        }
        else
        {
            string[] strings = text.conflictIDs.Split("#");
            int[] ids = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                ids[i] = int.Parse(strings[i]);
            }

            contractStruct.conflictIDs = ids;
        }
        contractStruct.icon = Resources.Load<Sprite>($"UI/UIImage/Contracts/{contractStruct.iconName}");
        return contractStruct;
    }

}
