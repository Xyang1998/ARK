using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DamageTextPool 
{
    private static Stack<GameObject> texts=new Stack<GameObject>();

    public static GameObject GetGO()
    {
        GameObject go;
        if (texts.Count != 0)
        {
            go = texts.Pop();
            

        }

        else
        {
            go = Object.Instantiate(Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"DamageText"));
            go.GetComponent<Text>().material =
                Object.Instantiate(Resources.Load<Material>("UI/Material/DamageText"));

        }
       
        go.SetActive(true);
        return go;
        
    }

    public static void AddToPool(GameObject go)
    {
        go.SetActive(false);
        texts.Push(go);
    }
    
}
