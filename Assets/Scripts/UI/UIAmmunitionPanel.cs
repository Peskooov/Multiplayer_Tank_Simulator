using System.Collections.Generic;
using UnityEngine;

public class UIAmmunitionPanel : MonoBehaviour
{
    [SerializeField] private Transform ammunitionPanel;
    [SerializeField] private UIAmmunitionElement ammunitionElementPrefab;

    private List<UIAmmunitionElement> allAmmunationElements = new List<UIAmmunitionElement>();
    private List<Ammunition> allAmmunation = new List<Ammunition>();
    private Turret turret;
    
    private int lastSelectionAmmunationIndex;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStarted()
    {
        turret = Player.Local.ActiveVehicle.Turret;
        turret.UpdateSelectedAmmunation += OnTurretUpdateSelectedAmmunation;

        for (int i = 0; i < ammunitionPanel.childCount; i++)
        {
            Destroy(ammunitionPanel.GetChild(i).gameObject);
        }

        allAmmunationElements.Clear();
        allAmmunation.Clear();
        
        for (int i = 0; i < turret.Ammunition.Length; i++)
        {
            UIAmmunitionElement ammunitionElement = Instantiate(ammunitionElementPrefab);
            ammunitionElement.transform.SetParent(ammunitionPanel);
            ammunitionElement.transform.localScale = Vector3.one;
            ammunitionElement.SetAmmunition(turret.Ammunition[i]);

            turret.Ammunition[i].AmmoCountChanged += OnAmmoCountChanged;

            allAmmunationElements.Add(ammunitionElement);
            allAmmunation.Add(turret.Ammunition[i]);

            if (i == 0)
            {
                ammunitionElement.Select();
            }
        }
    }

    private void OnMatchEnd()
    {
        if (turret != null)
            turret.UpdateSelectedAmmunation -= OnTurretUpdateSelectedAmmunation;

        for (int i = 0; i < allAmmunation.Count; i++)
        {
            allAmmunation[i].AmmoCountChanged -= OnAmmoCountChanged;
        }
    }
    
    private void OnAmmoCountChanged(int count)
    {
        allAmmunationElements[turret.SelectedAmmunitionIndex].UpdateAmmoCount(count);
    }

    private void OnTurretUpdateSelectedAmmunation(int index)
    {
        allAmmunationElements[lastSelectionAmmunationIndex].UnSelect();
        allAmmunationElements[index].Select();
        
        lastSelectionAmmunationIndex = index;
    }
}
