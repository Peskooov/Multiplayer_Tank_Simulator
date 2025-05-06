using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class MatchMemberList : NetworkBehaviour
{
	public static MatchMemberList Instance;
	public static UnityAction<List<MatchMemberData>> UpdateList;
	[SerializeField] private List<MatchMemberData> allMemberData = new List<MatchMemberData>();
	public int MemberDataCount => allMemberData.Count;
	
	private void Awake()
	{
		Instance = this;
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		allMemberData.Clear();
	}

	[Server]
	public void SvAddMember(MatchMemberData data)
	{
		allMemberData.Add(data);
		
		RpcClearMemberList();

		for (int i = 0; i < allMemberData.Count; i++)
		{
			RpcAddMember(allMemberData[i]);
		}
	}

	[Server]
	public void SvRemoveMember(MatchMemberData data)
	{
		for (int i = 0; i < allMemberData.Count; i++)
		{
			if (allMemberData[i].ID == data.ID)
			{
				allMemberData.RemoveAt(i);
				break;
			}
		}

		RpcRemoveMember(data);
	}

	[ClientRpc]
	private void RpcClearMemberList()
	{
		if(isServer) return;
		allMemberData.Clear();
	}

	[ClientRpc]
	private void RpcAddMember(MatchMemberData data)
	{
		if (isClient && isServer)
		{
			UpdateList?.Invoke(allMemberData);
			return;
		}

		allMemberData.Add(data);

		UpdateList?.Invoke(allMemberData);
	}

	[ClientRpc]
	private void RpcRemoveMember(MatchMemberData data)
	{
		for (int i = 0; i < allMemberData.Count; i++)
		{
			if (allMemberData[i].ID == data.ID)
			{
				allMemberData.RemoveAt(i);
				break;
			}
		}

		UpdateList?.Invoke(allMemberData);
	}
}