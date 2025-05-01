using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Vehicle))]
public class VehicleCamouflage : MonoBehaviour
{
    [SerializeField] private float baseDistance;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float percent;
    [SerializeField] private float percentLerpRate;
    [SerializeField] private float percentOnFire;

    private Vehicle vehicle;
    
    private float targetPercent;
    private float currentDistance;

    public float CurrentDistance => currentDistance;
    
    private void Start()
    {
        if (NetworkSessionManager.Instance.IsServer == false) return;

        vehicle = GetComponent<Vehicle>();
        vehicle.Turret.Shot += OnShot;
    }

    private void OnDestroy()
    {
        if(NetworkSessionManager.Instance.IsServer == false) return;
        
        vehicle.Turret.Shot -= OnShot;
    }

    private void Update()
    {
        if (NetworkSessionManager.Instance.IsServer == false) return;

        if (vehicle.NormalizedLinearVelocity > 0.01f)
            targetPercent = 0.5f;

        if (vehicle.NormalizedLinearVelocity <= 0.01f)
            targetPercent = 1.0f;

        percent = Mathf.MoveTowards(percent, targetPercent, Time.deltaTime * percentLerpRate);
        percent = Mathf.Clamp01(percent);
        
        currentDistance =  baseDistance * percent;
    }

    private void OnShot()
    {
        percent = percentOnFire;
    }
}
