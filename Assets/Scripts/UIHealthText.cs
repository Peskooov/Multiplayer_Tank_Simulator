using System.Collections;
using TMPro;
using UnityEngine;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private Destructible destructible;
    
    private void Start()
    {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    IEnumerator LateSubscribe()
    {
        while (NetworkSessionManager.Events.PlayerVehicleSpawned == null)
        {
            yield return new WaitForSeconds(1f);
        }
        
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }
    
    private void OnDestroy()
    {
        if (NetworkSessionManager.Events.PlayerVehicleSpawned  != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (destructible != null)
            destructible.HitPointChange -= OnHitPointChanged;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        destructible = vehicle;
 
        destructible.HitPointChange += OnHitPointChanged;
        text.text = destructible.HitPoint.ToString();
    }

    private void OnHitPointChanged(int hitPoint)
    {
        text.text = hitPoint.ToString();
    }
}
