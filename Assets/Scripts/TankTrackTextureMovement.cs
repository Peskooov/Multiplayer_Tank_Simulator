using System;
using UnityEngine;

public class TankTrackTextureMovement : MonoBehaviour
{
    [SerializeField] private Renderer leftTrackRenderer;
    [SerializeField] private Renderer rightTrackRenderer;

    [SerializeField] private Vector2 direction;
    [SerializeField] private float modifier;
    
    private TrackTank tank;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void FixedUpdate()
    {
        float speed = tank.LeftWheelRPM / 60 * modifier * Time.fixedDeltaTime;
        leftTrackRenderer.material.SetTextureOffset("_BaseMap", leftTrackRenderer.material.GetTextureOffset("_BaseMap") + direction * speed);
        
        speed = tank.RightWheelRPM / 60 * modifier * Time.fixedDeltaTime;
        rightTrackRenderer.material.SetTextureOffset("_BaseMap", rightTrackRenderer.material.GetTextureOffset("_BaseMap") + direction * speed);
    }
}
