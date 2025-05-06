using System;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float stopDistance = 1;
    [SerializeField] private float pathUpdateRate = 0.33f;

    private Vector3 target;
    private Vector3 nextPathPoint;
    private Vehicle vehicle;
    private NavMeshPath path;
    private int cornerIndex;
    
    private bool hasPath;
    private bool reachedDestination;

    private float timerUpdatePath;
    
    public bool HasPath => hasPath;
    public bool ReachedDestination => reachedDestination;
    
    private void Awake()
    {
        path = new NavMeshPath();
        vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        SetDestination( GameObject.FindGameObjectWithTag("Finish").transform.position);
        
        if (pathUpdateRate > 0)
        {
            timerUpdatePath += Time.deltaTime;

            if (timerUpdatePath > pathUpdateRate)
            {
                CalculatePath(target);
                timerUpdatePath = 0;
            }
        }
        
        UpdateTarget();
        MoveToTarget();
    }

    public void SetDestination(Vector3 target)
    {
        if(this.target == target && hasPath) return;
        
        this.target = target;
        
        CalculatePath(target);
    }

    public void ResetPath()
    {
     hasPath = false;
     reachedDestination = false;
    }

    private void UpdateTarget()
    {
        if(!hasPath) return;
        nextPathPoint = path.corners[cornerIndex];
        
        if (Vector3.Distance(transform.position, nextPathPoint) < stopDistance)
        {
            if (path.corners.Length - 1 > cornerIndex)
            {
                cornerIndex++;
                nextPathPoint = path.corners[cornerIndex];
            }
            else
            {
                hasPath = false;
                reachedDestination = true;
            }
        }
        
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    }

    private void CalculatePath(Vector3 target)
    {
        Debug.Log(target);
        
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        
        hasPath = path.corners.Length > 0;
        reachedDestination = false;
        
        cornerIndex = 1;
        
        
    }
    
    private Vector3 GetReferenceMovementDirectionZX()
    {
        var tankPosition = vehicle.transform.GetPositionZX();
        var targetPosition = nextPathPoint.GetPositionZX();
        
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
        if(nextPathPoint == null) return;

        if (reachedDestination)
        {
            vehicle.SetTargetControl(new Vector3(0, 1, 0));
            return;
        }

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
