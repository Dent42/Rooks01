using Godot;
using System;

public class StateGemFreeze : StateGem
{
    public override void EnterState(Gem g)
    {
        GameManager.GRID.numFrozen++;

        if(GameManager.GRID.numFrozen == GameManager.GRID.totalGems)
        {
            bool hasAdjacents = GameManager.GRID.checkAdjacents();
            
            if(!hasAdjacents)
            {
                //TODO add code for setting grid idle, reset preview matrix
                GameManager.GRID.doIdle();
            }
        }
    }

    public override void ExitState(Gem g, StateGem nextState)
    { 
        GameManager.GRID.numFrozen--;
    }

    public override void Update(Gem g, double delta)
    {

        //check for burst
        if(g.adjacentX >1 || g.adjacentY >1)
        {
            g.changeState(g.burst);
        }

        //check for fall
        if(GameManager.GRID.checkFall(g))
        {
            g.changeState(g.fall);
        }

        //continuously check if top row is empty
        GameManager.GRID.checkTopRow();
    }

    public override void HandleInput(Gem g, InputEvent evt) { }

    public override void Trigger(Gem g, string s)
    {
        if(s == Gem.IDLE)
        {
            g.changeState(g.idle);
        }
    }
}