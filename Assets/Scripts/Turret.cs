using Mirror;
using UnityEngine;

public class Turret : NetworkBehaviour
{
    [SerializeField] protected Transform launchPoint;
    public Transform LaunchPoint => launchPoint;
    
    
}
