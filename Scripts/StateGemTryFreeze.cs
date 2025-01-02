using Godot;
using System;

public class StateGemTryFreeze : StateGem
{
    public override void EnterState(Gem g)
    {
        //if the gem is being dropped on an invalid spot, snap back
        if(!GameManager.GRID.canFreezePreview())
        {
            g.changeState(g.snapBack);
            GameManager.GRID.doSnapBack();
        }
    }

    public override void ExitState(Gem g, StateGem nextState)
    {
        if(nextState == g.freeze)
        {
            GameManager.GRID.doFreezePreview();
        }
    }

    public override void Update(Gem g, double delta) 
    { 
        Vector3 snapTo = new Vector3(g.x0 + g.dx, g.y0 + g.dy, 0);

        if (g.Position != snapTo)
        {
            g.Position = g.Position.MoveToward(snapTo, (float)(g.speed * delta));
        }
        else
        {
            g.changeState(g.freeze);
        }
    }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s) { }
}