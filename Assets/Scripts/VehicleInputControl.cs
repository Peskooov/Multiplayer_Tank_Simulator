using UnityEngine;

public class VehicleInputControl : MonoBehaviour
{
    [SerializeField] private Vehicle vehicle;
    
    protected virtual void Update()
    {
        vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"),
            Input.GetAxis("Vertical")));
    }
}