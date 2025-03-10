using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public struct GameData
{
    [JsonInclude]
    public BoardData boardState;
    [JsonInclude]
    public HandData ownHand;
    [JsonInclude]
    public Dictionary<EnergyType, int>[] unChosenRolls;
}
public partial class GameModel : INetworkReciever
{
    private BoardModel board;

    private PlayerModel currentPlayer;
    private PlayerModel currentDrafter;

    private CardImporter cardImporter = new CardImporter();

    private int turnCount = 1;

#nullable enable
    private PlayerModel? player1;
    private PlayerModel? player2;


    List<ActionData> history = new List<ActionData>();

    List<Dictionary<EnergyType, int>> unChosenRolls = new List<Dictionary<EnergyType, int>>();

    public GameModel()
    {
        Restart();
    }

    public void Restart()
    {
        history.Clear();
        board = new BoardModel();
        cardImporter.ImportCards();
    }

    public bool ConnectPlayer(string name, PlayerId playerId, LoginId loginId)
    {
        if(player1 == null)
        {
            var deckCards = cardImporter.Cards;
            foreach(var card in deckCards)
            {
                if(card.cardType == CardType.hero)
                {
                    deckCards.Remove(card);
                    if(board.TryPlayCard(true, card, -1))
                    {
                        history.Add(new ActionData(playerId, ActionType.UnitPlayed, 0, -1));
                    }
                    break;
                }
            }
            deckCards.AddRange(deckCards);
            var playerDeck = new DeckModel(deckCards);
            player1 = new PlayerModel(name, playerId, playerDeck);

            return true;
        }
        else if(player2 == null)
        {
            var deckCards = cardImporter.Cards;
            foreach(var card in deckCards)
            {
                if(card.cardType == CardType.hero)
                {
                    deckCards.Remove(card);
                    if(board.TryPlayCard(false, card, -1))
                    {
                        history.Add(new ActionData(playerId, ActionType.UnitPlayed, 0, -1));
                    }
                    break;
                }
            }
            deckCards.AddRange(deckCards);
            var playerDeck = new DeckModel(deckCards);
            player2 = new PlayerModel(name, playerId, playerDeck);

            return true;
        }
        playerId = new(-1);
        return false;
    }

    public bool PlayerReady(long playerId)
    {
        if(player1 == null || player2 == null)
        {
            return false;
        }

        if(player1.State.id.Equals(playerId))
        {
            player1.ReadyUp();
        }
        else if(player2.State.id.Equals(playerId))
        {
            player2.ReadyUp();
        }

        if(player1.State.ready && player2.State.ready)
        {
            StartGame();
        }

        return true;
    }

    private void StartGame()
    {
        var player1starts = Random.NextBool();

        currentPlayer = player1starts ? player1 : player2; 

        currentDrafter = currentPlayer;

        for(var i = 0; i < 5; i++)
        {
            player1.DrawCard();
            player2.DrawCard();
            history.Add(new ActionData(player1.State.id, ActionType.CardDrawn, player1.State.hand.handCards.Count - 1, 0));
            history.Add(new ActionData(player2.State.id, ActionType.CardDrawn, player2.State.hand.handCards.Count - 1, 0));
        }
        RollDice();

        NetworkManager.Instance.SendMessage(player1.State.id.Value, MessageType.GameStartMsg);
        NetworkManager.Instance.SendMessage(player2.State.id.Value, MessageType.GameStartMsg);
    }

    private void StartNextTurn()
    {
        if(unChosenRolls.Count > 0)
        {
            return;
        }

        currentPlayer.ClearEnergies();

        if(currentPlayer == player1)
        {
            currentPlayer = player2;
        }
        else
        {
            currentPlayer = player1;
        }
        currentPlayer.DrawCard();
        history.Add(new ActionData(currentPlayer.State.id, ActionType.CardDrawn, currentPlayer.State.hand.handCards.Count - 1, 0));

        if(turnCount % 2 == 0)
        {
            RollDice();
        }

        turnCount++;
    }
    private void RollDice()
    {
        foreach(var die in Dice)
        {
            unChosenRolls.Add(die.RollDice());
        }
    }

