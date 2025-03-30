using Mirror;
using UnityEngine;

public class NetworkSessionManager : NetworkManager
{
  [SerializeField] private GameSessionCollector gameSessionCollector;
  [SerializeField] private MatchController matchController;

  [SerializeField] private SphereArea[] spawnZoneRed;
  [SerializeField] private SphereArea[] spawnZoneBlue;

  public Vector3 RandomSpawnPointRed => spawnZoneRed[Random.Range(0, spawnZoneRed.Length)].RandomInside;
  public Vector3 RandomSpawnPointBlue => spawnZoneBlue[Random.Range(0, spawnZoneBlue.Length)].RandomInside;

  public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
  public static GameSessionCollector Events => Instance.gameSessionCollector;
  public static MatchController Match => Instance.matchController;

  public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
  public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);

  public override void OnServerConnect(NetworkConnectionToClient conn)
  {
      base.OnServerConnect(conn);
      
      gameSessionCollector.SvOnAddPlayer();
  }
}
