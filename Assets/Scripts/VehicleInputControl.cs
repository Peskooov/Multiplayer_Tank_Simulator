using UnityEngine;

public class VehicleInputControl : MonoBehaviour
{
    public const float AimDistance = 1000;
    
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
        if (player.ActiveVehicle == null) return;

        if (player.isLocalPlayer && player.authority)
        {
            player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"),
                Input.GetAxis("Vertical")));
            player.ActiveVehicle.NetAimPoint =
                TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position,
                    VehicleCamera.Instance.transform.forward);

            if (Input.GetMouseButtonDown(0))
                player.ActiveVehicle.Fire();
        }
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

        Rigidbody rb = Player.Local.ActiveVehicle.GetComponent<Rigidbody>();

        foreach (var hit in hits)
        {
            if(hit.rigidbody == rb)
                continue;

            return hit.point;
        } 
        
        return ray.GetPoint(AimDistance);
    }
}