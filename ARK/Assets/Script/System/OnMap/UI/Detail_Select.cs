using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Detail_Select : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text CName;
    public Text hp;
    public Text np;
    public Text atk;
    public Text speed;
    public Text defense;
    public Text magicDefense;
    public Text critRate;
    public Text criticalDamage;
    public Text npRate;
    public Text healRate;
    public Text effectHitRate;
    public Text effectResistanceRate;
    public Image classIcon;
    public RectTransform skillContent;
    
    public void ShowCharacterDetail(CharacterDataStruct dataStruct)
    {
        BaseCharacterState baseCharacterState=Instantiate(Resources.Load<BaseCharacterState>($"Character/CharacterData/{dataStruct.name}_{dataStruct.id}"));
        CName.text = dataStruct.CName;
        classIcon.sprite = ClassIcons.GetClassIcon(dataStruct.characterClass);
        CharacterStateData data = baseCharacterState.CharacterStateData;
        hp.text = $"{(int)data.MaxHP}";
        np.text = $"{(int)data.MaxNP}";
        atk.text = data.ATK.ToString();
        speed.text = data.Speed.ToString();
        defense.text = data.Defense.ToString();
        magicDefense.text = data.MagicDefense.ToString();
        critRate.text = data.CritRate.ToString();
        criticalDamage.text = data.CriticalDamage.ToString();
        npRate.text = data.NPRate.ToString();
        healRate.text = data.Healing.ToString();
        effectHitRate.text = data.EffectHitRate.ToString();
        effectResistanceRate.text = data.EffectResistanceRate.ToString();
        
        //技能部分
        ClearSkills();
        float y=0;
        float space = skillContent.GetComponent<VerticalLayoutGroup>().spacing;
        for (int i = 0; i < dataStruct.skillIDs.Length; i++)
        {
            SkillViewUI skillViewUI =
                Instantiate(Resources.Load<SkillViewUI>(FilePath.onMapUIPrefabPath + "SkillDesc"),skillContent);
            y = skillViewUI.GetComponent<RectTransform>().sizeDelta.y;
            SkillText text = TextSystem.skillExcelLoader.GetTextStruct(dataStruct.skillIDs[i]);
            SkillDataStruct skillDataStruct = new SkillDataStruct(ref text);
            skillViewUI.BindSkill(ref skillDataStruct);
        }
        skillContent.sizeDelta = new Vector2(skillContent.sizeDelta.x, (y + space) * dataStruct.skillIDs.Length);
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        ClearSkills();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void ClearSkills()
    {
        int num = skillContent.childCount;
        for (int i = 0; i < num; i++)
        {
            DestroyImmediate(skillContent.GetChild(0).gameObject);
        }
    }
    

}
