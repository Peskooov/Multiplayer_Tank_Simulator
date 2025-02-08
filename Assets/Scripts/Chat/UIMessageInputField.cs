using Mirror;
using TMPro;
using UnityEngine;

namespace NetworkChat
{
    public class UIMessageInputField : MonoBehaviour
    {
        public static UIMessageInputField Instance;
        
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private TMP_InputField nicknameInputField;
        [SerializeField] private TMP_InputField privateInput;
        public bool IsEmpty => messageInputField.text == "";
        
        private NetworkManager networkManager;
        
        private void Awake()
        {
            Instance = this;
            
            networkManager = FindFirstObjectByType<NetworkManager>();
            
            
        }

        public string GetString()
        {
            return messageInputField.text;
        }

        public string GetNickname()
        {
            return nicknameInputField.text;
        }

        public string GetPrivate()
        {
            return privateInput.text;
        }
        
        public void ClearString()
        {
            messageInputField.text = "";
        }
        
        public void ClearPrivate()
        {
            privateInput.text = "";
        }

        public void SendMessageToChat()
        {
            User.Local.SendMessageToChat();
        }
        
        public void JoinToChat()
        {
            

            User.Local.JoinToChat();
        }
    }
}