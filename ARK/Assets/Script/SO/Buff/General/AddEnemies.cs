using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AddEnemies",menuName = "ScriptableObject/AddEnemies")]
public class AddEnemies : BaseBuff
{
    public EnemySetting[] enemySettings;
    private List<Enemy> enemies = new List<Enemy>();
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        
        Debug.Log("敌人生成");
        foreach (var enemy in enemySettings)
        {
            
            //Enemy newEnemy = new Enemy(enemy.ID, enemy.enemyName.ToString(), CharacterCamp.Enemy); 
            //enemies.Add(newEnemy);
        }
        BattleSystem.Instance.PriPriorityActionAdd(AddEnemiesTask,null);
        Debug.Log("敌人创建结束");
        
        
    }

    public async UniTask AddEnemiesTask(BaseCharacter initiator)
    {
        List<Enemy> spawnEnemies= BattleSystem.Instance.AddEnemies(enemies);
        Debug.Log("AddEnemies");
        await BattleSystem.Instance.Spawn(spawnEnemies);
    }
    
}
