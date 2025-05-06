using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public interface IMatchCondition
{
    bool IsTriggered { get; }

    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}

public class MatchController : NetworkBehaviour
{
    private static int TeamIDCounter;

    public static int GetNextTeam()
    {
        return TeamIDCounter++ % 2;
    }

    public static void ResetTeamIDCounter()
    {
        TeamIDCounter = 1;
    }

    public event UnityAction MatchStart;
    public event UnityAction MatchEnd;
    
    public event UnityAction SvMatchStart;
    public event UnityAction SvMatchEnd;
    
    [SerializeField] private MatchMemberSpawner spawner;
    [SerializeField] private float delayAfterSpawnBeforeStartMatch = 0.5f;
    
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
        if (isMatchActive) return;

        isMatchActive = true;

        spawner.SvRespawnVehiclesAllMembers();

        StartCoroutine(StartEventMatchWithDelay(delayAfterSpawnBeforeStartMatch));
    }

    private IEnumerator StartEventMatchWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
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
