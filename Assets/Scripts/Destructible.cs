using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public event UnityAction<Destructible> Destroyed;
    public event UnityAction<Destructible> Recovered;
    public event UnityAction<int> HitPointChanged;

    [SerializeField] private UnityEvent eventDestroyed;
    [SerializeField] private UnityEvent eventRecovered;

    [SerializeField] private int maxHitPoint;
    [SerializeField] private int currentHitPoint;

    public int MaxHitPoint => maxHitPoint;
    public int HitPoint => currentHitPoint;

    [SyncVar(hook = nameof(SyncHitPoint))]
    private int syncCurrentHitPoint;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = maxHitPoint;
        currentHitPoint = maxHitPoint;
    }

    [Server]
    public void SvApplyDamage(int damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            syncCurrentHitPoint = 0;

            RpcDestroy();
        }
    }

    [Server]
    protected void SvRecovery()
    {
        syncCurrentHitPoint = maxHitPoint;
        currentHitPoint = maxHitPoint;

        RpcRecovery();
    }
    #endregion

    #region Client
    private void SyncHitPoint(int oldValue, int newValue)
    {
        currentHitPoint = newValue;
        HitPointChanged?.Invoke(newValue);
    }
    
    [Client]
    private void RpcDestroy()
    {
        Destroyed?.Invoke(this);
        eventDestroyed?.Invoke();
    }
    
    [Client]
    private void RpcRecovery()
    {
        Recovered?.Invoke(this);
        eventRecovered?.Invoke();
    }
    #endregion
}