    private bool TryDraftEnergy(long playerId, int draftIndex)
    {
        if(unChosenRolls.Count == 0)
        {
            return false;
        }

        if(!currentDrafter.State.id.Equals(playerId))
        {
            return false;
        }

        foreach(var energy in unChosenRolls[draftIndex])
        {
            currentDrafter.AddEnergy(energy.Key, energy.Value);
        }
        unChosenRolls.RemoveAt(draftIndex);

        switch(unChosenRolls.Count)   //not quite safe, change if time
        {
            case 0:
            case 1:
            case 3:
            case 5:
            case 7:
                if(currentDrafter == player1)
                {
                    currentDrafter = player2;
                }
                else
                {
                    currentDrafter = player1;
                }
                break;
        }

        return true;

    }

    private bool PlayUnitToBoard(long playerId, CardData card, int benchIndex)
    {
        if(unChosenRolls.Count > 0)
        {
            return false;
        }
        if(!currentPlayer.State.id.Equals(playerId))
        {
            return false;
        }

        if(!currentPlayer.State.hand.handCards.Contains(card))
        {
            return false;
        }

        if(!currentPlayer.HasSufficientEnergies(card.EnergyType, card.cost))
        {
            return false;
        }

        bool success = board.TryPlayCard(currentPlayer == player1, card, benchIndex);
        if(success)
        {
            currentPlayer.RemoveHandCard(card);
            currentPlayer.SubtractEnergy(card.EnergyType, card.cost);
            history.Add(new ActionData(currentPlayer.State.id, ActionType.UnitPlayed, 0, benchIndex));
        }
        return success;
    }

    private bool UnitAttacks(long playerId, int benchIndex, int targetIndex)
    {
        if(unChosenRolls.Count > 0)
        {
            return false;
        }
        if(!currentPlayer.State.id.Equals(playerId))
        {
            return false;
        }

        if(!board.SideContainsCard(currentPlayer == player1, benchIndex, out CardData card))
        {
            return false;
        }

        if(!currentPlayer.HasSufficientEnergies(card.EnergyType, card.cost))
        {
            return false;
        }

        bool success = board.TryAttack(currentPlayer == player1, card.damage, -1, out bool alive);
        if(success)
        {

            currentPlayer.SubtractEnergy(card.EnergyType, card.cost);
            history.Add(new ActionData(currentPlayer.State.id, ActionType.UnitAttacks, benchIndex, targetIndex));

            if(!alive)
            {
                if(!board.SideContainsHeroCard(currentPlayer != player1))
                {
                    history.Add(new ActionData(currentPlayer.State.id,ActionType.GameWin,0,0));
                }
            }
        }

        return success;
    }

    private void UseUnitAbility(long playerId, int casterIndex, int targetIndex)
    {
        if(unChosenRolls.Count > 0)
        {
            return;
        }
        if(!currentPlayer.State.id.Equals(playerId))
        {
            return;
        }

        if(!board.SideContainsCard(currentPlayer == player1, casterIndex, out CardData card))
        {
            return;
        }

        if(!currentPlayer.HasSufficientEnergies(card.EnergyType, card.abilityCost))
        {
            return;
        }

        if(TryUseAbility(playerId, card, targetIndex, casterIndex))
        {
            currentPlayer.SubtractEnergy(card.EnergyType, card.abilityCost);
            history.Add(new ActionData(currentPlayer.State.id, ActionType.UnitAbilityUsed, casterIndex, targetIndex));
        }

    }

