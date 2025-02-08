using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Turret : NetworkBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate;

    [SerializeField] private Slider slider;
    
    private float currentTime;

    private void Start()
    {
        if (slider)
        {
            slider.maxValue = fireRate;
            slider.value = fireRate;
        }
    }

    private void Update()
    {
        if (isOwned)
        {
            if (slider)
                slider.value = currentTime;
        }

        if (isServer)
        {
            currentTime += Time.deltaTime;
            
        }
    }

    [Command]
    public void CmdFire()
    {
        SvFire();
    }

    [Server]
    private void SvFire()
    {
        if (currentTime < fireRate) return;

        GameObject p = Instantiate(projectile, transform.position, transform.rotation);
        projectile.transform.GetComponent<Projectile>().SetParent(transform);

        currentTime = 0;
        
        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        GameObject p = Instantiate(projectile, transform.position, transform.rotation);
        projectile.transform.GetComponent<Projectile>().SetParent(transform);
    }
}
