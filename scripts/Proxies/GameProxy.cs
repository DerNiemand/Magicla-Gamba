using Godot;
using System;
using System.Text.Json;
using System.Threading.Tasks;
public partial class GameProxy : Control, INetworkReciever
{
    [Export]
    PackedScene cardScene;

    [Export]
    PackedScene energyCardScene;

    [Export]
    EndScreen endScreen;

    [Export]
    EnergyCounter energyCounter;

    ActiveCardProxy activeCardProxy;
    HandCardProxy handCardProxy;
    EnemyCardProxy enemyActiveCardProxy;

    private int currentHistoryStep;
    public override void _Ready()
    {
        currentHistoryStep = 0;
        activeCardProxy = GetNode<ActiveCardProxy>("ActiveCard");
        handCardProxy = GetNode<HandCardProxy>("HandCards");
        enemyActiveCardProxy = GetNode<EnemyCardProxy>("EnemyCards/ActiveCard");
        NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.ClientGameReadyMsg);

    }

    public void HandleMessage(long SenderId, int messageTypeInt, string message)
    {
        MessageType messageType = (MessageType)messageTypeInt;
        switch (messageType)
        {
            case MessageType.GameStateMsg:
                HandleGameStateMessage(message);
                break;

            case MessageType.LoginResp:
            case MessageType.ReadyResp:
            case MessageType.GameStartMsg:
            case MessageType.ClientGameReadyMsg:
            case MessageType.LoginMsg:
            case MessageType.ReadyMsg:
            case MessageType.TurnStartMsg:
            case MessageType.TurnEndMsg:
            case MessageType.UnitPlayedMsg:
            case MessageType.SpellPlayedMsg:
            case MessageType.UnitAttackMsg:
            case MessageType.ActiveUnitSwitchedMsg:
            case MessageType.EnergyDraftMsg:
            case MessageType.UnitAbilityUsedMsg:
                break;
            default:
                GD.PrintErr("The GameProxy recieved a MessageType that it does not know how to handle");
                break;
        }

    }


    private async void HandleGameStateMessage(string message)
    {
        GameStateMessage data = JsonSerializer.Deserialize<GameStateMessage>(message);
        if (data == null)
        {
            return;
        }

        GetNode<TextureButton>("TurnEndButton").Disabled = !data.PlayersTurn;
        energyCounter.SetEnergies(data.Energy.energies);

        for (currentHistoryStep += 0; currentHistoryStep < data.History.Count; currentHistoryStep++)
        {
            var action = data.History[currentHistoryStep];
            switch (action.type)
            {
                case ActionType.SpellPlayed:
                    if (action.player.Equals(Multiplayer.GetUniqueId()))
                    {
                        handCardProxy.RemoveCard(action.card);

                        switch (action.card.abilityName)
                        {
                            case ("Heal"):
                            case ("HealActive"):
                                if (action.targetedBenchIndex == -1)
                                {
                                    activeCardProxy.TryPlayBuffVfx();
                                }
                                else if (GetNode("BenchCards").GetChildCount() > action.targetedBenchIndex)
                                {
                                    var benchCardProxy = GetNode("BenchCards").GetChild(action.targetedBenchIndex) as BenchCardProxy;
                                    benchCardProxy.TryPlayBuffVfx();
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (action.card.abilityName)
                        {
                            case ("Heal"):
                            case ("HealActive"):
                                if (action.targetedBenchIndex == -1)
                                {
                                    enemyActiveCardProxy.TryPlayBuffVfx();
                                }
                                else if (GetNode("EnemyCards/BenchCards").GetChildCount() > action.targetedBenchIndex)
                                {
                                    var benchCardProxy = GetNode("EnemyCards/BenchCards").GetChild(action.targetedBenchIndex) as EnemyCardProxy;

                                    benchCardProxy.TryPlayBuffVfx();
                                }
                                break;
                            case ("RedrawHand"):
                                handCardProxy.ClearHandCards();
                                break;
                        }
                    }
                    break;

                case ActionType.UnitPlayed:
                    if (action.player.Equals(Multiplayer.GetUniqueId()))
                    {
                        if (action.targetedBenchIndex == -1)
                        {
                            handCardProxy.RemoveCard(data.BoardData.ownBoardCards.activeCard);
                            activeCardProxy.UpdateCard(data.BoardData.ownBoardCards.activeCard);
                        }
                        else if (GetNode("BenchCards").GetChildCount() > action.targetedBenchIndex)
                        {
                            handCardProxy.RemoveCard(data.BoardData.ownBoardCards.benchCards[action.targetedBenchIndex]);
                            var benchCardProxy = GetNode("BenchCards").GetChild(action.targetedBenchIndex) as BenchCardProxy;
                            benchCardProxy.UpdateCard(data.BoardData.ownBoardCards.benchCards[action.targetedBenchIndex]);
                            benchCardProxy.TryPlayPlaceVfx();
                        }
                    }
                    else
                    {
                        if (action.targetedBenchIndex == -1)
                        {
                            enemyActiveCardProxy.UpdateCard(data.BoardData.enemyBoardCards.activeCard);
                        }
                        else if (GetNode("EnemyCards/BenchCards").GetChildCount() > action.targetedBenchIndex)
                        {
                            var benchCardProxy = GetNode("EnemyCards/BenchCards").GetChild(action.targetedBenchIndex) as EnemyCardProxy;
                            benchCardProxy.UpdateCard(data.BoardData.enemyBoardCards.benchCards[action.targetedBenchIndex]);
                            benchCardProxy.TryPlayPlaceVfx();
                        }
                    }
                    break;

                case ActionType.CardDrawn:
                    if (action.player.Equals(Multiplayer.GetUniqueId()))
                    {
                        handCardProxy.DrawHandCard(data.HandData.handCards[action.instigatingBenchIndex]);
                        await ToSignal(handCardProxy, HandCardProxy.SignalName.DrawAnimationEnd);
                    }
                    break;

                case ActionType.UnitAttacks:

                    if (action.player.Equals(Multiplayer.GetUniqueId()))
                    {
                        if (action.targetedBenchIndex == -1)
                        {
                            enemyActiveCardProxy.TryPlayHitVfx();
                            await ToSignal(enemyActiveCardProxy, EnemyCardProxy.SignalName.AnimationFinished);
                            if (!data.BoardData.enemyBoardCards.hasActiveCard)
                            {
                                enemyActiveCardProxy.TryPlayDiscardAnimation();
                                await ToSignal(enemyActiveCardProxy, EnemyCardProxy.SignalName.AnimationFinished);
                            }
                        }
                        else if (GetNode("EnemyCards/BenchCards").GetChildCount() > action.targetedBenchIndex)
                        {
                            var benchCardProxy = GetNode("EnemyCards/BenchCards").GetChild(action.targetedBenchIndex) as EnemyCardProxy;

                            benchCardProxy.TryPlayHitVfx();
                            await ToSignal(benchCardProxy, EnemyCardProxy.SignalName.AnimationFinished);


                            if (!data.BoardData.enemyBoardCards.benchCards.ContainsKey(action.targetedBenchIndex))
                            {
                                benchCardProxy.TryPlayDiscardAnimation();
                                await ToSignal(benchCardProxy, EnemyCardProxy.SignalName.AnimationFinished);
                            }
                        }
                    }
                    else
                    {
                        if (action.targetedBenchIndex == -1)
                        {
                            activeCardProxy.TryPlayHitVfx();
                            await ToSignal(activeCardProxy, ActiveCardProxy.SignalName.AnimationFinished);
                            if (!data.BoardData.ownBoardCards.hasActiveCard)
                            {
                                activeCardProxy.TryPlayDiscardAnimation();
                                await ToSignal(activeCardProxy, ActiveCardProxy.SignalName.AnimationFinished);

                            }
                        }
                        else if (GetNode("BenchCards").GetChildCount() > action.targetedBenchIndex)
                        {
                            var benchCardProxy = GetNode("BenchCards").GetChild(action.targetedBenchIndex) as BenchCardProxy;
                            benchCardProxy.TryPlayHitVfx();
                            await ToSignal(benchCardProxy, BenchCardProxy.SignalName.AnimationFinished);

                            if (!data.BoardData.ownBoardCards.benchCards.ContainsKey(action.targetedBenchIndex))
                            {
                                benchCardProxy.TryPlayDiscardAnimation();
                                await ToSignal(benchCardProxy, BenchCardProxy.SignalName.AnimationFinished);
                            }
                        }
                    }
                    break;

                case ActionType.UnitAbilityUsed:
                    if (action.player.Equals(Multiplayer.GetUniqueId()))
                    {
                        string ability = "";

                        if (action.instigatingBenchIndex == -1)
                        {
                            ability = data.BoardData.ownBoardCards.activeCard.abilityName;
                        }
                        else
                        {
                            ability = data.BoardData.ownBoardCards.benchCards[action.instigatingBenchIndex].abilityName;
                        }

                        switch (ability)
                        {
                            case ("Heal"):
                            case ("HealSelf"):
                            case ("HealActive"):
                                if (action.targetedBenchIndex == -1)
                                {
                                    activeCardProxy.TryPlayBuffVfx();
                                }
                                else if (GetNode("BenchCards").GetChildCount() > action.targetedBenchIndex)
                                {
                                    var benchCardProxy = GetNode("BenchCards").GetChild(action.targetedBenchIndex) as BenchCardProxy;
                                    benchCardProxy.TryPlayBuffVfx();
                                }
                                break;
                        }
                    }
                    else
                    {
                        string ability = "";

                        if (action.instigatingBenchIndex == -1)
                        {
                            ability = data.BoardData.ownBoardCards.activeCard.abilityName;
                        }
                        else
                        {
                            ability = data.BoardData.enemyBoardCards.benchCards[action.instigatingBenchIndex].abilityName;
                        }

                        switch (ability)
                        {
                            
                            case ("Heal"):
                            case ("HealSelf"):
                            case ("HealActive"):
                                if (action.targetedBenchIndex == -1)
                                {
                                    enemyActiveCardProxy.TryPlayBuffVfx();
                                }
                                else if (GetNode("EnemyCards/BenchCards").GetChildCount() > action.targetedBenchIndex)
                                {
                                    var benchCardProxy = GetNode("EnemyCards/BenchCards").GetChild(action.targetedBenchIndex) as EnemyCardProxy;

                                    benchCardProxy.TryPlayBuffVfx();
                                }
                                break;

                            case ("RedrawHand"):
                                handCardProxy.ClearHandCards();
                                break;
                        }
                    }
                    break;

                case ActionType.ActiveCardSwitched:
                    if (action.player.Equals(Multiplayer.GetUniqueId()))
                    {
                        if (GetNode("BenchCards").GetChildCount() > action.instigatingBenchIndex && action.instigatingBenchIndex >= 0)
                        {
                            var benchCardProxy = GetNode("BenchCards").GetChild(action.instigatingBenchIndex) as BenchCardProxy;

                            benchCardProxy.TryPlaySummonActiveAnimation();
                            benchCardProxy.AnimationFinished += () => benchCardProxy.UpdateCard(data.BoardData.ownBoardCards.benchCards[action.instigatingBenchIndex]);
                            activeCardProxy.TryPlaySummonBenchAnimation(action.instigatingBenchIndex);
                            activeCardProxy.AnimationFinished += () => activeCardProxy.UpdateCard(data.BoardData.ownBoardCards.activeCard);
                        }
                    }
                    else
                    {
                        if (GetNode("EnemyCards/BenchCards").GetChildCount() > action.instigatingBenchIndex && action.instigatingBenchIndex >= 0)
                        {
                            var enemyCardProxy = GetNode("EnemyCards/BenchCards").GetChild(action.instigatingBenchIndex) as EnemyCardProxy;

                            enemyCardProxy.TryPlaySummonActiveAnimation();
                            enemyCardProxy.AnimationFinished += () => enemyCardProxy.UpdateCard(data.BoardData.enemyBoardCards.benchCards[action.instigatingBenchIndex]);
                            enemyActiveCardProxy.TryPlaySummonBenchAnimation(action.instigatingBenchIndex);
                            enemyActiveCardProxy.AnimationFinished += () => enemyActiveCardProxy.UpdateCard(data.BoardData.enemyBoardCards.activeCard);
                        }
                    }
                    break;

                case ActionType.GameWin:
                    endScreen.GameOver(action.player.Equals(Multiplayer.GetUniqueId()));
                    break;

                default:
                    GD.Print("If you read this, i fucked up");
                    break;
            }
        }

        foreach (var child in GetNode("BenchCards").GetChildren())
        {
            if (child is BenchCardProxy card)
            {
                if (data.BoardData.ownBoardCards.benchCards.ContainsKey(card.GetIndex()))
                {
                    card.UpdateCard(data.BoardData.ownBoardCards.benchCards[card.GetIndex()]);
                }
                else
                {
                    card.RemoveCard();
                }
            }
        }

        if (data.BoardData.ownBoardCards.hasActiveCard)
        {
            activeCardProxy.UpdateCard(data.BoardData.ownBoardCards.activeCard);
        }
        else
        {
            activeCardProxy.RemoveCard();
        }


        foreach (var child in GetNode("EnemyCards/BenchCards").GetChildren())
        {
            if (child is EnemyCardProxy card)
            {
                if (data.BoardData.enemyBoardCards.benchCards.ContainsKey(card.GetIndex()))
                {
                    card.UpdateCard(data.BoardData.enemyBoardCards.benchCards[card.GetIndex()]);
                }
                else
                {
                    card.RemoveCard();
                }
            }
        }

        if (data.BoardData.enemyBoardCards.hasActiveCard)
        {
            enemyActiveCardProxy.UpdateCard(data.BoardData.enemyBoardCards.activeCard);
        }
        else
        {
            enemyActiveCardProxy.RemoveCard();
        }

        foreach (var child in GetNode("EnergyCards").GetChildren())
        {
            GetNode("EnergyCards").RemoveChild(child);
        }

        foreach (var roll in data.UnChosenRolls)
        {
            var engergyCard = energyCardScene.Instantiate<EnergyCard>();
            engergyCard.SetDraftTurnIndicatorActive(data.PlayersDraftTurn);
            GetNode("EnergyCards").AddChild(engergyCard);
            engergyCard.SetEnergies(roll);
        }

        handCardProxy.SetHandCards(data.HandData.handCards);
    }

    public void _OnTurnEndButtonPressed()
    {
        NetworkManager.Instance.SendMessage(NetworkManager.ServerID, MessageType.TurnEndMsg);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
    }
}
