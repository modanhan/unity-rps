using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameStarted : MainApplicationReference
{
    MainApplication application;

    Player self;
    Player opponent;
    public override void Init(MainApplication application)
    {
        this.application = application;
    }

    void Awake()
    {
        self = PhotonNetwork.LocalPlayer;
        // opponent = PhotonNetwork.PlayerListOthers[0];
    }

    void OnGUI()
    {
        int y = 10;
        GUI.Label(new Rect(10, y += 25, 200, 20), self.NickName);
        GUI.Label(new Rect(10, y += 25, 200, 20), "vs Worthy opponent: ");
        // GUI.Label(new Rect(10, y += 25, 200, 20), opponent.NickName);

        y += 25;
        {
            int x = 10;
            if (GUI.Button(new Rect(10, y, 100, 100), "Rock"))
            {

            }
            if (GUI.Button(new Rect(x += 110, y, 100, 100), "Paper"))
            {

            }
            if (GUI.Button(new Rect(x += 110, y, 100, 100), "Scissors"))
            {

            }
        }
    }
}