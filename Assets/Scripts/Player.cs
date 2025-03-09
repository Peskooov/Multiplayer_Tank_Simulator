using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private Vehicle vehiclePrefab;

    public Vehicle ActiveVehicle { get; set; }
    
    [Command]
    private void CmdSpawnVehicle()
    {
        SvSpawnClientVehicle();
    }

    [Server]
    public void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;
        
        GameObject playerVehicle =
            Instantiate(vehiclePrefab.gameObject, transform.position,
                Quaternion.identity); // spawn on client
        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient); // spawn on server

        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;

        RpcSetVehicle(ActiveVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        ActiveVehicle = vehicle.GetComponent<Vehicle>();

        if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }
    }
}