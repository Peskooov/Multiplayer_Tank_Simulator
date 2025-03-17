using TMPro;
using UnityEngine;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Update()
    {
        if(Player.Local == null) return;
        if(Player.Local.ActiveVehicle == null) return;

        text.text = Player.Local.ActiveVehicle.HitPoint.ToString();
    }
}
