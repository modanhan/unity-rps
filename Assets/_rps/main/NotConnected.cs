﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NotConnected : MainApplicationReference
{
    string gameVersion = "1";
    MainApplication application;
    string playerName = "";

    bool connecting = false;
    public override void Init(MainApplication application)
    {
        this.application = application;
    }
    void OnGUI()
    {
        int y = 10;
        GUI.Label(new Rect(10, y += 25, 200, 20), "RPS!");
        GUI.Label(new Rect(10, y += 25, 200, 20), "User name:");
        playerName = GUI.TextField(new Rect(10, y += 25, 200, 20), playerName, 25);
        GUI.enabled = !connecting;
        if (GUI.Button(new Rect(10, y += 25, 200, 20), "Login"))
        {
            Connect();
            connecting = true;
        }
        if (connecting)
        {
            GUI.Label(new Rect(10, y += 25, 200, 20), "Connecting...");
        }
        GUI.enabled = true;
    }

    void Connect()
    {
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public override void OnConnectedToMaster()
    {
        application.UpdateState(MainApplication.State.kConnected);
    }
}
