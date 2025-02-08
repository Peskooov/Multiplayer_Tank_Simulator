using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VehicleCamera : MonoBehaviour
{
    [SerializeField] private Vehicle vehicle;
    [SerializeField] private Vector3 offset;

    [Header("Sensetive Limit")] 
    [SerializeField] private float rotateSensetive;
    [SerializeField] private float scrollSensetive;
    
    [Header("Rotation Limit")] 
    [SerializeField] private float minVerticalAngle;
    [SerializeField] private float maxVerticalAngle;

    [Header("Distance")] 
    [SerializeField] private float distance;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float distanceLerpRate;
    [SerializeField] private float distanceOffsetFromCollisionHit;

    private new Camera camera;
    private Vector2 rotationControl;

    private float deltaRotationX;
    private float deltaRotationY;

    private float currentDistance;

    private void Start()
    {
        camera = GetComponent<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateControl();

        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        
        //Calculate position & rotation
        deltaRotationX += rotationControl.x * rotateSensetive;
        deltaRotationY += rotationControl.y * -rotateSensetive;

        deltaRotationY = ClampAngle(deltaRotationY, minVerticalAngle, maxVerticalAngle);

        Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
        Vector3 finalPosition = vehicle.transform.position - (finalRotation * Vector3.forward * distance);
        finalPosition = AddLocalOffset(finalPosition);

        // Calculate current distance
        float targetDistance = distance;

        RaycastHit hit;

        Debug.DrawLine(vehicle.transform.position + new Vector3(0, offset.y, 0), finalPosition, Color.red);

        if (Physics.Linecast(vehicle.transform.position + new Vector3(0, offset.y, 0), finalPosition, out hit))
        {
            float distanceToHit = Vector3.Distance(vehicle.transform.position + new Vector3(0, offset.y, 0), hit.point);

            if (hit.transform != vehicle)
            {
                if (distanceToHit < distance)
                    targetDistance = distanceToHit - distanceOffsetFromCollisionHit;
            }
        }

        currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);

        // Correct camera position
        finalPosition = vehicle.transform.position - (finalRotation * Vector3.forward * currentDistance);

        // Apply transform
        transform.rotation = finalRotation;
        transform.position = finalPosition;
        transform.position = AddLocalOffset(transform.position);
    }

    private void UpdateControl()
    {
        rotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        distance += -Input.mouseScrollDelta.y * scrollSensetive;
    }
    
    private Vector3 AddLocalOffset(Vector3 position)
    {
        Vector3 result = position;
        result += new Vector3(0, offset.y, 0);
        result += transform.right * offset.x;
        result += transform.forward * offset.z;

        return result;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Vehicle setTarget)
    {
        vehicle = setTarget;
    }
}