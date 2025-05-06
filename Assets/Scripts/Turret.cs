using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    public event UnityAction<int> UpdateSelectedAmmunation;
    public event UnityAction Shot;
    
    [SerializeField] protected Transform launchPoint;
    public Transform LaunchPoint => launchPoint;
    
    [SerializeField] private float fireRate;

    [SerializeField] protected Ammunition[] ammunition;
    public Ammunition[] Ammunition => ammunition;
    
    private float fireTimer;
    public float FireTimerNormalized => fireTimer / fireRate;
    
    public ProjectileProperties SelectedProjectile => Ammunition[syncSelectedAmmunitionIndex].ProjectileProp;
    
    [SyncVar]
    [SerializeField] private int syncSelectedAmmunitionIndex;
    public int SelectedAmmunitionIndex => syncSelectedAmmunitionIndex;

    protected virtual void OnFire() { }
    
    public void SetSelectedProjectile(int index)
    {
        if(!isOwned) return;
        if(index <0 || index > ammunition.Length) return;
        
        syncSelectedAmmunitionIndex = index;
        
        if(isClient)
            CmdReloadAmmunition();
        
        UpdateSelectedAmmunation?.Invoke(index);
    }
    
    public void Fire()
    {
        if(!isOwned) return;
        
        if(isClient)
            CmdFire();
    }

    [Server]
    public void SvFire()
    {
        if(fireTimer>0) return;
        
        if(!ammunition[syncSelectedAmmunitionIndex].SvDrawAmmo(1)) return;
        
        OnFire();

        fireTimer = fireRate;

        RpcFire();
        
        Shot?.Invoke();
    }

    [Command]
    private void CmdReloadAmmunition()
    {
        fireTimer = fireRate;
    }

    [Command]
    private void CmdFire()
    {
       SvFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if(isServer) return;

        fireTimer = fireRate;
        
        OnFire();
        
        Shot?.Invoke();
    }
     
    protected virtual void Update()
    {
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;
        
        if (isOwned)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Клавиша 1
            {
                //projectileIndex = 0;
      
                    //RpcAmmoChanged();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // Клавиша 2
            {
                //projectileIndex = 1;
                
                //RpcAmmoChanged();
            }
        }
    }
}
