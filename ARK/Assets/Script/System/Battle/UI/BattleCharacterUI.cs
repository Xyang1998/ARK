using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class BattleCharacterUI : BaseBattleUI
{
    public Image portraitImage; //人物头像
    public Text HPText;
    public Text NPText;
    public Button Ultimate;
    
    //private float npTargetPer;
    //private float npCurPer;
    //private bool updatingNP=false;
    /// <summary>
    /// target>Cur时,flag=ture,否则为false
    /// </summary>
    //private bool npFlag = false;

    /// <summary>
    /// 大招准备时颜色
    /// </summary>
    public Color unReadyColor;
    /// <summary>
    /// 大招未准备时颜色
    /// </summary>
    public Color readyColor;

    public override void Init(CharacterStateData data, CharacterDataStruct dataStruct)
    {
        base.Init(data, dataStruct);
        
        if (data.NP >= data.MaxNP - 0.1)
        {
            UltimateReady();
        }
        else
        {
            UltimateBlock();
        }
        if (Name)
        {
            Name.text = dataStruct.CName;
        }
        if (HPText)
        {
            HPText.text = ((int)data.HP).ToString();
        }
        
        if (NPText)
        {
            NPText.text = ((int)data.NP).ToString();
        }
        
        portraitImage.material = Instantiate(Resources.Load<Material>("UI/Material/Portrait"));
        Sprite portrait =
            Resources.Load<Sprite>($"UI/UIImage/CharacterPortrait/{dataStruct.name}_{data.ID}/Portrait");
        Sprite mask =
            Resources.Load<Sprite>($"UI/UIImage/CharacterPortrait/{dataStruct.name}_{data.ID}/Mask");
        if (portrait)
        {
            portraitImage.material.SetTexture("_MainTex", portrait.texture);
        }

        if (mask)
        {
            portraitImage.material.SetTexture("_MaskTex", mask.texture);
        }


    }

    public void BindUButton(BaseCharacter character)
    {
        Character c=character as Character;
        if (c!=null)
        {
            Ultimate.image.sprite = c.ultimate.skillDataStruct.icon;
            Ultimate.onClick.AddListener(c.UltimateClick);
        }

    }
    

    public override void UltimateReady()
    {
        Ultimate.enabled = true;
        Ultimate.image.color=readyColor;
    }

    public void UltimateBlock()
    {
        Ultimate.enabled = false;
        Ultimate.image.color=unReadyColor;
    }

    public override void UpdateHP()
    {
        if (HPText)
        {
            HPText.text = ((int)characterStateData.HP).ToString();
        }
        base.UpdateHP();
    }

    public override void UpdateNP()
    {
        if (NPText)
        {
            NPText.text = ((int)characterStateData.NP).ToString();
        }
        base.UpdateNP();
    }
}
