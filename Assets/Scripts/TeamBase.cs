using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class TeamBase : MonoBehaviour
{
    [SerializeField] private float captureLevel;
    [SerializeField] private float captureAmountPerVehicle;
    [SerializeField] private int teamID;

    public float CaptureLevel => captureLevel;

    [SerializeField] private List<Vehicle> allVehicles = new List<Vehicle>();

    
    private void Update()
    {
        if (NetworkSessionManager.Instance.IsServer)
        {
            bool isAllDead = true;

            for (int i = 0; i < allVehicles.Count; i++)
            {
                if (allVehicles[i].HitPoint > 0)
                {
                    isAllDead = false;
                    
                    captureLevel += captureAmountPerVehicle * Time.deltaTime;
                    captureLevel = Mathf.Clamp(captureLevel, 0, 100);
                }
            }

            if (allVehicles.Count == 0 || isAllDead) 
                captureLevel = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vehicle vehicle = other.transform.root.GetComponent<Vehicle>();

        if (vehicle == null) return;
        if (vehicle.HitPoint <= 0) return;
        if (allVehicles.Contains(vehicle)) return;
        if (vehicle.Owner.GetComponent<Player>().TeamID == teamID) return;

        vehicle.HitPointChanged += OnHitPointChanged;
        allVehicles.Add(vehicle);
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle vehicle = other.transform.root.GetComponent<Vehicle>();
        
        if (vehicle == null) return;

        vehicle.HitPointChanged -= OnHitPointChanged;
        allVehicles.Remove(vehicle);
    }
    
    public void Reset()
    {
        captureLevel = 0;

        for (int i = 0; i < allVehicles.Count; i++)
        {
            allVehicles[i].HitPointChanged -= OnHitPointChanged;
        }
        
        allVehicles.Clear();
    }
    
    private void OnHitPointChanged(int value)
    {
        captureLevel = 0;
    } 
}