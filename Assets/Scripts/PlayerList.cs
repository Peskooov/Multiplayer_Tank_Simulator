using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerList : NetworkBehaviour
{
	public static PlayerList Instance;
	public static UnityAction<List<PlayerData>> UpdatePlayerList;
	public List<PlayerData> allPlayerData = new List<PlayerData>();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
        
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		allPlayerData.Clear();
	}

	[Server]
	public void SvAddPlayer(PlayerData data)
	{
		allPlayerData.Add(data);
		
		RpcClearPlayerDataList();

		for (int i = 0; i < allPlayerData.Count; i++)
		{
			RpcAddPlayer(allPlayerData[i]);
		}

		UpdatePlayerList?.Invoke(allPlayerData);
	}

	[Server]
	public void SvRemovePlayer(PlayerData data)
	{
		for (int i = 0; i < allPlayerData.Count; i++)
		{
			if (allPlayerData[i].ID == data.ID)
			{
				allPlayerData.RemoveAt(i);
				break;
			}
		}

		RpcRemovePlayer(data);
	}

	[ClientRpc]
	private void RpcClearPlayerDataList()
	{
		if(isServer) return;
		allPlayerData.Clear();
	}

	[ClientRpc]
	private void RpcAddPlayer(PlayerData data)
	{
		if (isClient && isServer)
		{
			UpdatePlayerList?.Invoke(allPlayerData);
			return;
		}

		allPlayerData.Add(data);

		UpdatePlayerList?.Invoke(allPlayerData);
	}

	[ClientRpc]
	private void RpcRemovePlayer(PlayerData data)
	{
		for (int i = 0; i < allPlayerData.Count; i++)
		{
			if (allPlayerData[i].ID == data.ID)
			{
				allPlayerData.RemoveAt(i);
				break;
			}
		}

		UpdatePlayerList?.Invoke(allPlayerData);
	}
}