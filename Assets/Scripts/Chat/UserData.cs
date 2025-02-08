
using Mirror;

namespace NetworkChat
{
    public class UserData
    {
        public int ID;
        public string Nickname;
        public string PrivateMessage;

        public UserData(int id, string nickname, string privateMessage)
        {
            ID = id;
            Nickname = nickname;
            PrivateMessage = privateMessage;
        }
        
        public UserData(int id, string nickname)
        {
            ID = id;
            Nickname = nickname;
        }
    }

    public static class UserDataWriteRead
    {
        public static void WriteUserData(this NetworkWriter writer, UserData data)
        {
            writer.WriteInt(data.ID);
            writer.WriteString(data.Nickname);
            writer.WriteString(data.PrivateMessage);
        }
        
        public static UserData ReadUserData(this NetworkReader reader)
        {
            return new UserData(reader.ReadInt(), reader.ReadString(),reader.ReadString());
        }
    }
}