    private void PlaySpell(long playerId, CardData card, int targetIndex)
    {
        if(!currentPlayer.State.id.Equals(playerId))
        {
            return;
        }

        if(!currentPlayer.State.hand.handCards.Contains(card))
        {
            return;
        }

        if(!currentPlayer.HasSufficientEnergies(card.EnergyType, card.cost))
        {
            return;
        }

        history.Add(new ActionData(currentPlayer.State.id, ActionType.SpellPlayed, 0, targetIndex, card));
        if(TryUseAbility(playerId, card, targetIndex))
        {
            currentPlayer.RemoveHandCard(card);
            currentPlayer.SubtractEnergy(card.EnergyType, card.cost);   
        }
        else
        {
            history.RemoveAt(history.Count - 1);
        }
    }


    public bool TryUseAbility(long playerId, CardData card, int targetIndex, int casterIndex = 0)
    {
        if(unChosenRolls.Count > 0)
        {
            return false;
        }
        var success = false;
        switch(card.abilityName)
        {
            case "Heal":
                success = board.TryHeal(currentPlayer == player1, card, targetIndex, card.abilityStat);
                break;

            case "HealActive":
                success = board.TryHeal(currentPlayer == player1, card, -1, card.abilityStat);
                break;
            case "HealSelf":
                success = board.TryHeal(currentPlayer == player1, card, casterIndex, card.abilityStat);
                break;
            case "searchAny":
                throw new NotImplementedException();
                break;

            case "Damage":
                success = board.TryAttack(currentPlayer == player1, card.abilityStat, -1, out bool alive);
                if (!alive)
                {
                    if (!board.SideContainsHeroCard(currentPlayer != player1))
                    {
                        history.Add(new ActionData(currentPlayer.State.id, ActionType.GameWin, 0, 0));
                    }
                }
                break;

            case "SwitchActive":
                int switchIndex = -1;
                for(int i = 0;i <= 3; i++)
                {
                    if(board.SideContainsCard(currentPlayer != player1, i, out card))
                    {
                        switchIndex = i;
                        break;
                    }
                }
                if (switchIndex != -1)
                {
                     success = board.TrySwitchActiveCard(currentPlayer != player1, switchIndex);
                }
                break;
            case "RedrawHand":
                success = RedrawHand(playerId, card.abilityStat);
                break;

            case "AddUnivarsalEnergy":
                currentPlayer.AddEnergy(EnergyType.universalEnergy, card.abilityStat);
                success = true;
                break;

            case "AddSpellEnergy":
                currentPlayer.AddEnergy(EnergyType.spellEnergy, card.abilityStat);
                success = true;
                break;

            case "AddSummonEnergy":
                currentPlayer.AddEnergy(EnergyType.summonEnergy, card.abilityStat);
                success = true;
                break;

            case "AddSpecialEnergy":
                currentPlayer.AddEnergy(EnergyType.specialEnergy, card.abilityStat);
                success = true;
                break;

            case "Draw":
                for(int i = 0; i < card.abilityStat; i++)
                {
                    currentPlayer.DrawCard();
                    history.Add(new ActionData(currentPlayer.State.id, ActionType.CardDrawn, currentPlayer.State.hand.handCards.Count - 2, 0));
                }
                success = true;
                break;

            default:
                break;
        }

        return success;
    }

    private bool RedrawHand(long playerId, int drawAmount)
    {
        if(player1.State.id.Equals(playerId))
        {
            player2.PutHandIntoDeck();
            player2.ShuffleDeck();
            for(int i = 0; i < drawAmount; i++)
            {
                player2.DrawCard();
                history.Add(new ActionData(player2.State.id, ActionType.CardDrawn, player2.State.hand.handCards.Count - 1, 0));

            }
            return true;
        }
        if(player2.State.id.Equals(playerId))
        {
            player1.PutHandIntoDeck();
            player1.ShuffleDeck();
            for(int i = 0; i < drawAmount; i++)
            {
                player1.DrawCard();
                history.Add(new ActionData(player1.State.id, ActionType.CardDrawn, player1.State.hand.handCards.Count - 1, 0));

            }
            return true;
        }
        return false;
    }

