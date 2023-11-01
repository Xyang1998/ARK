using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCharacter : BaseCharacter
{
    public SystemCharacter():base()
    {
        CharacterDataStruct dataStruct;
        dataStruct.id = -1;
        dataStruct.description = "";
        dataStruct.icon = null;
        dataStruct.name = "system";
        dataStruct.characterCamp = CharacterCamp.System;
        dataStruct.characterClass = CharacterClass.Special;
        dataStruct.stars = 1;
        dataStruct.CName = "system";
        dataStruct.skillIDs = Array.Empty<int>();
        CharacterDataStruct = dataStruct;
    }
    
}
