using System;
using UnityEngine;
using Mirror;

namespace Tanks2D
{
    public class VehicleColor : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderer;
        [SerializeField] private Vehicle vehicle;

        private void Start()
        {
            Color color = vehicle.Owner.GetComponent<Player>().PlayerColor;

            foreach (var sprite in spriteRenderer)
            {
                sprite.color = new Color(color.r, color.g, color.b);
            }
        }
    }
}