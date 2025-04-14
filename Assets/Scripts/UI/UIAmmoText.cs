using UnityEngine;
using System.Collections;
using TMPro;

public class UIAmmoText : MonoBehaviour
{/*
    [SerializeField] private TMP_Text text;
    [SerializeField] private Transform selectImage;
    [SerializeField] private Transform firstProjectile;
    [SerializeField] private Transform secondProjectile;
    
    private Turret turret;
    
    private void Start()
    {
        if(NetworkSessionManager.Events != null)
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        else
        {
            StartCoroutine(WaitEvents());
        }
    }

    IEnumerator WaitEvents()
    {
        while (NetworkSessionManager.Events == null)
        {
            yield return new WaitForSeconds(1f);
            
        }
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }
    private void OnDestroy()
    {
        //if (NetworkSessionManager.Events.PlayerVehicleSpawned != null)
         //   NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (turret != null)
            turret.AmmoChanged -= OnAmmoChanged;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;
        
        turret.AmmoChanged += OnAmmoChanged;
        text.text = turret.AmmoCount.ToString();
    }

    private void OnAmmoChanged(int ammo)
    {
        text.text = Player.Local.ActiveVehicle.Turret.AmmoCount.ToString();
        
        selectImage.transform.position = turret.ProjectileIndex == 0 ? firstProjectile.position + new Vector3(0,15,0) : secondProjectile.position+ new Vector3(0,15,0);
    }*/
}