    private bool TrySwitchActiveCard(long playerId, int benchIndex)
    {
        if(unChosenRolls.Count > 0)
        {
            return false;
        }

        if(!currentPlayer.State.id.Equals(playerId))
        {
            return false;
        }

        CardData activeCard = board.GetState(currentPlayer == player1).ownBoardCards.activeCard;

        if(!currentPlayer.HasSufficientEnergies(activeCard.EnergyType, activeCard.retreatCost))
        {
            return false;
        }

        bool success = board.TrySwitchActiveCard(currentPlayer == player1, benchIndex);

        if(success)
        {
            currentPlayer.SubtractEnergy(activeCard.EnergyType, activeCard.retreatCost);
            history.Add(new ActionData(currentPlayer.State.id, ActionType.ActiveCardSwitched, benchIndex, 0));
        }

        return success;
    }

    private void SendStateUpdate()
    {
        var txt = "";
        foreach(ActionData action in history)
        {
            txt += action.ToString();
            txt += "\n";
        }
        txt += "\n";
        GD.Print(txt);
        var msg = new GameStateMessage(player1.State.hand, player2.State.hand.handCards.Count, board.GetState(true), currentPlayer == player1, currentDrafter == player1, player1.State.energy, history, unChosenRolls);
        var msgJson = JsonSerializer.Serialize(msg);
        NetworkManager.Instance.SendMessage(player1.State.id.Value, MessageType.GameStateMsg, msgJson);
        msg = new GameStateMessage(player2.State.hand, player1.State.hand.handCards.Count, board.GetState(false), currentPlayer == player2, currentDrafter == player2, player2.State.energy, history, unChosenRolls);
        msgJson = JsonSerializer.Serialize(msg);
        NetworkManager.Instance.SendMessage(player2.State.id.Value, MessageType.GameStateMsg, msgJson);
    }


    public void HandleMessage(long senderId, int messageTypeInt, string message)
    {
        MessageType messageType = (MessageType)messageTypeInt;

        switch(messageType)
        {
            case MessageType.LoginMsg:
                HandleLoginMessage(senderId, message);
                break;
            case MessageType.ReadyMsg:
                HandleReadyMessage(senderId);
                break;
            case MessageType.ClientGameReadyMsg:
                HandleClientGameReadyMsg(senderId);
                break;
            case MessageType.TurnEndMsg:
                HandleTurnEndMessage(senderId);
                break;
            case MessageType.UnitPlayedMsg:
                HandleUnitPlayedMessage(senderId, message);
                break;
            case MessageType.SpellPlayedMsg:
                HandleSpellPlayedMessage(senderId, message);
                break;
            case MessageType.UnitAttackMsg:
                HandleUnitAttackMessage(senderId, message);
                break;
            case MessageType.UnitAbilityUsedMsg:
                HandleUnitAbilityUsedMessage(senderId, message);
                break;
            case MessageType.ActiveUnitSwitchedMsg:
                HandleActiveUnitSwitchedMessage(senderId, message);
                break;
            case MessageType.EnergyDraftMsg:
                HandleEnergyDraftMessage(senderId, message);
                break;

            //these cases are sent by the Model and should not be handled by it.
            case MessageType.LoginResp:
            case MessageType.ReadyResp:
            case MessageType.GameStartMsg:
            case MessageType.TurnStartMsg:
            case MessageType.GameStateMsg:
                break;
            default:
                GD.PrintErr("The Model recieved a MessageType that it does not know how to handle");
                break;
        }
    }

