using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    [SerializeField] private int maxHitPoints;
    [SerializeField] private GameObject destroySFX;

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
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeHitPoint(currentHitPoint, currentHitPoint -5);
        }
    }

    

    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = maxHitPoints;
        currentHitPoint = maxHitPoints;
    }

    [Server]
    public void SvAppyDamage(int damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            if (destroySFX != null)
            {
                GameObject sfx = Instantiate(destroySFX, transform.position, Quaternion.identity);
                NetworkServer.Spawn(sfx);
            }

            Destroy(gameObject);
        }
    }
    
    private void ChangeHitPoint(int oldValue, int newValue)
    {
        currentHitPoint = newValue;
        HitPointChange?.Invoke(newValue);
    }
}

