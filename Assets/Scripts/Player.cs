using System.Collections;
using Mirror;
using Unity.VisualScripting;
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
                return x.GetComponent<Player>();

            return null;
        }
    }

    [SerializeField] private Vehicle vehiclePrefab;
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

        // teamID = TeamIDCounter % 2;
        // TeamIDCounter++;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (authority)
        {
            // CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (ActiveVehicle != null && VehicleCamera.Instance != null)
            {
                ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoom);
            }
        }

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                foreach (var p in FindObjectsOfType<Player>())
                {
                    if (p.ActiveVehicle != null)
                    {
                        NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                        Destroy(p.ActiveVehicle.gameObject);
                        p.ActiveVehicle = null; // Сбрасываем ссылку на объект
                    }
                }

                foreach (var p in FindObjectsOfType<Player>())
                {
                    p.SvSpawnClientVehicle();
                }
            }
        }

        if (authority)
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

        Debug.Log("Spawning tank on server");
        GameObject playerVehicle = Instantiate(vehiclePrefab.gameObject, transform.position,
                Quaternion.identity); // spawn on client

        playerVehicle.transform.position = teamID % 2 == 0
            ? NetworkSessionManager.Instance.RandomSpawnPointRed
            : NetworkSessionManager.Instance.RandomSpawnPointBlue;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient); // spawn on server

        Debug.Log("Tank spawned with netId: " + playerVehicle.GetComponent<NetworkIdentity>().netId);

        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;

        Debug.Log("Tank owner set to: " + netIdentity.netId);

        RpcSetVehicle(ActiveVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null)
        {
            Debug.LogWarning("RpcSetVehicle: vehicle is null");
            return;
        }

        StartCoroutine(WaitAndSetVehicle(vehicle));
    }

    private IEnumerator WaitAndSetVehicle(NetworkIdentity vehicle)
    {
        yield return new WaitForSeconds(0.1f); // Подождать 100 мс

        if (vehicle != null)
        {
            ActiveVehicle = vehicle.GetComponent<Vehicle>();

            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                Debug.Log("Setting vehicle target for camera");
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            }

            VehicleSpawned?.Invoke(ActiveVehicle);
        }
        else
        {
            Debug.LogWarning("WaitAndSetVehicle: vehicle is null after delay");
        }
    }

    [Server]
    public void SvDespawnClientVehicle()
    {
        if (ActiveVehicle != null)
        {
            Debug.Log("Despawning tank on server");
            NetworkServer.UnSpawn(ActiveVehicle.gameObject);
            Destroy(ActiveVehicle.gameObject);
            ActiveVehicle = null; // Сбрасываем ссылку на объект
        }
    }
}