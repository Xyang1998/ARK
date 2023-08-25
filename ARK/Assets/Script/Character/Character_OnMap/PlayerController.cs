using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : ISystem
{ 
    private MyPlayable player; //当前控制的角色
    public GameObject playerGo; //带有刚体的角色父物体
    private Rigidbody2D playerRigidBody2D; //当前控制的角色的刚体
    private Animator playerAnimator;
    private float inputX;
    public float moveSpeed = 5;
    private int isMoveID=Animator.StringToHash("IsMove");
    private int isAttackID=Animator.StringToHash("IsAttack");
    private bool moveAndAttackLock = false; //锁定移动和攻击
    private PlayableDirector director;

    public MyPlayable Player
    {
        set
        {
            MyPlayable p = value.gameObject.GetComponent<MyPlayable>();
            if (p != null)
            {
                player = p;
                playerRigidBody2D = p.GetComponentInParent<Rigidbody2D>();
                playerAnimator = p.GetComponent<Animator>();
                director = p.GetComponent<PlayableDirector>();

            }
        }
        get
        {
            return player;
        }
    }
    private TeamState playerState;
    
    public virtual void Turn(float direction)
    {
        if (direction > 0)
        {
            player.transform.rotation=Quaternion.Euler(0,0,0);
        }
        else if(direction<0)
        {
            player.transform.rotation=Quaternion.Euler(0,180,0);
        }
    }

    private void Update()
    {
        if (!moveAndAttackLock)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Test();
        }
    }

    private void FixedUpdate()
    {
        if (!moveAndAttackLock)
        {
            Move();
        }
        
    }

    private void Move()
    {
        inputX = Input.GetAxis("Horizontal");
        if (Mathf.Abs(inputX) > 0.01)
        {
            playerAnimator.SetBool(isMoveID,true);
            Vector2 input = new Vector2(inputX*moveSpeed,playerRigidBody2D.velocity.y);
            Turn(input.x);
            playerRigidBody2D.velocity = input ;
        }
        else
        {
            playerAnimator.SetBool(isMoveID,false);
        }
    }

    public void Lock()
    {
        moveAndAttackLock = true;
    }

    public void UnLock()
    {
        moveAndAttackLock = false;
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerAnimator.SetBool(isAttackID,true);
            playerAnimator.SetBool(isMoveID,false);
        }
        else
        {
            playerAnimator.SetBool(isAttackID,false);
        }
    }

    public override void Init()
    {
        Player = playerGo.GetComponentInChildren<MyPlayable>();
    }

    public override void Tick()
    {
        
    }

    public void Test()
    {
        playerAnimator.Play("Skill_2");
        //Debug.log("1");
    }
}
