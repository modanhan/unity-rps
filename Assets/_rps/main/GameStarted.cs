using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameStarted : MainApplicationReference, IOnEventCallback
{
    const int kMaxRounds = 5;
    MainApplication application;

    Player self;
    Player opponent;

    class PlayerState
    {
        public int health = 10;
        public CardType played = CardType.None;
    }
    PlayerState selfState, opponentState;
    int currentRound = 1;
    CardResult roundResult = CardResult.None;

    public override void Init(MainApplication application)
    {
        this.application = application;
    }

    void Awake()
    {
        self = PhotonNetwork.LocalPlayer;
        opponent = PhotonNetwork.PlayerListOthers[0];
        selfState = new PlayerState();
        opponentState = new PlayerState();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void newRound()
    {
        currentRound++;
        selfState.played = CardType.None;
        opponentState.played = CardType.None;
    }

    void Play(CardType type)
    {
        selfState.played = type;
        object[] data = { type };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventCode.kPlayedCard, data, raiseEventOptions, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode.kPlayedCard)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
            object[] data = (object[])photonEvent.CustomData;
            CardType type = (CardType)data[0];
            if (player == self)
            {
                selfState.played = type;
            }
            else if (player == opponent)
            {
                opponentState.played = type;
            }
            else
            {
                Debug.LogError("Unknown player event raised in room");
                return;
            }
            UpdatePlayed();
        }
    }

    void UpdatePlayed()
    {
        if (selfState.played != CardType.None && opponentState.played != CardType.None)
        {
            var result = Card.Compare(selfState.played, opponentState.played);
            if (result == CardResult.Win)
            {
                opponentState.health--;
            }
            else if (result == CardResult.Lose)
            {
                selfState.health--;
            }
            roundResult = result;
            newRound();
        }
    }

    void OnGUI()
    {
        int y = 10;
        GUI.Label(new Rect(10, y += 25, 200, 20), self.NickName);
        GUI.Label(new Rect(10, y += 25, 200, 20), "vs Worthy opponent: ");
        GUI.Label(new Rect(10, y += 25, 200, 20), opponent.NickName);

        GUI.Label(new Rect(10, y += 50, 200, 20), "Round " + currentRound);
        GUI.Label(new Rect(10, y += 25, 200, 20), self.NickName + "health: " + selfState.health);
        GUI.Label(new Rect(10, y += 25, 200, 20), opponent.NickName + "Health: " + opponentState.health);

        if (selfState.played != CardType.None)
        {
            GUI.Label(new Rect(10, y += 50, 200, 20), "Played " + selfState.played);
        }
        else
        {
            GUI.Label(new Rect(10, y += 50, 200, 20), "");
        }

        switch (roundResult)
        {
            case CardResult.Lose: GUI.Label(new Rect(10, y += 25, 200, 20), "Round lost."); break;
            case CardResult.Tie: GUI.Label(new Rect(10, y += 25, 200, 20), "Round tied."); break;
            case CardResult.Win: GUI.Label(new Rect(10, y += 25, 200, 20), "Round won!"); break;
            default: break;
        }

        y += 25;
        {
            GUI.enabled = selfState.played == CardType.None;
            int x = 10;
            if (GUI.Button(new Rect(10, y, 100, 100), "Rock"))
            {
                Play(CardType.Rock);
            }
            if (GUI.Button(new Rect(x += 110, y, 100, 100), "Paper"))
            {
                Play(CardType.Paper);
            }
            if (GUI.Button(new Rect(x += 110, y, 100, 100), "Scissors"))
            {
                Play(CardType.Scissors);
            }
            GUI.enabled = true;
        }
    }
}