using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClassIcons
{
    private static Dictionary<CharacterClass, Sprite> classImageDict = new Dictionary<CharacterClass, Sprite>();

    public static Sprite GetClassIcon(CharacterClass characterClass)
    {
        if (classImageDict.ContainsKey(characterClass))
        {
            return classImageDict[characterClass];
        }
        else
        {
            Sprite sprite = Resources.Load<Sprite>($"UI/UIImage/Class/{characterClass}");
            classImageDict.Add(characterClass,sprite);
            return sprite;
        }
    }
}
