using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractTextUI : MonoBehaviour
{
   public Text contractText;
   public Image level;
   public void Init(int _level,string text)
   {
      contractText.text = text;
      level.sprite = Resources.Load<Sprite>($"UI/UIImage/Contracts/level_{_level}");
   }
   
}