    private void HandleLoginMessage(long senderId, string message)
    {
        LoginMessage? data = JsonSerializer.Deserialize<LoginMessage>(message);

        if(data == null)
        {
            return;
        }
        PlayerId playerId = new(senderId);
        var success = ConnectPlayer(data.Name, playerId, data.LoginId);

        if(success)
        {
            LoginResponse resp = new LoginResponse(playerId);
            var respStr = JsonSerializer.Serialize(resp);
            NetworkManager.Instance.SendMessage(senderId, MessageType.LoginResp, respStr);
        }
    }


    private void HandleReadyMessage(long senderId)
    {
        var success = PlayerReady(senderId);

        var msg = new ReadyResponse(success);
        var msgJson = JsonSerializer.Serialize(msg);

        NetworkManager.Instance.SendMessage(senderId, MessageType.ReadyResp, msgJson);
    }

    private void HandleClientGameReadyMsg(long senderId)
    {

        HandData handData = new HandData();
        if(player1.State.id.Equals(senderId))
        {
            handData = player1.State.hand;
            var msg = new GameStateMessage(handData, player2.State.hand.handCards.Count, board.GetState(true), currentPlayer == player1, currentDrafter == player1, player1.State.energy, history, unChosenRolls);
            var msgJson = JsonSerializer.Serialize(msg);

            NetworkManager.Instance.SendMessage(senderId, MessageType.GameStateMsg, msgJson);
        }
        else if(player2.State.id.Equals(senderId))
        {
            handData = player2.State.hand;
            var msg = new GameStateMessage(handData, player1.State.hand.handCards.Count, board.GetState(false), currentPlayer == player2, currentDrafter == player2, player2.State.energy, history, unChosenRolls);
            var msgJson = JsonSerializer.Serialize(msg);

            NetworkManager.Instance.SendMessage(senderId, MessageType.GameStateMsg, msgJson);
        }


    }
    private void HandleTurnEndMessage(long senderId)
    {
        if(currentPlayer.State.id.Equals(senderId))
        {
            StartNextTurn();
            SendStateUpdate();
        }
    }

    private void HandleUnitPlayedMessage(long senderId, string message)
    {
        UnitPlayedMessage? data = JsonSerializer.Deserialize<UnitPlayedMessage>(message);

        if(data == null)
        {
            return;
        }

        PlayUnitToBoard(senderId, data.Unit, data.BenchIndex);
        SendStateUpdate();

    }

    private void HandleSpellPlayedMessage(long senderId, string message)
    {
        SpellPlayedMessage? data = JsonSerializer.Deserialize<SpellPlayedMessage>(message);

        if(data == null)
        {
            return;
        }

        PlaySpell(senderId, data.Spell, data.TargetIndex);
        SendStateUpdate();
    }

    private void HandleUnitAttackMessage(long senderId, string message)
    {
        UnitAttackMessage? data = JsonSerializer.Deserialize<UnitAttackMessage>(message);

        if(data == null)
        {
            return;
        }

        UnitAttacks(senderId, data.BenchIndex, -1);
        SendStateUpdate();
    }
    private void HandleUnitAbilityUsedMessage(long senderId, string message)
    {
        UnitAbilityUsedMessage? data = JsonSerializer.Deserialize<UnitAbilityUsedMessage>(message);

        if(data == null)
        {
            return;
        }

        UseUnitAbility(senderId, data.BenchIndex, data.TargetIndex);
        SendStateUpdate();
    }

    private void HandleActiveUnitSwitchedMessage(long senderId, string message)
    {
        ActiveUnitSwitchedMessage? data = JsonSerializer.Deserialize<ActiveUnitSwitchedMessage>(message);

        if(data == null)
        {
            return;
        }

        TrySwitchActiveCard(senderId, data.BenchIndex);
        SendStateUpdate();
    }

    private void HandleEnergyDraftMessage(long senderId, string message)
    {
        EnergyDraftMessage? data = JsonSerializer.Deserialize<EnergyDraftMessage>(message);

        if(data == null)
        {
            return;
        }

        TryDraftEnergy(senderId, data.DraftIndex);
        SendStateUpdate();
    }

}
