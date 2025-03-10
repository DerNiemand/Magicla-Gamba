using Godot;
using System;

public partial class EndScreen : CenterContainer
{

    [Export]
    Control[] victoryUi;

    [Export]
    Control[] defeatUi;

    public void GameOver(bool won)
    {
        Visible = true;

        if (won)
        {
            foreach(Control c in victoryUi)
            {
                c.Visible = true;
            }
        }
        else
        {
            foreach (Control c in defeatUi)
            {
                c.Visible = true;
            }
        }
    }
}
