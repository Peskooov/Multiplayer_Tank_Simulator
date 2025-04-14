using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAmmunitionElement : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoCountText;
    [SerializeField] private Image projectileIco;
    [SerializeField] private GameObject selectionBorder;

    public void SetAmmunition(Ammunition ammunition)
    {
        projectileIco.sprite = ammunition.ProjectileProp.Icon;
        UpdateAmmoCount(ammunition.AmmoCount);
    }

    public void UpdateAmmoCount(int count)
    {
        ammoCountText.text = count.ToString();
    }

    public void Select()
    {
        selectionBorder.SetActive(true);
    }

    public void UnSelect()
    {
        selectionBorder.SetActive(false);
    }
}
