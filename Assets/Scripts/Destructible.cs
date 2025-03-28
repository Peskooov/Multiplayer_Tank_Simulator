using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    [SerializeField] private int maxHitPoints;
    [SerializeField] private GameObject destroySFX;

    [SerializeField] private UnityEvent<Destructible> OnDestroed;
    public UnityEvent<Destructible> OnEventDeath => OnDestroed;
    
    public UnityAction<int> HitPointChange;

    public int MaxHitPoints => maxHitPoints;    
    
    public int HitPoint => currentHitPoint;
    private int currentHitPoint;
    
    [SyncVar( hook = nameof(ChangeHitPoint) )]
    private int syncCurrentHitPoint;

    [SyncVar( hook = "T")] 
    public NetworkIdentity Owner;
    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = maxHitPoints;
        currentHitPoint = maxHitPoints;
    }

    [Server]
    public void SvApplyDamage(int damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            if (destroySFX != null)
            {
                GameObject sfx = Instantiate(destroySFX, transform.position, Quaternion.identity);
                NetworkServer.Spawn(sfx);
            }

            syncCurrentHitPoint = 0;
            
            RpcDestroy();
        }
    }

    [Client]
    private void RpcDestroy()
    {
        OnDestructibleDestroy();
    }

    protected virtual void OnDestructibleDestroy()
    {
        OnDestroed?.Invoke(this);
    }
    
    private void ChangeHitPoint(int oldValue, int newValue)
    {
        currentHitPoint = newValue;
        HitPointChange?.Invoke(newValue);
    }
}

