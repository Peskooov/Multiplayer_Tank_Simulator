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
        // Проверяем, что игрок и его транспортное средство существуют
        if (player == null || player.ActiveVehicle == null) return;

        // Убедимся, что управление происходит только для локального игрока
        if (player.isLocalPlayer)
        {
            // Управление движением
            player.ActiveVehicle.SetTargetControl(new Vector3(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Jump"),
                Input.GetAxis("Vertical")
            ));

            // Управление прицеливанием
            if (player.ActiveVehicle.isOwned)
            {
                player.ActiveVehicle.NetAimPoint =
                    TraceAimPointWithoutPlayerVehicle(
                        VehicleCamera.Instance.transform.position,
                        VehicleCamera.Instance.transform.forward
                    );
            }

            // Обработка выстрела
            if (Input.GetMouseButtonDown(0))
            {
                player.ActiveVehicle.Fire();
            }
        }
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

        // Получаем Rigidbody локального игрока, если он существует
        Rigidbody rb = null;
        if (Player.Local != null && Player.Local.ActiveVehicle != null)
        {
            rb = Player.Local.ActiveVehicle.GetComponent<Rigidbody>();
        }

        // Игнорируем попадание в транспортное средство локального игрока
        foreach (var hit in hits)
        {
            if (hit.rigidbody == rb) continue;

            return hit.point;
        }

        // Если ничего не найдено, возвращаем точку на расстоянии AimDistance
        return ray.GetPoint(AimDistance);
    }
}