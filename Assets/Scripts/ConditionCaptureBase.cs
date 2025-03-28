using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ConditionCaptureBase : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private TeamBase redBase;
    [SerializeField] private TeamBase blueBase;
    
    [SyncVar]
    private float redBaseCaptureLevel;
    public float RedBaseCaptureLevel => redBaseCaptureLevel;
    
    [SyncVar]
    private float blueBaseCaptureLevel;
    public float BlueBaseCaptureLevel => blueBaseCaptureLevel;

    private bool isTriggered; 
    public bool IsTriggered => isTriggered;
    public void OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    public void OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    private void Start()
    {
        enabled = false;
    }

    private void Update()
    {
        if (isServer)
        {
            redBaseCaptureLevel = redBase.CaptureLevel;
            blueBaseCaptureLevel = blueBase.CaptureLevel;

            if (redBaseCaptureLevel >= 100 || blueBaseCaptureLevel >= 100)
            {
                isTriggered = true;
            }
        }
    }
    
    private void Reset()
    {
        redBase.Reset();
        blueBase.Reset();
        
        redBaseCaptureLevel = 0;
        blueBaseCaptureLevel = 0;
        
        isTriggered = false;
        enabled = true;
    }
}
