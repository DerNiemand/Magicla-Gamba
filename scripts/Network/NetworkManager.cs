using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class NetworkManager : Node
{
    private LoginId loginId;
    public LoginId LoginId
    {
        get
        {
            return loginId;
        }
        private set
        {
            if (loginId == null)
            {
                loginId = value;
            }
        }
    }

    public static NetworkManager Instance { get; private set; }

    [Signal]
    public delegate void OnPlayerIdSetEventHandler();

    private const int Port = 34568;
    ENetMultiplayerPeer peer;
    public const long ServerID = 1;

    GameModel model;

    PlayerId playerId;

    public PlayerId PlayerId
    {
        get
        {
            return playerId;
        }
        set
        {
            playerId = value;
        }
    }

    public override void _Ready()
    {
#if !DEBUG
        loginId = GetGuid();
#else
        var guid = Guid.NewGuid();
        loginId = new LoginId(guid);
#endif

        peer = new ENetMultiplayerPeer();
        Instance = this;
#if GODOT_SERVER
        CreateDedicatedServer();
#endif

    }

    private LoginId GetGuid()
    {
        if (FileAccess.FileExists("user://user_data.dat"))
        {
            var file = FileAccess.Open("user://user_data.dat", FileAccess.ModeFlags.ReadWrite);
            var firstLine = file.GetLine();
            if (!Guid.TryParse(firstLine, out Guid guid))
            {
                guid = Guid.NewGuid();
                var text = guid.ToString() + "\n" + file.GetAsText();
                file.Resize(0);
                file.StoreString(text);
                file.Flush();
            }
            return new LoginId(guid);
        }
        else
        {
            var file = FileAccess.Open("user://user_data.dat", FileAccess.ModeFlags.Write);
            var guid = Guid.NewGuid();
            file.StoreString(guid.ToString() + "\n");
            file.Flush();
            return new LoginId(guid);
        }
    }

#if !GODOT_SERVER
    public bool CreateHost()
    {
        if (Error.Ok != peer.CreateServer(Port))
        {
            return false;
        }
        Multiplayer.MultiplayerPeer = peer;

        var file = FileAccess.Open("res://Resources/default_deck.txt", FileAccess.ModeFlags.Read);
        var jsontext = file.GetAsText();
        List<CardData> deck = JsonSerializer.Deserialize<List<CardData>>(jsontext);

        model = new GameModel();
        var msg = new LoginMessage("dave", deck, loginId);
        var msgJson = JsonSerializer.Serialize(msg);

        SendMessage(ServerID,(int)MessageType.LoginMsg, msgJson);

        return true;
    }

    public bool ConnectGame(string ConnectionIp, string name, List<CardData> deck, LoginId id)
    {
        if (Godot.Error.Ok != peer.CreateClient(ConnectionIp, Port))
        {
            return false;
        }

        Multiplayer.MultiplayerPeer = peer;

        Multiplayer.ConnectedToServer += () => JoinGame(name, deck, id);
        return true;
    }

    private void JoinGame(string name, List<CardData> Deck, LoginId id)
    {
        var msg = new LoginMessage(name, Deck, id);
        var msgJson = JsonSerializer.Serialize(msg);

        Error test = SendMessage(ServerID, (int)MessageType.LoginMsg, msgJson);
    }
#endif


    public Error SendMessage(long recieverId,MessageType messageType, string msg = "")
    {
        var result = RpcId(recieverId,"RecieveMessage" ,(int)messageType, msg);
        
        return result;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RecieveMessage(int messageType, string message)
    {
         var senderId = Multiplayer.GetRemoteSenderId();
        
        if (model != null)
        {
            model.HandleMessage(senderId, messageType, message);
        }

        if (GetTree().CurrentScene is INetworkReciever scene)
        {
            scene.HandleMessage(senderId, messageType, message);
        }
        
    }



#if GODOT_SERVER
    private void CreateDedicatedServer()
    {
        while (Error.Ok != peer.CreateServer(Port))
        {
        }
        Multiplayer.MultiplayerPeer = peer;

        model = new GameModel();
    }
#endif
}
