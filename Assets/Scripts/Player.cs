using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    public static int TeamIDCounter;
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

    public UnityAction<Vehicle> VehicleSpawned;

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

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
        }
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
       

        if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }
        
        vehicleInputControl.enabled = true;

        VehicleSpawned?.Invoke(ActiveVehicle);
    }
}