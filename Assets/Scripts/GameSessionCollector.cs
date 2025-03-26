using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class GameSessionCollector : NetworkBehaviour
{
    public UnityAction<Vehicle> PlayerVehicleSpawned;

    [Server]
    public void SvOnAddPlayer()
    {
        RpcOnAddPlayer();
    }

    [Client]
    public void RpcOnAddPlayer()
    {
        if (Player.Local != null)
        { 
            Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
        }
    }
    
    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        PlayerVehicleSpawned?.Invoke(vehicle);
    }
}
