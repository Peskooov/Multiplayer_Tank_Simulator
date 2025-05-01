using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{
    [SerializeField] private Transform mainCanvas;
    
    [SerializeField] private SizeMap sizeMap;
    
    [SerializeField] private UITankMark tankMarkPrefab;
    
    [SerializeField] private Image bgImage;
    
    private UITankMark[] tankMarks;
    private Vehicle[] vehicles;

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

    private IEnumerator WaitForMatch()
    {
        while (NetworkSessionManager.Match == null)
        {
            yield return new WaitForSeconds(1f);
        }
        
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }
    
    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }

    }

    private void Update()
    {
        if(tankMarks == null) return;

        for (int i = 0; i < tankMarks.Length; i++)
        {
            if(vehicles[i] == null) continue;
            
            if (vehicles[i] != Player.Local.ActiveVehicle)
            {
                bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(vehicles[i].netIdentity);
                tankMarks[i].gameObject.SetActive(isVisible);
            }

            if(!tankMarks[i].gameObject.activeSelf) continue;
            
            
            Vector3 normPos = sizeMap.GetNormPos(vehicles[i].transform.position);
            
            Vector3 posInMinimap = new Vector3(normPos.x * bgImage.rectTransform.sizeDelta.x * 0.5f, normPos.z * bgImage.rectTransform.sizeDelta.y * 0.5f, 0);
            posInMinimap.x *= mainCanvas.localScale.x;
            posInMinimap.y *= mainCanvas.localScale.y;
            
            tankMarks[i].transform.position = bgImage.transform.position + posInMinimap;
        }
    }

    private void OnMatchStart()
    {
        vehicles = FindObjectsOfType<Vehicle>();
        
        tankMarks = new UITankMark[vehicles.Length];

        for (int i = 0; i < tankMarks.Length; i++)
        {
            tankMarks[i] = Instantiate(tankMarkPrefab);
            
            if(vehicles[i].TeamID == Player.Local.TeamID)
                tankMarks[i].SetLocalTeamColor();
            else
                tankMarks[i].SetOtherTeamColor();
            
            tankMarks[i].transform.SetParent(bgImage.transform);
        }
    }

    private void OnMatchEnd()
    {
        for (int i = 0; i < bgImage.transform.childCount; i++)
        {
            Destroy(bgImage.transform.GetChild(i).gameObject);
        }
        
        tankMarks = null;
    }
}
