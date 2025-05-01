using TMPro;
using UnityEngine;

public class UITankInfo : MonoBehaviour
{
    [SerializeField] private UIHealthSlider healthSlider;
    [SerializeField] private Vector3 worldOffset;
    public Vector3 WorldOffset => worldOffset; 
    
    private Vehicle tank;
    public Vehicle Tank => tank;

    public void SetTank(Vehicle vehicle)
    {
        tank = vehicle;
        
        healthSlider.Init(tank.TeamID, Player.Local.TeamID);
    }
}
