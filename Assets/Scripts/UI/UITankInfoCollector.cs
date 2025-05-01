using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform tankInfoPanel;
    [SerializeField] private UITankInfo tankInfoPrefab;
    
    private UITankInfo[] tankInfo;
    private List<Vehicle> vehiclesWithoutLocal;
    
    private void Start()
    {
        if (NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
        }
        else
        {
            StartCoroutine(WaitForMatch());
        }
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    private IEnumerator WaitForMatch()
    {
        while (NetworkSessionManager.Match == null)
        {
            yield return new WaitForSeconds(1f);
        }
        
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }
    
    private void Update()
    {
        if (tankInfo == null) return;
    
        for (int i = 0; i < tankInfo.Length; i++)
        {
            if (tankInfo[i] == null) continue;
        
            bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(tankInfo[i].Tank.netIdentity);
            tankInfo[i].gameObject.SetActive(isVisible);
        
            if (tankInfo[i].gameObject.activeSelf == false) continue;
        
            Vector3 screenPos = VehicleCamera.Instance.Camera.WorldToScreenPoint(tankInfo[i].Tank.transform.position + tankInfo[i].WorldOffset);
            if (screenPos.z > 0)
            {
                tankInfo[i].transform.position = screenPos;
            }
        }
    }

    private void OnMatchStart()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();
        
        vehiclesWithoutLocal = new List<Vehicle>(vehicles.Length -1);

        for (int i = 0; i < vehicles.Length; i++)
        {
            if(vehicles[i] == Player.Local.ActiveVehicle) continue;
            
            vehiclesWithoutLocal.Add(vehicles[i]);
        }
        
        tankInfo = new UITankInfo[vehiclesWithoutLocal.Count];

        for (int i = 0; i < vehiclesWithoutLocal.Count; i++)
        {
            tankInfo[i] = Instantiate(tankInfoPrefab);
            tankInfo[i].SetTank(vehiclesWithoutLocal[i]);
            tankInfo[i].transform.SetParent(tankInfoPanel);
        }
    }

    private void OnMatchEnd()
    {
        for (int i = 0; i < tankInfoPanel.transform.childCount; i++)
        {
            Destroy(tankInfoPanel.transform.GetChild(i).gameObject);
        }

        tankInfo = null;
    }
}
