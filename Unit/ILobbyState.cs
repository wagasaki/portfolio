using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILobbyState
{
    void OnEnter(LobbyUICharacter unit);
    void OnUpdate();
    void OnExit();
}
