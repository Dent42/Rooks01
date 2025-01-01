using Godot;
using System;

public class StateGemIdle : StateGem
{
    public override void EnterState(Gem g) { }

    public override void ExitState(Gem g, StateGem nextState) { }

    public override void Update(Gem g, double delta)
    {
        //idle, do nothing
    }

    public override void HandleInput(Gem g, InputEvent evt)
    {
        //press click event, change state to drag
        if (evt.IsActionPressed("click"))
        {
            // GD.Print("Input Event: " + g.Name);
            g.changeState(g.drag);
        }
    }

    public override void Trigger(Gem g, string s)
    {
        if(s == Gem.PREVIEW)
        {
            g.changeState(g.snapPreview);
        }
        else if(s == Gem.FREEZE)
        {
            g.changeState(g.freeze);
        }
    }
}