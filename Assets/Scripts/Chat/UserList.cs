using System.Collections.Generic;
using Mirror;
using UnityEngine.Events;

namespace NetworkChat
{
	public class UserList : NetworkBehaviour
	{
		public static UserList Instance;
		public static UnityAction<List<UserData>> UpdateUserList;
		public List<UserData> AllUsersData = new List<UserData>();
		
		private void Awake()
		{
			Instance = this;
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
		
			AllUsersData.Clear();
		}

		[Server]
		public void SvAddCurrentUser(UserData data)
		{
			if (AllUsersData.Exists(user => user.ID == data.ID))
			{
				return; // Данный пользователь уже существует, ничего не делаем.
			}
			
			AllUsersData.Add(data);

			//if (isServerOnly)
			//	RpcClearUserDataList();

			for (int i = 0; i < AllUsersData.Count; i++)
			{
				RpcAddCurrentUser(AllUsersData[i]);
			}
			
			UpdateUserList?.Invoke(AllUsersData);
		}

		[Server]
		public void SvRemoveCurrentUser(UserData data)
		{
			for (int i = 0; i < AllUsersData.Count; i++)
			{
				if (AllUsersData[i].ID == data.ID)
				{
					AllUsersData.RemoveAt(i);
					break;
				}
			}

			RpcRemoveCurrentUser(data);
		}

		[ClientRpc]
		private void RpcClearUserDataList()
		{
			AllUsersData.Clear();
		}

		[ClientRpc]
		private void RpcAddCurrentUser(UserData data)
		{
			if (AllUsersData.Exists(user => user.ID == data.ID))
			{
				//Debug.Log($"User with ID {userId} already exists on the client.");
				return; // Пользователь уже существует, ничего не делаем.
			}
			/*
			if (isClient && isServer)
			{
				UpdateUserList?.Invoke(AllUsersData);
				return;
			}*/
			
			AllUsersData.Add(data);
		
			UpdateUserList?.Invoke(AllUsersData);
		}

		[ClientRpc]
		private void RpcRemoveCurrentUser(UserData data)
		{
			for (int i = 0; i < AllUsersData.Count; i++)
			{
				if (AllUsersData[i].ID == data.ID)
				{
					AllUsersData.RemoveAt(i);
					break;
				}
			}
		
			UpdateUserList?.Invoke(AllUsersData);
		}
	}
}