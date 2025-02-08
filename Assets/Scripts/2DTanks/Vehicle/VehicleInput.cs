using System;
using UnityEngine;

namespace Tanks2D
{
    [RequireComponent(typeof(Player))]
    public class VehicleInput : MonoBehaviour
    {
        private Player player;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        private void Update()
        {
            if (player.isOwned && player.isLocalPlayer)
                UpdateControlKeyboard();
        }

        private void UpdateControlKeyboard()
        {
            if (player.ActiveVehicle == null) return;

            float thrust = 0;
            float torque = 0;

            if (Input.GetKey(KeyCode.UpArrow))
                thrust = 1.0f;

            if (Input.GetKey(KeyCode.DownArrow))
                thrust = -1.0f;

            if (Input.GetKey(KeyCode.LeftArrow))
                torque = 1.0f;

            if (Input.GetKey(KeyCode.RightArrow))
                torque = -1.0f;

            if (Input.GetMouseButtonDown(0))
                player.ActiveVehicle.Fire();
            if (Input.GetMouseButtonDown(1))
                player.ActiveVehicle.RocketFire();

            player.ActiveVehicle.ThrustControl = thrust;
            player.ActiveVehicle.TorqueControl = torque;
        }
    }
}