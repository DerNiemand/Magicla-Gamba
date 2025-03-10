using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class MainMenu : Control, INetworkReciever
{
    
    LineEdit ipField;

    NetworkManager networkManager;

    [Export]
    private PackedScene gameScene;

    public override void _Ready()
    {
        ipField = FindChild("IPField") as LineEdit;
        networkManager = GetTree().GetRoot().GetNode<NetworkManager>("NetworkManager");
        networkManager.OnPlayerIdSet += DisplayPlayerId;
    }

    public void _On_Join_Button_Pressed()
    {
        if (ipField == null)
        {
            return;
        }

        if (ipField.Text.Length == 0)
        {
            return;
        }

        var file = FileAccess.Open("res://Resources/default_deck.txt", FileAccess.ModeFlags.Read);
        var jsontext = file.GetAsText();
        file.Close();
        List<CardData> deck = JsonSerializer.Deserialize<List<CardData>>(jsontext);
        

        networkManager.ConnectGame(ipField.Text,"steve", deck,new LoginId(new Guid()));
    }

    public void _On_Host_Button_Pressed()
    {
        networkManager.CreateHost();
    }

    public void _On_Ready_Button_Pressed()
    {
        networkManager.SendMessage(NetworkManager.ServerID, MessageType.ReadyMsg);
    }

    private void DisplayPlayerId()
    {
        //var GameIdLabel = FindChild("GameId") as Label;
        //if (GameIdLabel != null)
        //{
        //    GameIdLabel.Text = networkManager.PlayerId.Value.ToString();
        //}
    }

    public void HandleMessage(long SenderId, int messageTypeInt, string message)
    {
        MessageType messageType = (MessageType)messageTypeInt;
        switch(messageType)
        {
            case MessageType.LoginResp:

                HandleLoginResponse(message);

                break;

            case MessageType.ReadyResp:

                HandleReadyResponse(message);
                break;

            case MessageType.GameStartMsg:
                HandleGameStartMessage(message);
                break;

            //these messages should not be handled by the main menu
            case MessageType.LoginMsg:
            case MessageType.ReadyMsg:
            case MessageType.TurnStartMsg:
            case MessageType.TurnEndMsg:
            case MessageType.UnitPlayedMsg:
            case MessageType.SpellPlayedMsg:
            case MessageType.UnitAttackMsg:
            case MessageType.ActiveUnitSwitchedMsg:
            case MessageType.ClientGameReadyMsg:
            case MessageType.GameStateMsg:
            case MessageType.EnergyDraftMsg:
            case MessageType.UnitAbilityUsedMsg:
                break;
            default:
                GD.PrintErr("The MainMenu recieved a MessageType that it does not know how to handle");
                break;
        }
    }

    private void HandleLoginResponse(string message)
    {
        LoginResponse data = JsonSerializer.Deserialize<LoginResponse>(message);
        if (data == null)
        {
            return;
        }

        var readyButton = FindChild("ReadyButton") as TextureButton;
        readyButton.Visible = true;

        networkManager.PlayerId = data.PlayerId;
        DisplayPlayerId();
    }

    private void HandleReadyResponse(string message)
    {
        ReadyResponse data = JsonSerializer.Deserialize<ReadyResponse>(message);

        if (data == null)
        {
            return;
        }

        var readyButton = FindChild("ReadyButton") as TextureButton;
        readyButton.Visible = !data.Success;
    }

    private void HandleGameStartMessage(string message)
    {
        GetTree().ChangeSceneToPacked(gameScene);
    }

    private void _OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
