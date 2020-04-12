using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EnteredRoom : MainApplicationReference
{
    MainApplication application;

    List<string> playerNames;
    public override void Init(MainApplication application)
    {
        this.application = application;
        foreach (var v in PhotonNetwork.PlayerList)
        {
            playerNames.Add(v.NickName);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerNames.Add(newPlayer.NickName);
    }
    void Awake()
    {
        playerNames = new List<string>();
    }
    void OnGUI()
    {
        int y = 10;
        if (playerNames.Count != MainApplication.kMaxPlayersPerRoom)
        {
            GUI.Label(new Rect(10, y += 25, 200, 20), "Finding opponent...");
        }
        else
        {
            GUI.Label(new Rect(10, y += 25, 200, 20), "Worthy opponent: ");
            foreach (var v in playerNames)
            {
                if (v != PhotonNetwork.NickName)
                {
                    GUI.Label(new Rect(10, y += 25, 200, 20), v);
                }
            }
        }
    }
}
