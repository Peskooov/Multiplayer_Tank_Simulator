using System.Collections.Generic;
using UnityEngine;

namespace NetworkChat
{
    public class UIMessageViewer : MonoBehaviour
    {
        [SerializeField] private UIMessageBox messageBox;
        [SerializeField] private Transform messagePanel;

        [SerializeField] private UIMessageBox userBox;
        [SerializeField] private Transform userListPanel;
        
        private void Start()
        {
            User.ReceiveMessageToChat += OnReceiveMessageToChat;
            UserList.UpdateUserList += OnUpdateUserList;
        }

        private void OnDestroy()
        {
            User.ReceiveMessageToChat -= OnReceiveMessageToChat; 
            UserList.UpdateUserList -= OnUpdateUserList;
        }

        private void OnReceiveMessageToChat(UserData data, string message)
        {
            AppendMessage(data, message);
        }

        private void AppendMessage(UserData data, string message)
        {
            if (string.IsNullOrEmpty(data.PrivateMessage) || data.PrivateMessage == User.Local.Data.Nickname || data.ID == User.Local.Data.ID)
            {
                UIMessageBox newMessageBox = Instantiate(messageBox); // Создайте новый экземпляр для каждого сообщения
                string formattedMessage = data.Nickname + ": " + message;
                newMessageBox.SetText(formattedMessage);

                // Примените стиль в зависимости от ID
                if (data.ID == User.Local.Data.ID)
                    newMessageBox.SetStyleBySelf(); // Стиль для собственного сообщения
                else
                    newMessageBox.SetStyleBySender(); // Стиль для чужого сообщения

                // Установите родителя и масштаб только если сообщение действительно будет отображено
                newMessageBox.transform.SetParent(messagePanel);
                newMessageBox.transform.localScale = Vector3.one; // Убедитесь, что размер в родительском объекте
            }
            
            /*  UIMessageBox newMessageBox = Instantiate(messageBox); // Создайте новый экземпляр для каждого сообщения
            
            // Если это не приватное сообщение
            if (string.IsNullOrEmpty(privateMessage))
            {
                string formattedMessage = nickname + ": " + message;
                newMessageBox.SetText(formattedMessage);
        
                // Примените стиль в зависимости от ID
                if (id == User.Local.Data.ID)
                    newMessageBox.SetStyleBySelf(); // Стиль для собственного сообщения
                else
                    newMessageBox.SetStyleBySender(); // Стиль для чужого сообщения
            }
            else
            {
                // Если это приватное сообщение, проверяем, является ли оно для локального пользователя
                if (privateMessage == User.Local.Data.Nickname || id == User.Local.Data.ID)
                {
                    string formattedMessage = nickname + ": " + message;
                    newMessageBox.SetText(formattedMessage);
                    // Примените стиль в зависимости от ID
                    if (id == User.Local.Data.ID)
                        newMessageBox.SetStyleBySelf(); // Стиль для собственного сообщения
                    else
                        newMessageBox.SetStyleBySender(); // Стиль для чужого сообщения
                }
                // Если это приватное сообщение, и оно не предназначено для локального пользователя, вы не должны ничего отображать
            }

            newMessageBox.transform.SetParent(messagePanel);
            newMessageBox.transform.localScale = Vector3.one; // Убедитесь, что размер в родительском объекте
            */
            
            /*if (string.IsNullOrEmpty(privateMessage))
            {
                box = Instantiate(messageBox);
                box.SetText(nickname + ": " + message);
                box.transform.SetParent(messagePanel);
                box.transform.localScale = Vector3.one;

                if (id == User.Local.Data.ID)
                    messageBox.SetStyleBySelf();
                else
                    messageBox.SetStyleBySender();
            }
            else
            {
                // Если privateMessage не пустое, проверяем, является ли это сообщение для локального пользователя
                if (privateMessage == User.Local.Data.Nickname || id == User.Local.Data.ID)
                {
                    box1 = Instantiate(messageBox);
                    box1.SetText(nickname + ": " + message);
                    box1.transform.SetParent(messagePanel);
                    box1.transform.localScale = Vector3.one;

                    if (id == User.Local.Data.ID)
                        box1.SetStyleBySelf();
                    else
                        box1.SetStyleBySender();
                }
            }*/
        }

        private void OnUpdateUserList(List<UserData> userList)
        {
            for (int i = 0; i < userListPanel.childCount; i++)
            {
                Destroy(userListPanel.GetChild(i).gameObject);
            }

            for (int i = 0; i < userList.Count; i++)
            {
                UIMessageBox userbox = Instantiate(userBox);
                userbox.SetText(userList[i].Nickname);
                userbox.transform.SetParent(userListPanel);
                userbox.transform.localScale = Vector3.one;
            }
        }
    }
}