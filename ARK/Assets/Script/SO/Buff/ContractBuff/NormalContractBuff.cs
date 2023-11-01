using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NormalContractBuff",menuName = "ScriptableObject/NormalContractBuff")]
public class NormalContractBuff : NormalBuff
{
   public void Reset()
   {
      buffType = BuffType.Passive;
      isPermanent = true;
      changeBase = true;
   }
#if UNITY_EDITOR
   

   void OnValidate()
   {
      buffType = BuffType.Passive;
      isPermanent = true;
      changeBase = true;
   }
   #endif
}
