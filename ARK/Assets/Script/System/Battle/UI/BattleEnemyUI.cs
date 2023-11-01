using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnemyUI : BaseBattleUI
{
    private Transform enemyUIPoint; //血条

    private Camera battleCamera;




    public void BindBattleCamera(Camera _camera,Transform uiPoint)
    {
        enemyUIPoint = uiPoint;
        battleCamera = _camera;
        Vector3 screenPos = battleCamera.WorldToScreenPoint(enemyUIPoint.position);
        ////Debug.log(screenPos);
        transform.position = screenPos;


    }



    

  
}
