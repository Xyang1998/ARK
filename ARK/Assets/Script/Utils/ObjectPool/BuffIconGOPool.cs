using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class BuffIconGOPool
{
    private static Stack<GameObject> _sprites=new Stack<GameObject>();

    public static GameObject GetSpriteGO(Sprite sprite)
    {
        if (sprite == null) return null;
        GameObject go;
        if (_sprites.Count == 0)
        {
            go = Object.Instantiate(Resources.Load<GameObject>( FilePath.battleUIPrefabPath+ "BuffIcon"));

        }
        else
        {
            go = _sprites.Pop();
           
        }
        go.GetComponent<Image>().sprite = sprite;
        go.SetActive(true);
        
        return go;
    }

    public static void AddToPool(GameObject go)
    {
        go.transform.parent = null;
        go.SetActive(false);
        _sprites.Push(go);
    }
    
}

public static class TargetGOPool
{
    private static Stack<GameObject> GOPool=new Stack<GameObject>();
    private static GameObject mainTarget;

    public static GameObject GetMainTargetGO()
    {
        if (!mainTarget)
        {
            mainTarget = Object.Instantiate(Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"MainTarget"));
            
        }
        mainTarget.SetActive(true);
        return mainTarget;
    }
    

    public static GameObject GetSecTargetGO()
    {
        GameObject go;
        if (GOPool.Count == 0)
        {
            go = Object.Instantiate(Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"SecTarget"));

        }
        else
        {
            go = GOPool.Pop();
           
        }
        go.SetActive(true);
        return go;
    }

    public static void AddToPool(GameObject go)
    {
        if (go)
        {
            go.transform.parent = null;
            go.SetActive(false);
            if (go.transform.childCount == 0)
            {
                GOPool.Push(go);
            }
        }
    }
    
}

