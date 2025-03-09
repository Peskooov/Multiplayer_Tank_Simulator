using Mirror;
using UnityEngine;

public class NetworkSessionManager : NetworkManager
{
  [SerializeField] private SphereArea[] spawnZoneRed;
  [SerializeField] private SphereArea[] spawnZoneBlue;

  public Vector3 RandomSpawnPointRed => spawnZoneRed[Random.Range(0, spawnZoneRed.Length)].RandomInside;
  public Vector3 RandomSpawnPointBlue => spawnZoneBlue[Random.Range(0, spawnZoneBlue.Length)].RandomInside;
  
  public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

  public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
  public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);
}
