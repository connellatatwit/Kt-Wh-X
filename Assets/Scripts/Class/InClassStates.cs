using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InClassStates
{
    public enum STATE
    {
        PLAY,
        GAMEOVER
    }
    public enum EVENT
    {
        UPDATE,
        EXIT,
        ENTER
    }

    public STATE name;
    protected EVENT stage;
    protected InClassStates newState;

    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }
    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    }
    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    }

    public InClassStates process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if(stage == EVENT.EXIT)
        {
            Exit();
            return newState;
        }
        return this;
    }
}

public class GameOver : InClassStates
{
    public GameOver() : base()
    {
        name = STATE.GAMEOVER;
        stage = EVENT.ENTER;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        newState = new Play();
        base.Exit();
    }
}

public class Play : InClassStates
{
    public Play() : base()
    {
        name = STATE.PLAY;
        stage = EVENT.ENTER;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        newState = new GameOver();
        base.Exit();
    }
}
