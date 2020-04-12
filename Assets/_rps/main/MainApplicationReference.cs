using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class MainApplicationReference : MonoBehaviourPunCallbacks
{
    public abstract void Init(MainApplication application);
}
