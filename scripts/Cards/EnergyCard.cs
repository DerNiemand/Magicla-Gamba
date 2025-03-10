using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class EnergyCard : ColorRect
{

    [Export]
    TextureRect draftTurnIndicator;

    [Export]
    SpriteFrames EnergySymbols;

    [Export]
    TextureRect[] EnergyIcons;

    public void SetEnergies(Dictionary<EnergyType, int> energies)
    {
        int i = 0;
        foreach(EnergyType type in energies.Keys)
        {
            for(int j = 0; j < energies[type]; j++)
            {
                EnergyIcons[i].Texture = EnergySymbols.GetFrameTexture(type.ToString(), 0);
                i++;
            }
        }
    }

    public void _OnGuiInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("LeftClick"))
        {
            EnergyDraftMessage msg = new EnergyDraftMessage(GetIndex());
            var msgJson = JsonSerializer.Serialize(msg);
            NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.EnergyDraftMsg, msgJson);
        }
    }

    public void SetDraftTurnIndicatorActive(bool active)
    {
        draftTurnIndicator.Visible = active;
    }

}
