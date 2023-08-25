using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUitls 
{
    /// <summary>
    /// 欧拉角度
    /// </summary>
    private static float cameraRotation;
    private static bool isFirst=true;

    public static void LookAtCamera(GameObject go)
    {
        if (isFirst)
        {
            cameraRotation=BattleUISystem.Instance.battleCamera.transform.rotation.eulerAngles.x;
            isFirst = false;
        }
        Vector3 temp = go.transform.rotation.eulerAngles;
        if (Math.Abs(temp.y - 180) < 0.1)
        {
            go.transform.rotation = Quaternion.Euler(-cameraRotation, temp.y, temp.z);
        }
        else
        {
            go.transform.rotation = Quaternion.Euler(cameraRotation, temp.y, temp.z);
        }
    }
}
