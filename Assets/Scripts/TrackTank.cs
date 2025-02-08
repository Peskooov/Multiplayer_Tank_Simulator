using System;
using UnityEngine;

[Serializable]
public class TrackWheelRow
{
    [SerializeField] private WheelCollider[] colliders;
    [SerializeField] private Transform[] meshes;
    
    public void SetTorque( float motorTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].motorTorque = motorTorque;
        }
    }
    
    public void Break(float breakTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = breakTorque;
        }
    }

    public void UpdateMeshTransform()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;
            
            colliders[i].GetWorldPose(out position, out rotation);

            meshes[i].position = position;
            meshes[i].rotation = rotation;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].motorTorque = 0;
            colliders[i].brakeTorque = 0;
        }
    }
}

public class TrackTank : Vehicle
{
    [SerializeField] private TrackWheelRow leftWheelRow;
    [SerializeField] private TrackWheelRow rightWheelRow;
}