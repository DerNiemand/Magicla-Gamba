using Godot;

public partial class CardBase : TextureRect
{
    [Export]
    Sprite3D frontSprite;
    [Export]
    Sprite3D backSprite;
    [Export]
    Sprite3D frameSprite;
    [Export]
    Label3D HealthLabel;
    [Export]
    Label3D DamageLabel;
    [Export]
    Label3D RetreatCostLabel;

    [Export]
    SpriteFrames illustrations;

    [Export]
    CostCounter CostCounter;

    [Export]
    AnimatedSprite3D vfxSprite;


    [Signal]
    public delegate void AnimationEndEventHandler();

    [Signal]
    public delegate void RightClickInputEventHandler(CardBase card);


    private Vector2I restingSize = new Vector2I(148, 205);
    public Vector2I RestingSize
    {
        private get
        {
            return restingSize;
        }
        set
        {
            restingSize = value;
        }
    }

    public CardData cardData;

    private AnimationPlayer animPlayer;
    private AnimationPlayer flipAnimPlayer;

    public override void _Ready()
    {
        UpdateData();
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        flipAnimPlayer = GetNode<AnimationPlayer>("FlipAnimationPlayer");
        GetNode<SubViewport>("SubViewport").Size = restingSize;
        vfxSprite.AnimationFinished += vfxSprite.Hide;
        vfxSprite.AnimationFinished += () => EmitSignal(SignalName.AnimationEnd);

    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("RightClick"))
        {
            EmitSignal(SignalName.RightClickInput, this);
        }
    }

    public void UpdateData()
    {
        if(illustrations.HasAnimation(cardData.name))
        {
            frontSprite.Texture = illustrations.GetFrameTexture(cardData.name, 0);
        }

        HealthLabel.Text = cardData.health.ToString();
        HealthLabel.Visible = cardData.health > 0;
        DamageLabel.Text = cardData.damage.ToString();
        DamageLabel.Visible = cardData.damage > 0;
        RetreatCostLabel.Text = cardData.retreatCost.ToString();
        RetreatCostLabel.Visible = cardData.retreatCost > 0;
        CostCounter.SetCost(cardData.EnergyType, cardData.cost);
    }

    public void OnAnimationEnd()
    {
        EmitSignal(SignalName.AnimationEnd);
    }

    public void SetPositionToGlobal()
    {
        Position = GlobalPosition;
    }

    public void PlayPlaceVfx()
    {
        vfxSprite.Visible = true;
        vfxSprite.Play("Place");
    }

    public void PlayHitVfx()
    {
        vfxSprite.Visible = true;
        vfxSprite.Play("Hit");
    }

    public void PlayBuffVfx()
    {
        vfxSprite.Visible = true;
        vfxSprite.Play("Buff");
    }

    public void PlayFlipCardAnimation()
    {
        flipAnimPlayer.Play("flip");
    }

    public void PlayDrawAnimation()
    {
        animPlayer.Play("Draw");
    }

    public void PlayDiscardAnimation()
    {
        SetPositionToGlobal();
        animPlayer.Play("DiscardPlayer");
    }

    public void PlayEnemyDiscardAnimation()
    {
        SetPositionToGlobal();
        animPlayer.Play("DiscardEnemy");
    }

    public void PlaySummonActiveAnimation()
    {
        SetPositionToGlobal();
        animPlayer.Play("PlayerSummonActive");
    }

    public void PlayEnemySummonActiveAnimation()
    {
        SetPositionToGlobal();
        animPlayer.Play("EnemySummonActive");
    }

    public void PlaySummonBenchAnimation(int slot)
    {
        string animName = "PlayerSummonBench" + (slot + 1);
        SetPositionToGlobal();
        animPlayer.Play(animName);
    }

    public void PlayEnemySummonBenchAnimation(int slot)
    {
        string animName = "EnemySummonBench" + (slot + 1);
        SetPositionToGlobal();
        animPlayer.Play(animName);
    }
}
