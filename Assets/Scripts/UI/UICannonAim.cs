using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;
    [SerializeField] private Image reloadSlider;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float smoothSpeed = 10f; // Добавлен параметр сглаживания
    
    private Vector3 aimPosition;
    private Vector3 aimScreenPosition;
    private Vector3 currentVelocity;
    private void Update()
    { 
        if (Player.Local == null || Player.Local.ActiveVehicle == null)
            return;
        
        
        Vehicle vehicle = Player.Local.ActiveVehicle;
        
        // Обновление полоски перезарядки
        reloadSlider.fillAmount = vehicle.Turret.FireTimerNormalized; 
        
        // Получение точки прицеливания
        Vector3 worldAimPoint = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(
            vehicle.Turret.LaunchPoint.position,
            vehicle.Turret.LaunchPoint.forward);

        // Конвертация в экранные координаты
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldAimPoint);
        
        // Проверка, находится ли точка перед камерой
        if(screenPoint.z > 0)
        {
            // Сглаживание движения прицела
            aimScreenPosition = Vector3.SmoothDamp(
                aimScreenPosition, 
                screenPoint, 
                ref currentVelocity, 
                smoothSpeed * Time.deltaTime);
            
            aim.transform.position = aimScreenPosition;
        }
        
        //aim.gameObject.SetActive(screenPoint.z > 0);
    }
}
