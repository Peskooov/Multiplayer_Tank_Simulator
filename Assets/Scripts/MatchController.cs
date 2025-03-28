using System;
using Mirror;
using UnityEngine.Events;

public interface IMatchCondition
{
    bool IsTriggered { get; }

    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}

public class MatchController : NetworkBehaviour
{
    public UnityAction MatchStart;
    public UnityAction MatchEnd;
    
    public UnityAction SvMatchStart;
    public UnityAction SvMatchEnd;
    
    [SyncVar] private bool isMatchActive; 
    public bool IsMatchActive => isMatchActive;

    public int WinTeamID = -1;
    
    private IMatchCondition[] matchConditions;

    private void Awake()
    {
        matchConditions = GetComponentsInChildren<IMatchCondition>();
    }

    private void Update()
    {
        if (isServer)
        {
            if (isMatchActive)
            {
                foreach (IMatchCondition matchCondition in matchConditions)
                {
                    if (matchCondition.IsTriggered)
                    {
                        SvEndMatch(); // Завершение матча
                        break;
                    }
                }
            }
        }
    }

    [Server]
    public void SvRestartMatch()
    {
        if(isMatchActive) return;
        
        isMatchActive = true;
        
        foreach (var p in FindObjectsOfType<Player>())
        {
            if (p.ActiveVehicle != null)
            {
                NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                Destroy(p.ActiveVehicle.gameObject);
                p.ActiveVehicle = null;
            }
        }

        foreach (var p in FindObjectsOfType<Player>())
        {
            p.SvSpawnClientVehicle();
        }

        foreach (IMatchCondition matchCondition in matchConditions)
        {
            matchCondition.OnServerMatchStart(this);
        }
        
        SvMatchStart?.Invoke();

        RpcMatchStart();
    }
    
    [Server]
    public void SvEndMatch()
    {
        foreach (IMatchCondition matchCondition in matchConditions)
        {
            matchCondition.OnServerMatchEnd(this);

            if (matchCondition is ConditionTeamDeathMatch)
            {
                WinTeamID = (matchCondition as ConditionTeamDeathMatch).WinTeamID;
            }

            if (matchCondition is ConditionCaptureBase)
            {
                if((matchCondition as ConditionCaptureBase).RedBaseCaptureLevel >= 100)
                    WinTeamID = TeamSide.TeamBlue;
                
                if((matchCondition as ConditionCaptureBase).BlueBaseCaptureLevel >= 100)
                    WinTeamID = TeamSide.TeamRed;
            }
        }
        
        isMatchActive = false;
        
        SvMatchEnd?.Invoke();
        
        RpcMatchEnd(WinTeamID);
    }

    [ClientRpc]
    public void RpcMatchStart()
    {
        MatchStart?.Invoke();
    }
    
    [ClientRpc]
    public void RpcMatchEnd(int winTeamID)
    {
        WinTeamID = winTeamID;
        MatchEnd?.Invoke();
    }
}
