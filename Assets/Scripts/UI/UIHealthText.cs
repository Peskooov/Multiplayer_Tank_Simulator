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
    
    private void OnDestroy()
    {
        if (destructible != null)
            destructible.HitPointChanged -= OnHitPointChanged;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        destructible = vehicle;

        destructible.HitPointChanged += OnHitPointChanged;
        text.text = destructible.HitPoint.ToString();
    }

    private void OnHitPointChanged(int hitPoint)
    {
        text.text = hitPoint.ToString();
    }
}
