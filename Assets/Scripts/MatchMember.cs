using UnityEngine;
using UnityEngine.Events;
using Mirror;

[System.Serializable]
public class MatchMemberData
{
    public int ID;
    public string Nickname;
    public int TeamID;
    public NetworkIdentity Member;
    
    public MatchMemberData(int id, string nickname, int teamID, NetworkIdentity member)
    {
        ID = id;
        Nickname = nickname;
        TeamID = teamID;
        Member = member;
    }
}

public static class MatchMemberDataEctention
{
    public static void WriteMatchMemberData(this NetworkWriter writer, MatchMemberData data)
    {
        writer.WriteInt(data.ID);
        writer.WriteString(data.Nickname);
        writer.WriteInt(data.TeamID);
        writer.WriteNetworkIdentity(data.Member);
    }

    public static MatchMemberData ReadMatchMemberData(this NetworkReader reader)
    {
        return new MatchMemberData(reader.ReadInt(), reader.ReadString(), reader.ReadInt(),
            reader.ReadNetworkIdentity());
    }
}

public class MatchMember : NetworkBehaviour
{
    public static event UnityAction<MatchMember, int> ChangeFrags;
    public Vehicle ActiveVehicle { get; set; }

    #region Data

    protected MatchMemberData data;
    public MatchMemberData Data => data;

    [Command]
    protected void CmdUpdateData(MatchMemberData data)
    {
        this.data = data;
    }

    #endregion

    #region Frags

    [SyncVar(hook = nameof(OnFragsChanged))]
    protected int fragsAmount;

    [Server]
    public void SvAddFrags()
    {
        fragsAmount++;

        ChangeFrags?.Invoke(this, fragsAmount);
    }

    [Server]
    public void SvResetFrags()
    {
        fragsAmount = 0;
    }

    private void OnFragsChanged(int oldValue, int newValue)
    {
        ChangeFrags?.Invoke(this, newValue);
    }

    #endregion

    #region Nickname

    [SyncVar(hook = nameof(OnNicknameChanged))]
    protected string nickname;

    public string Nickname => nickname;

    [Command]
    protected void CmdSetName(string name)
    {
        nickname = name;
        gameObject.name = name;
    }

    private void OnNicknameChanged(string oldValue, string newValue)
    {
        gameObject.name = newValue;
    }

    #endregion

    #region TeamID

    [SyncVar] 
    protected int teamID;
    public int TeamID => teamID;

    #endregion
}