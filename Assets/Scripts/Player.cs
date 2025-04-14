using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerData
{
    public int ID;
    public string Nickname;
    public int TeamID; 

    public PlayerData(int id, string nickname, int teamID)
    {
        ID = id;
        Nickname = nickname;
        TeamID = teamID;
    }
}

public static class PlayerDataWriteRead
{
    public static void WriteUserData(this NetworkWriter writer, PlayerData data)
    {
        writer.WriteInt(data.ID);
        writer.WriteString(data.Nickname);
        writer.WriteInt(data.TeamID);
    }
        
    public static PlayerData ReadUserData(this NetworkReader reader)
    {
        return new PlayerData(reader.ReadInt(), reader.ReadString(), reader.ReadInt());
    }
}

public class Player : NetworkBehaviour
{
    public static int TeamIDCounter;
    
    public event UnityAction<Vehicle> VehicleSpawned;
    public event UnityAction<ProjectileHitResult> ProjectileHit;
    public static UnityAction<int,int> ChangeFrags;
    
    public static Player Local
    {
        get
        {
            var x = NetworkClient.localPlayer;

            if (x != null)
            {
                return x.GetComponent<Player>();
            }

            return null;
        }
    }

    [SerializeField] private Vehicle vehiclePrefab;
    [SerializeField] private VehicleInputControl vehicleInputControl;
    public Vehicle ActiveVehicle { get; set; }

    [Header("Player")]
    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string Nickname;

    [SyncVar] [SerializeField] private int teamID;
    public int TeamID => teamID;

    [SyncVar(hook = nameof(OnFragsChanged))]
    private int frags;
    public int Frags
    {
        set
        {
            frags = value;

            //Server
            ChangeFrags.Invoke((int)netId, frags);
        }
        get { return frags; }
    }

    [Server]
    public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
    {
        ProjectileHit?.Invoke(hitResult);
        RpcInvokeProjectileHit(hitResult.Type, hitResult.Damage, hitResult.Point);
    }

    [ClientRpc]
    public void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
    {
        ProjectileHitResult hitResult = new ProjectileHitResult()
        {
            Type = type,
            Damage = damage,
            Point = hitPoint
        };

        ProjectileHit?.Invoke(hitResult);
    }
    
    //Client 
    private void OnFragsChanged(int oldValue, int newValue)
    {
        ChangeFrags?.Invoke((int)netId, newValue);
    }

    private PlayerData data;
    public PlayerData Data => data;

    private void OnNicknameChanged(string old, string newValue)
    {
        gameObject.name = "Player_" + newValue; // OnClient
    }

    [Command] // OnServer
    public void CmdSetName(string name)
    {
        Nickname = name;
        gameObject.name = "Player_" + name;
    }

    [Command]
    public void CmdSetTeamID(int teamID)
    {
        this.teamID = teamID;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        teamID = TeamIDCounter % 2;
        TeamIDCounter++;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
        PlayerList.Instance.SvRemovePlayer(data);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

            data = new PlayerData((int) netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, teamID);
            
            CmdAddPlayer(Data);
                
            CmdUpdateData(Data);
        }
    }

    [Command]
    private void CmdAddPlayer(PlayerData data)
    {
        PlayerList.Instance.SvAddPlayer(data);
    }
    
    [Command]
    private void CmdUpdateData(PlayerData data)
    {
        this.data = data;
    }
    
    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isOwned)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    private void OnMatchEnd()
    {
        if (ActiveVehicle != null)
        {
            ActiveVehicle.SetTargetControl(Vector3.zero);
            vehicleInputControl.enabled = false;
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (ActiveVehicle != null)
            {
                ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoom);
            }
        }

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
               NetworkSessionManager.Match.SvRestartMatch();
            }
        }

        if (isOwned)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    [Server]
    public void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;

        GameObject playerVehicle = Instantiate(vehiclePrefab.gameObject, transform.position, Quaternion.identity);

        playerVehicle.transform.position = teamID % 2 == 0
            ? NetworkSessionManager.Instance.RandomSpawnPointRed
            : NetworkSessionManager.Instance.RandomSpawnPointBlue;
        
            NetworkServer.Spawn(playerVehicle,netIdentity.connectionToClient);
        
        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        
        RpcSetVehicle(ActiveVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        ActiveVehicle = vehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;

        if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }
        
        vehicleInputControl.enabled = true;

        VehicleSpawned?.Invoke(ActiveVehicle);
    }
}