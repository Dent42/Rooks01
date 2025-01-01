using Godot;
using System;

public class StateGemBurst : StateGem
{
    public override void EnterState(Gem g) { }

    public override void ExitState(Gem g, StateGem nextState) { }

    public override void Update(Gem g, double delta)
    { 
        //TODO Run animation, then destroy

        //update grid and destroy
        GameManager.GRID.processBurst(g);
        g.QueueFree();
    }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s) { }
}