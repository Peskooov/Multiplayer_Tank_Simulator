using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleViewer : NetworkBehaviour
{
    private const float x_RAY_DISTANCE = 50;
    private const float base_EXIT_TIME_FROM_DISCOVERY = 10;
    private const float camouflage_DISTANCE = 150;
    private const float time_TO_UPDATE = 0.33f;
    
    [SerializeField] private float ViewDistance;
    [SerializeField] private Transform[] viewPoints;
    [SerializeField] private Color color;

    private Vehicle vehicle;
    private float remainingTimeLastUpdate;
    
    public List<VehicleDimensions> allVehicleDimensions = new ();
    public SyncList<NetworkIdentity> visibleVehicles = new ();
    public List<float> remainingTime = new ();

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        vehicle = GetComponent<Vehicle>();
        NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
        NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
    }

    private void OnSvMatchStart()
    {
        color = Random.ColorHSV();
        
        Vehicle[] allVeh = FindObjectsOfType<Vehicle>();
        for (int i = 0; i < allVeh.Length; i++)
        {
            if (vehicle == allVeh[i]) continue;

            VehicleDimensions vd = allVeh[i].GetComponent<VehicleDimensions>();
            if (vd == null) continue;

            if (vehicle.TeamID != allVeh[i].TeamID)
                allVehicleDimensions.Add(vd);
            else
            {
                visibleVehicles.Add(vd.Vehicle.netIdentity);
                remainingTime.Add(-1);
            }
        }
    }
    
    private void Update()
    {
        if (isServer == false) return;

        remainingTimeLastUpdate += Time.deltaTime;

       // if (remainingTimeLastUpdate >= time_TO_UPDATE)
       // {

            for (int i = 0; i < allVehicleDimensions.Count; i++)
            {
                if (allVehicleDimensions[i].Vehicle == null) continue;

                bool IsVisible = false;

                for (int j = 0; j < viewPoints.Length; j++)
                {
                    IsVisible = CheckVisibility(viewPoints[j].position, allVehicleDimensions[i]);

                    if (IsVisible == true) break;
                }

                if (IsVisible == true && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == false)
                {
                    visibleVehicles.Add(allVehicleDimensions[i].Vehicle.netIdentity);
                    remainingTime.Add(-1);
                }

                if (IsVisible == true && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] = -1;
                }

                if (IsVisible == false && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    if (remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] == -1)
                        remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] =
                            base_EXIT_TIME_FROM_DISCOVERY;
                    //visibleVehicles.Remove(allVehicleDimensions[i].Vehicle.netIdentity);
                }
            }

            for (int i = 0; i < remainingTime.Count; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= Time.deltaTime;
                    if (remainingTime[i] <= 0)
                        remainingTime[i] = 0;
                }

                if (remainingTime[i] == 0)
                {
                    remainingTime.RemoveAt(i);
                    visibleVehicles.RemoveAt(i);
                    i--; // Important: Decrement index after removal
                }

             //   remainingTimeLastUpdate = 0;
            //}
        }
    }

    public bool IsVisible(NetworkIdentity identity)
    {
        return visibleVehicles.Contains(identity);
    }
    
    private bool CheckVisibility(Vector3 viewPoint, VehicleDimensions vehicleDimensions)
    {
        float distance = Vector3.Distance(transform.position, vehicleDimensions.transform.position);
        
        if (Vector3.Distance(viewPoint, vehicleDimensions.transform.position) <= x_RAY_DISTANCE)
            return true;
        
        if (distance > ViewDistance) 
            return false;

        float currentViewDistance = ViewDistance;

        if (distance >= camouflage_DISTANCE)
        {
            VehicleCamouflage camouflage = vehicleDimensions.Vehicle.GetComponent<VehicleCamouflage>();
            
            if (camouflage != null)
                currentViewDistance = ViewDistance - camouflage.CurrentDistance;
        }

        if(distance > currentViewDistance) return false;
        
        return vehicleDimensions.IsVisibleFromPoint(transform.root, viewPoint, color);
    }
}