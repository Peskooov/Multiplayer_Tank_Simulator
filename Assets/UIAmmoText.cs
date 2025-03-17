using UnityEngine;
using TMPro;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Update()
    {
        if(Player.Local == null) return;
        if(Player.Local.ActiveVehicle == null) return;

        text.text = Player.Local.ActiveVehicle.Turret.AmmoCount.ToString();
    }
}
