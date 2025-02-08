using Mirror;
using UnityEngine;

namespace Tanks2D
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Vehicle[] vehiclePrefabs;

        public Vehicle ActiveVehicle { get; set; }

        [SyncVar] private Transform playerTransform;
        public Transform PlayerTransform => playerTransform;

        [SyncVar] private Color playerColor;
        public Color PlayerColor => playerColor;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSpawnVehicle();
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            playerColor = PlayerColoPalette.Instance.TakeRandomColor();
            playerTransform = PlayerPositionPool.Instance.TakeRandomPosition();

            transform.position = playerTransform.position;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            PlayerColoPalette.Instance.PutColor(playerColor);
            PlayerPositionPool.Instance.PutPosition(playerTransform);
        }

        [Command]
        private void CmdSpawnVehicle()
        {
            SvSpawnClientVehicle();
        }

        [Server]
        public void SvSpawnClientVehicle()
        {
            if (ActiveVehicle != null) return;

            int index = Random.Range(0, vehiclePrefabs.Length);

            GameObject playerVehicle =
                Instantiate(vehiclePrefabs[index].gameObject, transform.position,
                    Quaternion.identity); // spawn on client
            NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient); // spawn on server

            ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
            ActiveVehicle.Owner = netIdentity;

            RpcSetVehicle(ActiveVehicle.netIdentity);
        }

        [ClientRpc]
        private void RpcSetVehicle(NetworkIdentity vehicle)
        {
            ActiveVehicle = vehicle.GetComponent<Vehicle>();

            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle.transform);
            }
        }
    }
}