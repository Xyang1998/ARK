using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayable : MonoBehaviour
{
    private BoxCollider2D hitBox;
    private SystemMediator systemMediator;

    private void Awake()
    {
        hitBox = GetComponent<BoxCollider2D>();
        hitBox.enabled = false;
        systemMediator = SystemMediator.Instance;
    }

    void AttackStart()
    {
        systemMediator.playerController.Lock();

    }

    void EnableHitBox()
    {
        hitBox.enabled = true;
    }

    void DisableHitBox()
    {
        hitBox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.CompareTag("Enemy_OnMap"))
        {
            Debug.Log(col.transform.name);
            Enemy_OnMap enemyOnMap = col.GetComponent<Enemy_OnMap>();
            systemMediator.teamState.InitEnemies(enemyOnMap.enemySetting,enemyOnMap.reserveEnemySetting);
            systemMediator.teamState.bgm = enemyOnMap.bgm;
            systemMediator.mySceneManager.WorldToBattle();
            hitBox.enabled = false;
            
        }
    }

    void AttackEnd() 
    {
        
        systemMediator.playerController.UnLock();
    }
    
}
