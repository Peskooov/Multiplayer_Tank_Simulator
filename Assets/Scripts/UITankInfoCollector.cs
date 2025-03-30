using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform tankInfoPanel;
    [SerializeField] private UITankInfo tankInfoPrefab;
    
    private UITankInfo[] tankInfo;
    private List<Player> playersWithoutLocal;
    
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
        if (NetworkSessionManager.Match == null) return;
        
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
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
        if(tankInfo == null) return;

        for (int i = 0; i < tankInfo.Length; i++)
        {
            if(tankInfo[i] == null) return;
            
            Vector3 screenPos = VehicleCamera.Instance.Camera.WorldToScreenPoint(tankInfo[i].Tank.transform.position + tankInfo[i].WorldOffset);
            
            if(screenPos.z > 0)
            {
                tankInfo[i].transform.position = screenPos; 
            }
        }
    }

    private void OnMatchStart()
    {
        Player[] players = FindObjectsOfType<Player>();
        
        playersWithoutLocal = new List<Player>(players.Length -1);

        for (int i = 0; i < players.Length; i++)
        {
            if(players[i] == Player.Local) continue;
            
            playersWithoutLocal.Add(players[i]);
        }
        
        tankInfo = new UITankInfo[playersWithoutLocal.Count];

        for (int i = 0; i < playersWithoutLocal.Count; i++)
        {
            tankInfo[i] = Instantiate(tankInfoPrefab);
            tankInfo[i].SetTank(playersWithoutLocal[i].ActiveVehicle);
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
