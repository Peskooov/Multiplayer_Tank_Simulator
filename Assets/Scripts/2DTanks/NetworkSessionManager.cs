using Mirror;
using UnityEngine;

namespace Tanks2D
{
  public class NetworkSessionManager : NetworkManager
  {
    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);
  }
}