using System;
using UnityEngine;

[GameState]
public class LaunchState : BaseState
{
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        GameStateCtrl.GetInstance().GotoState("VersionUpdateState");
    }
}
