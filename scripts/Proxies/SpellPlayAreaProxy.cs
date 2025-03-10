using Godot;
using System;
using System.Text.Json;

public partial class SpellPlayAreaProxy : Control
{
    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if(data.Obj is Control control)
        {
            if(control.GetChild(0) is CardBase card)
            {
                if(card.cardData.cardType == CardType.spell || card.cardData.cardType == CardType.item)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        var control = data.Obj as Control;
        var card = control.GetChild(0) as CardBase;
        var msg = new SpellPlayedMessage(card.cardData, -1);
        var msgJson = JsonSerializer.Serialize(msg);
        NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.SpellPlayedMsg, msgJson);
    }
}
