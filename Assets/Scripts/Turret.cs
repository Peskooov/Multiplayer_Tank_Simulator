using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    [SerializeField] protected Transform launchPoint;
    public Transform LaunchPoint => launchPoint;
    
    [SerializeField] private float fireRate;

    [SerializeField] protected Projectile projectilePrefab;
    public Projectile ProjectilePrefab => projectilePrefab;

    private float fireTimer;
    public float FireTimerNormalized => fireTimer / fireRate;

    [SyncVar]
    [SerializeField] protected int ammoCount;
    public int AmmoCount => ammoCount;

    public UnityAction<int> OnAmmoChanged;

    [Server]
    public void SvAddAmmo(int count)
    {
        ammoCount += count;
        RpcAmmoChanged();
    }
    
    [Server]
    protected virtual bool SvDrawAmmo(int count)
    {
        if (ammoCount <= 0) return false;

        if (ammoCount >= count)
        {
            ammoCount -= count;
            RpcAmmoChanged();

            return true;
        }
        
        return false;
    }

    [Command]
    private void CmdFire()
    {
        if(fireTimer>0) return;
        
        if(!SvDrawAmmo(1)) return;
        
        OnFire();

        fireTimer = fireRate;

        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if(isServer) return;

        fireTimer = fireRate;
        
        OnFire();
    }

    [ClientRpc]
    private void RpcAmmoChanged()
    {
        OnAmmoChanged?.Invoke(ammoCount);
    }
    
    protected virtual void OnFire()
    {
        
    }

    public void Fire()
    {
        if(!isOwned) return;
        
        if(isClient)
            CmdFire();
    }

    protected virtual void Update()
    {
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;
    }
}
