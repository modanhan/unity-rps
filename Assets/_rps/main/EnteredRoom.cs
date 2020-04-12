using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EnteredRoom : MainApplicationReference
{
    MainApplication application;

    List<Player> players;
    public override void Init(MainApplication application)
    {
        this.application = application;
        foreach (var v in PhotonNetwork.PlayerList)
        {
            UpdateNewPlayer(v);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateNewPlayer(newPlayer);
    }

    void UpdateNewPlayer(Player newPlayer)
    {
        players.Add(newPlayer);
        if (players.Count == MainApplication.kMaxPlayersPerRoom)
        {
            application.UpdateState(MainApplication.State.kGameStarted);
        }
    }

    void Awake()
    {
        players = new List<Player>();
    }
    void OnGUI()
    {
        int y = 10;
        GUI.Label(new Rect(10, y += 25, 200, 20), "Finding opponent...");
    }
}
