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
    private void RpcOnAddPlayer()
    {
        Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        PlayerVehicleSpawned?.Invoke(vehicle);
    }

    IEnumerator WaitPlayer()
    {
        while (Player.Local == null)
        {
            yield return new WaitForSeconds(1f);
        }
        
        Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }
}
