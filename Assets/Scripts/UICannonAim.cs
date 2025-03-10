using System;
using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    private Vector3 aimPosition;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(Player.Local == null) return;
        if(Player.Local.ActiveVehicle == null) return;

        Vehicle vehicle = Player.Local.ActiveVehicle;

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
