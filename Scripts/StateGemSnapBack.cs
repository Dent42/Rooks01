using Godot;
using System;

public class StateGemSnapBack : StateGem
{
    public override void EnterState(Gem g)
    {
        // GD.Print("Enter: " + this.ToString() + " - " + g.Name);

        g.dx = 0;
        g.dy = 0;
        // g.adjacentX = 0;
        // g.adjacentY = 0;

        //TODO maybe better way to do this?
        //GameManager.GRID.doSnapBack();
    }

    public override void ExitState(Gem g) { }

    public override void Update(Gem g, double delta)
    {
        if (g.Position != g.getInitialPosition())
        {
            g.Position = g.Position.MoveToward(g.getInitialPosition(), (float)(g.speed * delta));
        }
        else
        {
            g.changeState(g.idle);
        }
    }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s) { }
}