using Godot;
using System;

public class StateGemFall : StateGem
{
    public override void EnterState(Gem g) { }

    public override void ExitState(Gem g, StateGem nextState) { }

    public override void Update(Gem g, double delta)
    { 
        

        if (g.Position != g.getInitialPosition())
        {
            g.Position = g.Position.MoveToward(g.getInitialPosition(), (float)(5 * delta));
        }
        else
        {
            //update matrix position

            //TODO change this to something else
            g.changeState(g.freeze);
        }
        
    }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s) { }
}