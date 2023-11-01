using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner:IComparable<Runner>//行动条上移动的橘色
{
    private CharacterCamp camp;

    public RunnerUI runnerUI;
    
    private BaseCharacter character;

    public BaseCharacter Character
    {
        get => character;
    }

    public Runner(BaseCharacter _character)
    {
        character = _character;
        character.BindRunner(this);

    }
    public float startPos = 0; //初始位置
    private float endPos = 10000;
    private float curPos = 0; //当前位置

    public float CurPos
    {
        get => curPos;
        set {
            if (value > endPos)
            {
                curPos = endPos;
                
            }
            else
            {
                curPos = value;
            }
        }
    }
    private bool posChangeFlag = true;
    private float remainTime;

    public float RemainTime
    {
        get
        {
            if (posChangeFlag)
            {
                remainTime=(endPos - curPos) / character.BattleCharacterStateData.Speed;
                posChangeFlag = false;
                return remainTime;

            }
            return remainTime;
        }
    }

    public void Move(float time) //移动，传入为最快抵达终点的时间
    {
        curPos += character.BattleCharacterStateData.Speed * time;
        posChangeFlag = true;
    }

    public void MoveDistance(float distance) //移动一定距离
    {
        curPos += distance;
        posChangeFlag = true;
    }

    public void FinishRun() //抵达终点并执行完动作后重返起点
    {
        curPos = startPos;
        posChangeFlag = true;
    }

    public void ToEnd() //达到终点
    {
        curPos = endPos;
        posChangeFlag = true;
    }

    public int CompareTo(Runner other)
    {
        return RemainTime.CompareTo(other.RemainTime);
        FlipA a = new FlipA();
        Book b =new  Book(a);
    }


}
public interface Flip
{

    void FlipBook(Book book);
}

public class FlipA:Flip
{
    public void FlipBook(Book book)
    {
        //翻书方法A
    }
}
public class FlipB:Flip
{
    public void FlipBook(Book book)
    {
        //翻书方法B
    }
}

public class Book
{
    private Flip comp;

    public Book(Flip c)
    {
        comp = c;
    }

    void FlipSelf()
    {
        comp.FlipBook(this);
    }
}