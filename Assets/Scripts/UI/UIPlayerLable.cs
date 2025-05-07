using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerLable : MonoBehaviour
{
    [SerializeField] private TMP_Text fragsText;
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private Image bgImage;
    [SerializeField] private Color selfColor;
    [SerializeField] private Color disableColor;

    private int netID;
    public int NetID => netID;

    public void Init(int netID, string nickname, Destructible dest)
    {
        this.netID = netID;
        nicknameText.text = nickname;
        
        if (dest != null)
        {
            dest.Destroyed += DisabledLable;
        }
        
        if (netID == Player.Local.netId)
        {
            bgImage.color = selfColor;
        }
    }

    public void UpdateFrags(int frag)
    {
        fragsText.text = frag.ToString();
    }

    private void DisabledLable(Destructible destructible)
    {
        bgImage.color = disableColor;
        destructible.Destroyed -= DisabledLable;
    }
}
