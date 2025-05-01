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
        if (player == null || player.ActiveVehicle == null) return;
        
        if (player.isOwned && player.isLocalPlayer)
        {
            player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"),Input.GetAxis("Vertical")));
            player.ActiveVehicle.NetAimPoint =TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position,VehicleCamera.Instance.transform.forward);
            
            if (Input.GetMouseButtonDown(0))
            {
                player.ActiveVehicle.Fire();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) player.ActiveVehicle.Turret.SetSelectedProjectile(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) player.ActiveVehicle.Turret.SetSelectedProjectile(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) player.ActiveVehicle.Turret.SetSelectedProjectile(2);
        }
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance); // Обратный порядок

        var t = Player.Local.ActiveVehicle;

        for (int i = hits.Length - 1; i >= 0; i--)
        {
            if (hits[i].collider.isTrigger == true)
                continue;
            if (hits[i].collider.transform.root.GetComponent<Vehicle>() == t)
                continue;
            return hits[i].point;
        }
        return ray.GetPoint(AimDistance);
    }
}