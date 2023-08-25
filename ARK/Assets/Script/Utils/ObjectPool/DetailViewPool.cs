using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectablePool
{
    private static Stack<GameObject> stack = new Stack<GameObject>();

    public static GameObject GetSelectable()
    {
        
        GameObject go;
        if (stack.Count == 0)
        {
                go = Object.Instantiate(Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"Selectable"));
        }
        else
        {
            go = stack.Pop();
        }
        return go;
        
    }

    public static void AddToPool(GameObject go)
    {
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, 9999);
        go.transform.SetParent(null);
        stack.Push(go);
    }
    
    
}

public static class SkillViewPool
{
    private static Stack<GameObject> stack = new Stack<GameObject>();

    public static GameObject GetSkillView()
    {
        GameObject go;
        if (stack.Count == 0)
        {
            go = Object.Instantiate(Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"SkillDesc"));
        }
        else
        {
            go = stack.Pop();
        }
        return go;
        
    }

    public static void AddToPool(GameObject go)
    {
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, 9999);
        go.transform.SetParent(null);
        stack.Push(go);
    }
}
public static class BuffViewPool
{
    private static Stack<GameObject> stack = new Stack<GameObject>();

    public static GameObject GetBuffView()
    {
        GameObject go;
        if (stack.Count == 0)
        {
            go = Object.Instantiate(Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"BuffDesc"));
                
                
        }
        else
        {
            go = stack.Pop();
        }
        return go;
        
    }

    public static void AddToPool(GameObject go)
    {
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, 9999);
        go.transform.SetParent(null);
        stack.Push(go);
    }
}
