using Godot;
using System;

public partial class CostCounterUI : TextureRect
{
    [Export]
    SpriteFrames EnergyIcons;

    [Export]
    int iconSize;

    public void SetCost(EnergyType type, int amount)
    {
        Vector2 size;

        size = Size;
        size.X = amount * iconSize;
        CustomMinimumSize = size;
        Texture = EnergyIcons.GetFrameTexture(type.ToString(), 0);
    }
}
