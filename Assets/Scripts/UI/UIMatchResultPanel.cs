using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMatchResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject matchResultPanel;
    [SerializeField] private TMP_Text matchResultText;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
       // NetworkSessionManager.Match.MatchStart -= OnMatchStart;
       // NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    {
        matchResultPanel.SetActive(false);
    } 
    
    private void OnMatchEnd()
    {
        matchResultPanel.SetActive(true);

        int winTeamID = NetworkSessionManager.Match.WinTeamID;

        if (winTeamID == -1)
        {
            matchResultText.text = "Ничья!";
            return;
        }
        
        if (winTeamID == Player.Local.TeamID)
        {
            matchResultText.text = "Победа!";
            matchResultText.color = Color.green;
        }
        else
        {
            matchResultText.text = "Поражение!";
            matchResultText.color = Color.red;
        }
        
    } 
}
