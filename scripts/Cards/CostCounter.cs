using Godot;
using System;

public partial class CostCounter : Node3D
{
    [Export]
    Sprite3D[] CostIcons;

    [Export]
    SpriteFrames EnergyIcons;


    public void SetCost(EnergyType type, int amount)
    {
        for (int i = 0; i < CostIcons.Length; i++)
        {
            if (CostIcons.Length > i)
            {
                CostIcons[i].Texture = EnergyIcons.GetFrameTexture(type.ToString(), 0);
                CostIcons[i].Visible = i < amount;
            }
        }
        var pos = Position;
        pos.X = -0.175f * (amount - 1);
        Position = pos;
    }
}
