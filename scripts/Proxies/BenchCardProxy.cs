using Godot;
using System.Text.Json;

public partial class BenchCardProxy : CenterContainer
{
    [Signal]
    public delegate void AnimationFinishedEventHandler();


    [Export]
    protected BattleUi battleUi;

    [Export]
    private Vector2I restingCardSize;
    protected int benchIndex;

    public override void _Ready()
    {
        benchIndex = GetIndex();
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if(data.Obj is Control control)
        {
            if(control.GetChild(0) is CardBase card)
            {
                if(card.cardData.cardType == CardType.summon || card.cardData.cardType == CardType.hero)
                {
                    if(!this.HasChild<CardBase>())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        var control = data.Obj as Control;
        var card = control.GetChild(0) as CardBase;
        var newCard = card.Duplicate(15) as CardBase;
        var cardVieportTexture = newCard.GetNode<SubViewport>("SubViewport").GetTexture();
        newCard.Texture = cardVieportTexture;
        newCard.cardData = card.cardData;
        newCard.RestingSize = restingCardSize;
        newCard.AnimationEnd += () => EmitSignal(SignalName.AnimationFinished);
        newCard.AnimationEnd += () => QueueSort();
        AddChild(newCard);
        var msg = new UnitPlayedMessage(newCard.cardData, benchIndex);
        var msgJson = JsonSerializer.Serialize(msg);
        NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.UnitPlayedMsg, msgJson);

    }

    public void UpdateCard(CardData card)
    {
        CardBase cardNode = null;
        foreach(var child in GetChildren())
        {
            if(child is CardBase)
            {
                cardNode = child as CardBase;
                break;
            }
        }

        if(cardNode == null)
        {
            cardNode = GD.Load<PackedScene>("res://Cards/base_card.tscn").Instantiate() as CardBase;
            cardNode.AnimationEnd += () => EmitSignal(SignalName.AnimationFinished);
            cardNode.AnimationEnd += () => QueueSort();
            cardNode.AnimationEnd += () => GD.Print("Oh it fwooshed");
            AddChild(cardNode);
        }

        cardNode.cardData = card;

        cardNode.UpdateData();
    }

    public void RemoveCard()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.QueueFree();
        }
    }

    public void _OnGuiInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("LeftClick"))
        {
            if(inputEvent.IsActionPressed("LeftClick"))
            {
                if(this.TryGetFirstChild(out CardBase card))
                {
                    battleUi.ActivateCardActionUI(card, benchIndex);
                    AcceptEvent();
                }
            }
        }
    }

    public bool TryGetCardChild(out CardBase card)
    {
        CardBase cardNode = null;
        card = new CardBase();
        foreach(var child in GetChildren())
        {
            if(child is CardBase)
            {
                cardNode = child as CardBase;
                break;
            }
        }

        if(cardNode != null)
        {
            card = cardNode;
            return true;
        }
        return false;
    }

    public bool TryPlayHitVfx()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlayHitVfx();
            return true;
        }
        return false;
    }

    public bool TryPlayPlaceVfx()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlayPlaceVfx();
            return true;
        }
        return false;
    }

    public bool TryPlayBuffVfx()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlayBuffVfx();
            return true;
        }
        return false;
    }

    public bool TryPlayDiscardAnimation()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlayDiscardAnimation();
            cardNode.AnimationEnd += () => cardNode.QueueFree();
            return true;
        }
        return false;
    }

    public bool TryPlaySummonActiveAnimation()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlaySummonActiveAnimation();
            return true;
        }
        return false;
    }
}