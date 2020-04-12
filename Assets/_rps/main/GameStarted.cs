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
    int selfPlayed = -1;
    int opponentPlayed = -1;

    int currentRound = 1;
    CardResult roundResult = CardResult.None;

    public override void Init(MainApplication application)
    {
        this.application = application;
    }

    void Awake()
    {
        CardDatabase.Init();
        self = PhotonNetwork.LocalPlayer;
        opponent = PhotonNetwork.PlayerListOthers[0];
        selfState = new RPSPlayerState();
        opponentState = new RPSPlayerState();
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
        selfPlayed = -1;
        opponentPlayed = -1;
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
        if (photonEvent.Code == EventCode.kPlayedCard)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
            object[] data = (object[])photonEvent.CustomData;
            int idx = (int)data[0];
            int ID = selfState.hand[idx];
            if (player == self)
            {
                selfPlayed = ID;
            }
            else if (player == opponent)
            {
                opponentPlayed = ID;
            }
            else
            {
                Debug.LogError("Unknown player event raised in room");
                return;
            }
            UpdatePlayed();
            selfState.hand.RemoveAt(idx);
            selfState.Draw();
        }
    }

    void UpdatePlayed()
    {
        if (selfPlayed != -1 && opponentPlayed != -1)
        {
            Card selfCard = CardDatabase.GetCard(selfPlayed);
            Card opponentCard = CardDatabase.GetCard(opponentPlayed);
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
        GUI.Label(new Rect(10, y += 25, 200, 20), opponent.NickName + " health: " + opponentState.health);

        if (selfPlayed != -1)
        {
            GUI.Label(new Rect(10, y += 50, 200, 20), "Played " + CardDatabase.GetCard(selfPlayed).Name());
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
            GUI.enabled = selfPlayed == -1;
            int x = -90;
            for (int i = 0; i < selfState.hand.Count; ++i)
            {
                var ID = selfState.hand[i];
                var card = CardDatabase.GetCard(ID);
                if (GUI.Button(new Rect(x += 100, y, 100, 100), card.Name() + "\n\n" + card.Desc()))
                {
                    Play(i);
                }
            }
            GUI.enabled = true;
        }
    }
}