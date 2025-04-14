using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Ammunition : NetworkBehaviour
{
    public event UnityAction<int> AmmoCountChanged;
    
    [SerializeField] protected ProjectileProperties projectileProp;
    
    [SyncVar(hook = nameof(SyncAmmCount))]
    [SerializeField] protected int syncAmmoCount;
    
    public ProjectileProperties ProjectileProp => projectileProp;
    public int AmmoCount => syncAmmoCount;

    #region Server

    [Server]
    public void SvAddAmmo(int count)
    {
        syncAmmoCount += count;
    }

    [Server]
    public bool SvDrawAmmo(int count)
    {
        if (syncAmmoCount == 0)
        {    
            return false;
        }
        
        if (syncAmmoCount >= count)
        {
            syncAmmoCount -= count;
            return true;
        }
        
        return false;
    }

    #endregion

    #region Client

    private void SyncAmmCount(int oldCount, int newCount)
    {
        AmmoCountChanged?.Invoke(newCount);
    }

    #endregion
}
