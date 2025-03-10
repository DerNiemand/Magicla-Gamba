using Godot;
using System;

public partial class InspectUi : HBoxContainer
{

    [Export]
    CenterContainer cardContainer;
    [Export]
    Label AbilityNameLabel;
    [Export]
    Label abilityDescriptionLabel;

    [Export]
    private Vector2I restingCardSize;

    [Export]
    CostCounterUI costCounter;

    public void Setup(CardBase card)
    {
        Visible = true;
        var newCard = card.Duplicate() as CardBase;
        var cardVieportTexture = newCard.GetNode<SubViewport>("SubViewport").GetTexture();
        newCard.Texture = cardVieportTexture;
        newCard.RestingSize = restingCardSize;
        newCard.cardData = card.cardData;
        AbilityNameLabel.Text = newCard.cardData.abilityName;
        (AbilityNameLabel.GetParent() as Control).Visible = !string.IsNullOrEmpty(card.cardData.abilityName);
        costCounter.SetCost(card.cardData.EnergyType, card.cardData.cost);
        abilityDescriptionLabel.Text = newCard.cardData.abilityDescription;
        (abilityDescriptionLabel.GetParent() as Control).Visible = !string.IsNullOrEmpty(newCard.cardData.abilityDescription);
        cardContainer.AddChild(newCard);
    }

    public void TearDown()
    {
        Visible = false;
        if (cardContainer.TryGetFirstChild(out CardBase card))
        {
            cardContainer.RemoveChild(card);
        }
    }

    public void _OnBackButtonPressed()
    {
        TearDown();
    }

}
