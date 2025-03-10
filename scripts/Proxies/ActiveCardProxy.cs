using Godot;
using System.Linq;
using System.Text.Json;

public partial class ActiveCardProxy : BenchCardProxy
{
    private bool firstAttack = true;

    public override void _Ready()
    {
        benchIndex = -1;
    }

    new public void _OnGuiInput(InputEvent inputEvent)
    {
        if(inputEvent.IsActionPressed("LeftClick"))
        {
            if(this.TryGetFirstChild(out CardBase card))
            {
                battleUi.ActivateCardActionUI(card, -1);
                AcceptEvent();
            }
        }
    }

    public bool TryPlaySummonBenchAnimation(int benchIndex)
    {
        if(TryGetCardChild(out CardBase cardNode))
        {
            cardNode.PlaySummonBenchAnimation(benchIndex);
            return true;
        }
        return false;
    }
}
