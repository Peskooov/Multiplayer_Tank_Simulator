using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerList : MonoBehaviour
{
    [SerializeField] private Transform localTeamPanel;
    [SerializeField] private Transform otherTeamPanel;
    
    [SerializeField] private UIPlayerLable playerLablePrefab;
    
    private List<UIPlayerLable> allPlayerLables = new List<UIPlayerLable>();

    private void Start()
    {
        MatchMemberList.UpdateList += OnUpdatePlayerList;
        Player.ChangeFrags += OnChangeFrags;
    }

    private void OnDestroy()
    {
        MatchMemberList.UpdateList -= OnUpdatePlayerList;
        Player.ChangeFrags -= OnChangeFrags;
    }

    private void OnUpdatePlayerList(List<MatchMemberData> playerData)
    {
        for (int i = 0; i < localTeamPanel.childCount; i++)
        {
            Destroy(localTeamPanel.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < otherTeamPanel.childCount; i++)
        {
            Destroy(otherTeamPanel.GetChild(i).gameObject);
        }
        
        allPlayerLables.Clear();

        for (int i = 0; i < playerData.Count; i++)
        {
            if (playerData[i].TeamID == Player.Local.TeamID)
            {
                AddPlayerLable(playerData[i],playerLablePrefab,localTeamPanel);
            }  
            else if (playerData[i].TeamID != Player.Local.TeamID)
            {
                AddPlayerLable(playerData[i],playerLablePrefab,otherTeamPanel);
            }     
        }
    }

    private void AddPlayerLable(MatchMemberData matchMemberData, UIPlayerLable playerLable,Transform parent)
    {
        UIPlayerLable uiPlayerLable = Instantiate(playerLable);
        uiPlayerLable.transform.SetParent(parent);
        Destructible destructible = matchMemberData.Member?.GetComponent<Destructible>();
        uiPlayerLable.Init(matchMemberData.ID, matchMemberData.Nickname, destructible);
        
        allPlayerLables.Add(uiPlayerLable);
    }

    private void OnChangeFrags(MatchMember member, int frags)
    {
        for (int i = 0; i < allPlayerLables.Count; i++)
        {
            if (allPlayerLables[i].NetID == member.netId)
            {
                allPlayerLables[i].UpdateFrags(frags);
            }
        }
    }
}
