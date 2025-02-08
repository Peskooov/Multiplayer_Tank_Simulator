using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkChat
{
    public class User : NetworkBehaviour
    {
        public static UnityAction<UserData, string> ReceiveMessageToChat;
        public static UnityAction<int, string, string> ReceivePrivateMessage;

        public static User Local
        {
            get
            {
                var x = NetworkClient.localPlayer;

                if (x != null)
                    return x.GetComponent<User>();

                return null;
            }
        }
        
        private UserData data;
        private UIMessageInputField messageInputField;

        public UserData Data => data;

        
        
        public override void OnStopServer()
        {
            base.OnStopServer();
            
            UserList.Instance.SvRemoveCurrentUser(data);
        }

        private void Start()
        {
            messageInputField = UIMessageInputField.Instance;
            data = new UserData((int)netId, "Nickname", messageInputField.GetPrivate());
        }

        private void Update()
        {
            if(!isOwned) return;
            
            if (Input.GetKey(KeyCode.Return))
            {
                SendMessageToChat();
            }
        }

        //Join
        public void JoinToChat()
        {
            data.Nickname = messageInputField.GetNickname();
            
            CmdAddUser(data);
        }

        [Command]
        private void CmdAddUser(UserData data)
        {
            UserList.Instance.SvAddCurrentUser(data);
        }
        
        [Command]
        private void CmdRemoveUser(UserData data)
        {
            UserList.Instance.SvRemoveCurrentUser(data);
        }
        
        //AllChat
        public void SendMessageToChat()
        {
            if(!isOwned) return;
            if(messageInputField.IsEmpty) return;
            
            CmdSendMessageToChat(data, messageInputField.GetString());

            messageInputField.ClearString();
        }

        [Command]
        private void CmdSendMessageToChat(UserData data, string message)
        {
            Debug.Log($"User send {data.ID} message to server. Message: " + message);
            
            SvPostMessage(data, message );
        }
        
        [Server]
        private void SvPostMessage(UserData data, string message)
        {
            Debug.Log("Server received message by user. Message: " + message);
            
            RpcReceiveMessage(data, message);
        }

        [ClientRpc]
        private void RpcReceiveMessage(UserData data, string message)
        {
            Debug.Log("User received message. Message: " + message);
            
            ReceiveMessageToChat?.Invoke(data,message);
        }
    }
}