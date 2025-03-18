using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    [SerializeField] protected Transform launchPoint;
    public Transform LaunchPoint => launchPoint;
    
    [SerializeField] private float fireRate;

    [SerializeField] protected Projectile[] projectilePrefab;
    public Projectile[] ProjectilePrefab => projectilePrefab;

    protected int projectileIndex;
    public int ProjectileIndex => projectileIndex;
    
    private float fireTimer;
    public float FireTimerNormalized => fireTimer / fireRate;

    [SyncVar]
    [SerializeField] protected int[] ammoCount;
    public int AmmoCount => ammoCount[projectileIndex];

    public UnityAction<int> OnAmmoChanged;

    private void Awake()
    {
        projectileIndex = 0;
    }

    [Server]
    public void SvAddAmmo(int count)
    { 
        ammoCount[projectileIndex] += count;
    
        RpcAmmoChanged();
    }
    
    [Server]
    protected virtual bool SvDrawAmmo(int count)
    {
        if (ammoCount[projectileIndex] <= 0) return false;

        if (ammoCount[projectileIndex] >= count)
        {
            ammoCount[projectileIndex] -= count;
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
        OnAmmoChanged?.Invoke(ammoCount[projectileIndex]);
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
        
        if (isOwned)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Клавиша 1
            {
                projectileIndex = 0;
                Debug.Log("Switched to projectile 1");
                
                RpcAmmoChanged();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // Клавиша 2
            {
                projectileIndex = 1;
                Debug.Log("Switched to projectile 2");
                
                RpcAmmoChanged();
            }
        }
    }
}
