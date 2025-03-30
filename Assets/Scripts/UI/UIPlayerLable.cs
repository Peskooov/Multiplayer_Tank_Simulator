using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerLable : MonoBehaviour
{
    [SerializeField] private TMP_Text fragsText;
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private Image bgImage;
    [SerializeField] private Color selfColor;

    private int netID;
    public int NetID => netID;

    public void Init(int netID, string nickname)
    {
        this.netID = netID;
        nicknameText.text = nickname;

        if (netID == Player.Local.netId)
        {
            bgImage.color = selfColor;
        }
    }

    public void UpdateFrags(int frag)
    {
        fragsText.text = frag.ToString();
    }
}
