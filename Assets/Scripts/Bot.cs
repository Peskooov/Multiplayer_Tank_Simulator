using System;
using UnityEngine;

public class Bot : MatchMember
{
    [SerializeField] private Vehicle vehicle;
    
    public override void OnStartServer()
    {
        base.OnStartServer();

        teamID = MatchController.GetNextTeam();
        nickname = "bot_" + GetRandomName();
        data = new MatchMemberData((int)netId, nickname, teamID, netIdentity);
        
        transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(teamID);
        
        ActiveVehicle = vehicle;
        ActiveVehicle.TeamID = teamID;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = nickname;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
        ActiveVehicle = vehicle;
        ActiveVehicle.TeamID = teamID;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = nickname;
        
        MatchMemberList.Instance.SvRemoveMember(data);
    }

    private void Start()
    {
        if (isServer)
        {
            MatchMemberList.Instance.SvAddMember(data);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        
    }

    private string GetRandomName()
    {
        string[] names =
        {
            "Overlord",
            "DeathBringer",
            "KnightX",
            "Titan",
            "Warrior",
            "Storm",
            "Phantom",
            "Blaze",
            "Viper",
            "Nexus",
            "Apex",
            "Zenith",
            "Shadow",
            "Specter",
            "Inferno",
            "Venom",
            "Hunter",
            "King",
            "Raptor",
            "Drakon",
            "Warlord",
            "Vortex"
        };
        
        return names[UnityEngine.Random.Range(0, names.Length)];
    }
}
