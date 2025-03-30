using UnityEngine;
using UnityEngine.UI;

public class UITankMark : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private Color localTeamColor;
    [SerializeField] private Color otherTeamColor;

    public void SetLocalTeamColor()
    {
        image.color = localTeamColor;
    }

    public void SetOtherTeamColor()
    {
        image.color = otherTeamColor;
    }
}
