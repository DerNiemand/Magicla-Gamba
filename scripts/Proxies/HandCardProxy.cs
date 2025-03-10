using Godot;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class HandCardProxy : HBoxContainer

{
    [Export]
    private int restingBottomPosition;

    [Export]
    private int focusBottomPosition;

    [Export]
    private Vector2I restingCardSize;

    [Export]
    PackedScene cardScene;

    [Export]
    SpellPlayAreaProxy spellPlayArea;

    [Export]
    BattleUi battleUi;


    [Signal]
    public delegate void DrawAnimationEndEventHandler();

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if(GetCardAt(atPosition, out CardBase card))
        {
            var dragCard = card.Duplicate(14) as CardBase;
            dragCard.Size = card.Size;
            dragCard.cardData = card.cardData;
            card.Visible = false;
            var dragControl = new Control();
            dragControl.AddChild(dragCard);
            dragCard.Position = -dragCard.Size / 2;
            SetDragPreview(dragControl);
            spellPlayArea.Visible = card.cardData.cardType == CardType.spell || card.cardData.cardType == CardType.item;
            return dragControl;
        }
        return base._GetDragData(atPosition);
    }

    public bool GetCardAt(Vector2 atPosition, out CardBase cardAt)
    {
        cardAt = null;
        foreach(CardBase card in GetChildren())
        {
            if(new Rect2(card.Position, card.Size).HasPoint(atPosition))
            {
                cardAt = card;
                return true;
            }
        }
        return false;
    }

    public void DragEnded(bool success)
    {
        foreach(CardBase card in GetChildren())
        {
            if(card.Visible == true)
            {
                continue;
            }

            if(success)
            {
                card.QueueFree();
            }
            else
            {
                card.Visible = true;
            }
        }
    }

    public void SetAllCardsVisibility(bool visible)
    {
        foreach(CardBase card in GetChildren())
        {
            card.Visible = visible;
        }
    }

    public override void _Notification(int what)
    {
        if(what == NotificationDragEnd)
        {
            if(!IsDragSuccessful())
            {
                DragEnded(false);
            }
            spellPlayArea.Visible = false;
        }
    }

    public void _OnMouseEntered()
    {
        OffsetBottom = focusBottomPosition;
    }

    public void _OnMouseExited()
    {
        OffsetBottom = restingBottomPosition;
    }

    public void ClearHandCards()
    {
        foreach(var child in GetChildren())
        {
            RemoveChild(child);
        }
    }

    public void DrawHandCard(CardData card)
    {
        var newHandCard = cardScene.Instantiate() as CardBase;
        newHandCard.cardData = card;

        newHandCard.AnimationEnd += SortHand;
        newHandCard.AnimationEnd += () => EmitSignal(SignalName.DrawAnimationEnd);

        AddChild(newHandCard);
        newHandCard.PlayDrawAnimation();
    }

    public void SetHandCards(List<CardData> cards)
    {
        ClearHandCards();
        foreach(var card in cards)
        {
            var newHandCard = cardScene.Instantiate() as CardBase;
            newHandCard.cardData = card;
            newHandCard.RightClickInput += PreviewCard;

            AddChild(newHandCard);
        }
    }

    public void RemoveCard(CardData card)
    {
        foreach(var child in GetChildren())
        {
            var handCard = child as CardBase;
            if(handCard.cardData == card)
            {
                RemoveChild(child);
                return;
            }
        }
    }

    public void SortHand()
    {
        QueueSort();
    }

    public void PreviewCard(CardBase card)
    {
        battleUi.ActivateInspectUI(card);
    }

}
