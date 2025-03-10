using Godot;
using System;

public partial class EnemyCardProxy : CenterContainer
{
    [Export]
    BattleUi battleUi;


    [Signal]
    public delegate void OnClickEventHandler();
    [Signal]
    public delegate void AnimationFinishedEventHandler();


    public void _OnGuiInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("LeftClick"))
        {
           
            if(this.HasChild<CardBase>())
            {
                AcceptEvent();
            }
        }

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
            cardNode.AnimationEnd += QueueSort;
            cardNode.RightClickInput += PreviewCard;
            AddChild(cardNode);
        }

        cardNode.cardData = card;

        cardNode.UpdateData();
    }

    public void RemoveCard()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            RemoveChild(cardNode);
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
            cardNode.PlayEnemyDiscardAnimation();
            cardNode.AnimationEnd += () => cardNode.QueueFree();
            return true;
        }
        return false;
    }

    public bool TryPlaySummonActiveAnimation()
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlayEnemySummonActiveAnimation();
            return true;
        }
        return false;
    }

    public bool TryPlaySummonBenchAnimation(int benchIndex)
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlayEnemySummonBenchAnimation(benchIndex);
            return true;
        }
        return false;
    }

    public void PreviewCard(CardBase card)
    {
        battleUi.ActivateInspectUI(card);
    }
}
