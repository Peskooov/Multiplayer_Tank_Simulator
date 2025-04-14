using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{
    [SerializeField] private SizeMap sizeMap;
    
    [SerializeField] private UITankMark tankMarkPrefab;
    
    [SerializeField] private Image bgImage;
    
    private UITankMark[] tankMarks;
    private Player[] players;

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
            if(players[i] == null) continue;
            
            Vector3 normPos = sizeMap.GetNormPos(players[i].ActiveVehicle.transform.position);
            
            Vector3 posInMinimap = new Vector3(normPos.x * bgImage.rectTransform.sizeDelta.x * 0.5f, normPos.z * bgImage.rectTransform.sizeDelta.y * 0.5f, 0);
            
            tankMarks[i].transform.position = bgImage.transform.position + posInMinimap;
        }
    }

    private void OnMatchStart()
    {
        players = FindObjectsOfType<Player>();
        
        tankMarks = new UITankMark[players.Length];

        for (int i = 0; i < tankMarks.Length; i++)
        {
            tankMarks[i] = Instantiate(tankMarkPrefab);
            
            if(players[i].TeamID == Player.Local.TeamID)
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
