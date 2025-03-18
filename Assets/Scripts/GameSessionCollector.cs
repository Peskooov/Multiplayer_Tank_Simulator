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
        if (Player.Local == null)
            StartCoroutine(DelayPlayerConnect());
        else
            Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }

    IEnumerator DelayPlayerConnect()
    {
        while (Player.Local == null)
        {
            yield return new WaitForSeconds(1f);
        }
        
        Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }
    
    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        PlayerVehicleSpawned?.Invoke(vehicle);
    }
}
