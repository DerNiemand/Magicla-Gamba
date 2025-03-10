using System.Collections.Generic;

public enum MessageType
{
    LoginMsg,
    LoginResp,
    ReadyMsg,
    ReadyResp,
    GameStartMsg,
    ClientGameReadyMsg,
    GameStateMsg,
    EnergyDraftMsg,
    TurnStartMsg,
    TurnEndMsg,
    UnitPlayedMsg,
    SpellPlayedMsg,
    UnitAttackMsg,
    UnitAbilityUsedMsg,
    ActiveUnitSwitchedMsg
}

public class LoginMessage
{
    #region variables

    public string Name
    {
        get;

    }

    public List<CardData> DeckCards
    {
        get;
    }

    public LoginId LoginId
    {
        get;
    }

    #endregion
    public LoginMessage(string name, List<CardData> deckCards, LoginId loginId)
    {
        Name = name;
        DeckCards = deckCards;
        LoginId = loginId;
    }
}
public class LoginResponse
{
    #region varibales
    public PlayerId PlayerId
    {
        get;
    }

    #endregion

    public LoginResponse(PlayerId playerId)
    {
        PlayerId = playerId;
    }
}
public class ReadyMessage
{
    #region variables

    #endregion
    public ReadyMessage()
    {

    }
}

public class ReadyResponse
{
    #region variables
    public bool Success
    {
        get;
    }
    #endregion
    public ReadyResponse(bool success)
    {
        Success = success;
    }
}

public class GameStartMessage
{
    #region variables

    #endregion

    public GameStartMessage()
    {

    }
}

public class ClientGameReadyMessage
{
    #region varibales

    #endregion

    public ClientGameReadyMessage()
    {

    }
}

public class GameStateMessage
{
    #region varibales
    public HandData HandData
    {
        get;
    }

    public int EnemyHandCardCount
    {
        get;
    }

    public BoardData BoardData
    {
        get;
    }

    public bool PlayersTurn
    {
        get;
    }

    public bool PlayersDraftTurn
    {
        get;
    }

    public EnergyData Energy
    {
        get;
    }

    public List<ActionData> History
    {
        get;
    }

    public List<Dictionary<EnergyType, int>> UnChosenRolls
    {
        get;
    }

    #endregion

    public GameStateMessage(HandData handData, int enemyHandCardCount, BoardData boardData, bool playersTurn, bool playersDraftTurn, EnergyData energy, List<ActionData> history, List<Dictionary<EnergyType, int>> unChosenRolls)
    {
        HandData = handData;
        EnemyHandCardCount = enemyHandCardCount;
        BoardData = boardData;
        PlayersTurn = playersTurn;
        PlayersDraftTurn = playersDraftTurn;
        Energy = energy;
        History = history;
        UnChosenRolls = unChosenRolls;
    }
}

public class EnergyDraftMessage
{
    public int DraftIndex
    {
        get;
    }
    public EnergyDraftMessage(int draftIndex) 
    {
        DraftIndex = draftIndex;
    }
}

public class TurnStartMessage
{
    #region varibales
    public CardData DrawnCard
    {
        get;
    }

    #endregion

    public TurnStartMessage(CardData drawnCard)
    {
        DrawnCard = drawnCard;
    }
}
public class TurnEndMessage
{
    public TurnEndMessage()
    {

    }
}
public class UnitPlayedMessage
{
    #region varibales
    public CardData Unit
    {
        get;
    }

    public int BenchIndex
    {
        get;
    }

    #endregion

    public UnitPlayedMessage(CardData unit, int benchIndex)
    {
        Unit = unit;
        BenchIndex = benchIndex;
    }
}

public class SpellPlayedMessage
{
    #region varibales
    public CardData Spell
    {
        get;
    }

    public int TargetIndex
    {
        get;
    }

    #endregion

    public SpellPlayedMessage(CardData spell, int targetIndex)
    {
        Spell = spell;
        TargetIndex = targetIndex;
    }
}

public class UnitAttackMessage
{
    #region varibales
    public int BenchIndex
    {
        get;
    }
    #endregion

    public UnitAttackMessage(int benchIndex)
    {
        BenchIndex= benchIndex;
    }
}

public class UnitAbilityUsedMessage
{
    #region variables
    public int BenchIndex
    {
        get;
    }

    public int TargetIndex
    {
        get;
    }

    public string AbilityName
    {
        get;
    }
    #endregion

    public UnitAbilityUsedMessage(int benchIndex, int targetIndex, string abilityName)
    {
        BenchIndex =  benchIndex;
        TargetIndex = targetIndex;
        AbilityName = abilityName;
    }
}

public class ActiveUnitSwitchedMessage
{
    #region variables
    public int BenchIndex
    {
        get;
    }
    #endregion

    public ActiveUnitSwitchedMessage(int benchIndex)
    {
        BenchIndex = benchIndex;
    }
}
