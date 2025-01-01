using Godot;
using System;

public class StateGemDrag : StateGem
{
    public override void EnterState(Gem g)
    {
        // GD.Print("Enter: " + this.ToString() + " - " + g.Name);
        GameManager.GRID.setValidCells(g.x0, g.y0, true);
    }

    public override void ExitState(Gem g, StateGem nextState)
    {
        GameManager.GRID.setValidCells(g.x0, g.y0, false);
        //GameManager.GRID.doSnapBack();
    }

    public override void Update(Gem g, double delta)
    {
        Vector3 mouseVector = GameManager.GM.GetMouseCoordinates3D();
        g.Position = g.Position.Lerp(mouseVector, (float)(g.speed * delta));
        GameManager.GRID.processDrag(mouseVector, g.x0, g.y0);
    }    

    public override void HandleInput(Gem g, InputEvent evt)
    {
        //release click event
        if (evt.IsActionReleased("click"))
        {
            Vector3 mouseVector = GameManager.GM.GetMouseCoordinates3D();
            bool validDragSpot = GameManager.GRID.processDrag(mouseVector, g.x0, g.y0);

            //TODO change auto snap back
            if(validDragSpot)
            {
                //get drop point from grid, adjust dx and dy, switch to SnapPreview TODO change to else
                g.dx = GameManager.GRID.dragX - g.x0;
                g.dy = GameManager.GRID.dragY - g.y0;
                g.changeState(g.tryFreeze);
            }
            else
            {
                g.changeState(g.snapBack);
            }
        }
    }

    public override void Trigger(Gem g, string s) { }
}