using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillDataStruct
{
    public SkillText text;
    public Sprite icon;

    public SkillDataStruct(ref SkillText skillText)
    {
        text = skillText;
        icon = Resources.Load<Sprite>($"UI/UIImage/Skills/{text.iconName}");
        if (icon == null)
        {
            icon = Resources.Load<Sprite>($"UI/UIImage/Skills/Default");
        }
    }
}
