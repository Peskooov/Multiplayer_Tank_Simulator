using Unity.VisualScripting;
using UnityEngine;

public class ConditionTeamDeathMatch : MonoBehaviour,IMatchCondition
{
    private int red;
    private int blue;
    
    private int winTeamID =-1;
    public int WinTeamID => winTeamID;

    private bool isTriggered;
    public bool IsTriggered => isTriggered;

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();

        foreach (var v in FindObjectsOfType<Player>())
        {
            if (v.ActiveVehicle == null) return;

            v.ActiveVehicle.OnEventDeath.AddListener(OnEventDeathHandler);
            
            if (v.TeamID == TeamSide.TeamRed)
            {
                red++;
            }
            else if (v.TeamID == TeamSide.TeamBlue)
            {
                blue++;
            }
        }
    }
    
    public void OnServerMatchEnd(MatchController controller)
    {
        
    }
    
    private void OnEventDeathHandler(Destructible dest)
    {
        var ownerPlayer = dest.Owner?.GetComponent<Player>();
        
        if(ownerPlayer == null) return;

        switch (ownerPlayer.TeamID)
        {
            case TeamSide.TeamRed:
                red --;
                break;
            case TeamSide.TeamBlue:
                blue--;
                break;
        }

        if (red <= 0)
        {
            red = 0;
            winTeamID = 1;
            isTriggered = true;
        }
        else if (blue <= 0)
        {
            blue = 0;
            winTeamID = 0;
            isTriggered = true;
        }
        
    }

    private void Reset()
    {
        red = 0;
        blue = 0;
        
        isTriggered = false;
    }
}
