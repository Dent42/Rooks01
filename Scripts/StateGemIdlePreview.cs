using Godot;
using System;

public class StateGemIdlePreview : StateGem
{
    public override void EnterState(Gem g) { }

    public override void ExitState(Gem g) { }

    public override void Update(Gem g, double delta) { }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s)
    {
        if(s == Gem.CANCEL)
        {
            g.changeState(g.snapBack);
        }
    }
}