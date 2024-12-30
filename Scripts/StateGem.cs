using Godot;
using System;


public abstract class StateGem
{
    public abstract void EnterState(Gem g);
    public abstract void ExitState(Gem g);
    public abstract void Update(Gem g, double delta);
    public abstract void HandleInput(Gem g, InputEvent evt);
    public abstract void Trigger(Gem g, string s);
}