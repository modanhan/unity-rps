using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Connected : MainApplicationReference
{
    MainApplication application;
    bool joining = false;
    public override void Init(MainApplication application)
    {
        this.application = application;
    }
    void OnGUI()
    {
        int y = 10;
        GUI.Label(new Rect(10, y += 25, 200, 20), "Connected!");
        GUI.enabled = !joining;
        if (GUI.Button(new Rect(10, y += 25, 200, 20), "Quick play"))
        {
            PhotonNetwork.JoinRandomRoom();
            joining = true;
        }
        GUI.enabled = true;
    }
}
