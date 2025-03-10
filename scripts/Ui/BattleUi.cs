using Godot;

public partial class BattleUi : Control
{
    [Export]
    CardActionUi cardActionUi;

    [Export]
    InspectUi inspectUi;

    public void ActivateCardActionUI(CardBase card, int benchIndex)
    {
        cardActionUi.Setup(card,benchIndex);
    }
    public void DeactivateCardActionUI()
    {
        cardActionUi.TearDown();
    }

    public void ActivateInspectUI(CardBase card)
    {
        inspectUi.Setup(card);
    }

    public void DeactivateInspectUI()
    {
        inspectUi.TearDown();
    }
}
