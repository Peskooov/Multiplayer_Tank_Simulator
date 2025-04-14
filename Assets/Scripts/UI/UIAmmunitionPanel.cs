using System.Collections.Generic;
using UnityEngine;

public class UIAmmunitionPanel : MonoBehaviour
{
    [SerializeField] private Transform ammunitionPanel;
    [SerializeField] private UIAmmunitionElement ammunitionElementPrefab;

    private List<UIAmmunitionElement> allAmmunitionElements = new List<UIAmmunitionElement>();
    private List<Ammunition> allAmmunition = new List<Ammunition>();
    private Turret turret;
    
    private int lastSelectionAmmunationIndex;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (turret != null)
            turret.UpdateSelectedAmmunition -= OnTurretUpdateSelectedAmmunition;

        for (int i = 0; i < allAmmunition.Count; i++)
        {
            allAmmunition[i].AmmoCountChanged -= OnAmmoCountChanged;
        }
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;
        turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;

        for (int i = 0; i < turret.Ammunition.Length; i++)
        {
            UIAmmunitionElement ammunitionElement = Instantiate(ammunitionElementPrefab);
            ammunitionElement.transform.SetParent(ammunitionPanel);
            ammunitionElement.transform.localScale = Vector3.one;
            
            ammunitionElement.SetAmmunition(turret.Ammunition[i]);

            turret.Ammunition[i].AmmoCountChanged += OnAmmoCountChanged;
            
            allAmmunitionElements.Add(ammunitionElement);
            allAmmunition.Add(turret.Ammunition[i]);
            
            if (i == 0)
            {
                ammunitionElement.Select();
            }
        }
    }

    private void OnAmmoCountChanged(int count)
    {
        allAmmunitionElements[turret.SelectedAmmunitionIndex].UpdateAmmoCount(count);
    }

    private void OnTurretUpdateSelectedAmmunition(int index)
    {
        allAmmunitionElements[lastSelectionAmmunationIndex].UnSelect();
        allAmmunitionElements[index].Select();
        
        lastSelectionAmmunationIndex = index;
    }
}
