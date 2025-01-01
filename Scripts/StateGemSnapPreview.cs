using Godot;
using System;

public class StateGemSnapPreview : StateGem
{
    public override void EnterState(Gem g) { }

    public override void ExitState(Gem g, StateGem nextState) { }

    public override void Update(Gem g, double delta)
    {
        Vector3 snapTo = new Vector3(g.x0 + g.dx, g.y0 + g.dy, 0);

        if (g.Position != snapTo)
        {
            g.Position = g.Position.MoveToward(snapTo, (float)(g.speed * delta));
        }
        else
        {
            g.changeState(g.idlePreview);
        }
    }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s) { }
}