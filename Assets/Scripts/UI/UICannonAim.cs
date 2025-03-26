using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;
    [SerializeField] private Image reloadSlider;
    
    private Vector3 aimPosition;
    [SerializeField] private Camera mainCamera;
    
    private void Update()
    { 
        if (Player.Local == null || Player.Local.ActiveVehicle == null)
            return;

        Vehicle vehicle = Player.Local.ActiveVehicle;
        
        reloadSlider.fillAmount = vehicle.Turret.FireTimerNormalized; 
        
        aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(vehicle.Turret.LaunchPoint.position,
            vehicle.Turret.LaunchPoint.forward);

        Vector3 result = mainCamera.WorldToScreenPoint(aimPosition);

        if (result.z > 0)
        {
            result.z = 0;
            aim.transform.position = result;
        }
    }
}
