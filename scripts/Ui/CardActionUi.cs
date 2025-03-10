using Godot;
using System.Text.Json;

public partial class CardActionUi : HBoxContainer
{
    [Export]
    CenterContainer cardContainer;
    [Export]
    TextureButton abilityButton;
    [Export]
    Label abilityDescriptionLabel;
    [Export]
    TextureButton moveToActiveButton;

    [Export]
    private Vector2I restingCardSize;

    [Export]
    CostCounterUI costCounter;

    private int benchIndex;

    public void Setup(CardBase card, int benchIndex)
    {
        Visible = true;
        this.benchIndex = benchIndex;
        var newCard = card.Duplicate() as CardBase;
        var cardVieportTexture = newCard.GetNode<SubViewport>("SubViewport").GetTexture();
        newCard.Texture = cardVieportTexture;
        newCard.RestingSize = restingCardSize;
        newCard.cardData = card.cardData;
        cardContainer.AddChild(newCard);
        abilityButton.Visible = !string.IsNullOrEmpty(card.cardData.abilityName);
        costCounter.SetCost(card.cardData.EnergyType,card.cardData.cost);
        abilityDescriptionLabel.Text = newCard.cardData.abilityDescription;
        (abilityDescriptionLabel.GetParent() as Control).Visible = !string.IsNullOrEmpty(newCard.cardData.abilityDescription);
        if (abilityButton.TryGetFirstChild(out Label abillityLabel))
        {
            abillityLabel.Text = card.cardData.abilityName;
        }
        moveToActiveButton.Visible = benchIndex != -1;
    }

    public void TearDown()
    {
        Visible = false;
        if(cardContainer.TryGetFirstChild(out CardBase card))
        {
            cardContainer.RemoveChild(card);
        }
        benchIndex = -2;
    }

    public void _OnAttackButtonPressed()
    {
        var msg = new UnitAttackMessage(benchIndex);
        var msgJason = JsonSerializer.Serialize(msg);

        NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.UnitAttackMsg, msgJason);
        TearDown();
    }

    public void _OnAbilityButtonPressed()
    {
        if(cardContainer.TryGetFirstChild(out CardBase card))
        {
            var msg = new UnitAbilityUsedMessage(benchIndex, -1, card.cardData.abilityName);
            var msgJason = JsonSerializer.Serialize(msg);

            NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.UnitAbilityUsedMsg, msgJason);
            TearDown();
        }
    }

    public void _OnMoveToActiveCardButtonPressed()
    {
        var msg = new ActiveUnitSwitchedMessage(benchIndex);
        var msgJason = JsonSerializer.Serialize(msg);

        NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.ActiveUnitSwitchedMsg, msgJason);

        TearDown();
    }

    public void _OnBackButtonPressed()
    {
        TearDown();
    }
}
