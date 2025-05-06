using System;
using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 GetPositionZX(this Transform t)
    {
        var x = t.position;
        x.y = 0;
        return x;
    }
}

public static class VectorExtensions
{
    public static Vector3 GetPositionZX(this Vector3 t)
    {
        var x = t;
        x.y = 0;
        return x;
    }
}

[RequireComponent(typeof(Vehicle))]
public class AIMovement : MonoBehaviour
{
    [SerializeField] private AIRaySensor sensorForward;
    [SerializeField] private AIRaySensor sensorBackward;
    [SerializeField] private AIRaySensor sensorLeft;
    [SerializeField] private AIRaySensor sensorRight;

    private Vector3 target;
    private Vehicle vehicle;
    
    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Finish").transform.position;
        
        MoveToTarget();
    }

    private Vector3 GetReferenceMovementDirectionZX()
    {
        var tankPosition = vehicle.transform.GetPositionZX();
        var targetPosition = target.GetPositionZX();
        
        return (tankPosition - tankPosition).normalized;
    }
    
    private Vector3 GetTankDirectionZX()
    {
        var tankDirection = vehicle.transform.forward.GetPositionZX();
        tankDirection.Normalize();
        
        return tankDirection;
    }
    
    private void MoveToTarget()
    {
        float turnControl = 0;
        float forwardThrust = 1;

        var referenceDirection = GetReferenceMovementDirectionZX();
        var tankDirection = GetTankDirectionZX();
        
        var forwardSensorState = sensorForward.Raycast();
        var leftSensorState = sensorLeft.Raycast();
        var rightSensorState = sensorRight.Raycast();

        if (forwardSensorState.Item1)
        {
            forwardThrust = 0;

            if (leftSensorState.Item1 == false)
            {
                turnControl = -1;
                forwardThrust = -0.2f;
            }
            else if (rightSensorState.Item1 == false)
            {
                turnControl = 1;
                forwardThrust = -0.2f;
            }
            else
            {
                forwardThrust = -1;
            }
        }
        else
        {
            turnControl = Mathf.Clamp(Vector3.SignedAngle( tankDirection, referenceDirection, Vector3.up), -45f, 45f)/ 45f;
            
            float MinSideDistance = 1;

            if (leftSensorState.Item1 && leftSensorState.Item2 < MinSideDistance && turnControl < 0)
                turnControl = -turnControl;

            if (rightSensorState.Item1 && rightSensorState.Item2 < MinSideDistance && turnControl > 0)
                turnControl = -turnControl;
        }
        
        vehicle.SetTargetControl(new Vector3(turnControl, 0, forwardThrust));
    }
}
