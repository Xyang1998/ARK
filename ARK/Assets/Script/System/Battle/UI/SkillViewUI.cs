using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillViewUI : MonoBehaviour
{
    public Image icon;
    public Text skillName;
    public Text skillDescription;
    public RectTransform textContent;
    public Text costText;
    public Text costNum;

    public void BindSkill(ref SkillDataStruct skillDataStruct)
    {
        icon.sprite = skillDataStruct.icon;
        skillName.text = skillDataStruct.text.skillName;
        skillDescription.text = skillDataStruct.text.description;
        int cost = skillDataStruct.text.cost;
        costText.text = cost < 0 ? "技能产费:" : "技能消耗:";
        costNum.text = Mathf.Abs(cost).ToString();
        textContent.sizeDelta = new Vector2(textContent.sizeDelta.x, skillDescription.rectTransform.sizeDelta.y);


    }
}
