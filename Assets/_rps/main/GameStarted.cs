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
    RPSPlayerState selfState;
    RPSPlayerState opponentState;
    int selfIdx = -1;
    int opponentIdx = -1;

    int currentRound = 1;
    CardResult roundResult = CardResult.None;

    public override void Init(MainApplication application)
    {
        this.application = application;
        selfState = new RPSPlayerState();

        for (int i = 0; i < selfState.deck.Count; ++i) Debug.Log(selfState.deck[i]);
        SyncInitCards();
    }

    void Awake()
    {
        CardDatabase.Init();
        self = PhotonNetwork.LocalPlayer;
        opponent = PhotonNetwork.PlayerListOthers[0];
    }

    void SyncInitCards()
    {
        object[] data = new object[selfState.deck.Count];
        for (int i = 0; i < selfState.deck.Count; ++i)
        {
            data[i] = selfState.deck[i];
        }
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventCode.kSyncInitCards, data, raiseEventOptions, sendOptions);
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
        selfIdx = -1;
        opponentIdx = -1;
    }

    void Play(int idx)
    {
        object[] data = { idx };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventCode.kPlayedCard, data, raiseEventOptions, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode.kSyncInitCards)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
            if (player == self) { Debug.LogError("Receiving command not sent to self."); return; }
            object[] data = (object[])photonEvent.CustomData;
            opponentState = new RPSPlayerState();
            for (int i = 0; i < opponentState.deck.Count; ++i)
            {
                opponentState.deck[i] = (int)data[i]; Debug.Log(opponentState.deck[i]);
            }
            for (int i = 0; i < RPSPlayerState.kDefaultHandSize; ++i)
            {
                selfState.Draw();
            }
            for (int i = 0; i < RPSPlayerState.kDefaultHandSize; ++i)
            {
                opponentState.Draw();
            }
        }
        if (photonEvent.Code == EventCode.kPlayedCard)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
            object[] data = (object[])photonEvent.CustomData;
            int idx = (int)data[0];
            if (player == self)
            {
                selfIdx = idx;
                int selfID = selfState.hand[selfIdx];
                Debug.Log("self played " + idx + "-th card = " + selfState.hand[selfIdx] + " = " + CardDatabase.GetCard(selfID).Name());
            }
            else if (player == opponent)
            {
                opponentIdx = idx;
                int opponentId = opponentState.hand[opponentIdx];
                Debug.Log("opponent played " + idx + "-th card = " + opponentState.hand[opponentIdx] + " = " + CardDatabase.GetCard(opponentId).Name());
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
        if (selfIdx != -1 && opponentIdx != -1)
        {
            int selfID = selfState.hand[selfIdx];
            int opponentID = opponentState.hand[opponentIdx];
            Card selfCard = CardDatabase.GetCard(selfID);
            Card opponentCard = CardDatabase.GetCard(opponentID);
            var result = Card.Compare(selfCard.type, opponentCard.type);
            if (result == CardResult.Win)
            {
                selfCard.Win(selfState, opponentState);
                opponentCard.Lose(opponentState, selfState);
            }
            else if (result == CardResult.Lose)
            {
                selfCard.Lose(selfState, opponentState);
                opponentCard.Win(opponentState, selfState);
            }
            else if (result == CardResult.Tie)
            {
                selfCard.Tie(selfState, opponentState);
                opponentCard.Tie(opponentState, selfState);
            }
            selfState.hand.RemoveAt(selfIdx);
            selfState.Draw();
            opponentState.hand.RemoveAt(opponentIdx);
            opponentState.Draw();
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
        GUI.Label(new Rect(10, y += 25, 200, 20), self.NickName + " health: " + selfState.health);
        if (opponentState != null)
        {
            GUI.Label(new Rect(10, y += 25, 200, 20), opponent.NickName + " health: " + opponentState.health);
        }

        if (selfIdx != -1)
        {
            GUI.Label(new Rect(10, y += 50, 200, 20), "Played " + CardDatabase.GetCard(selfIdx).Name());
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
            GUI.skin.button.wordWrap = true;
            GUI.enabled = (selfIdx == -1) && (opponentState != null);
            int x = -90;
            for (int i = 0; i < selfState.hand.Count; ++i)
            {
                var ID = selfState.hand[i];
                var card = CardDatabase.GetCard(ID);
                if (GUI.Button(new Rect(x += 100, y, 100, 100), card.Name() + card.ID + "\n\n" + card.Desc()))
                {
                    Play(i);
                }
            }
            GUI.enabled = true;
        }
    }
}