using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthSlider : MonoBehaviour
{
    [SerializeField] private UIHealthText uiHealthText;
    
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderImages;

    [SerializeField] private Color localTeamColor;
    [SerializeField] private Color otherTeamColor;

    private Destructible destructible;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }
    
    private void OnDestroy()
    {
        if (destructible != null)
            destructible.HitPointChanged -= OnHitPointChanged;
    }
    
    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        destructible = vehicle;

        destructible.HitPointChanged += OnHitPointChanged;
        slider.maxValue = destructible.MaxHitPoint;
        slider.value = destructible.MaxHitPoint;
        
        Debug.Log(destructible.HitPoint + "/" + destructible.MaxHitPoint);
    }

    private void OnHitPointChanged(int hitPoint)
    {
        Debug.Log(hitPoint);
        slider.value = hitPoint;
    }
    
    public void Init(int destructibleTeamID, int localPlayerTeamID)
    {
        if (localPlayerTeamID == destructibleTeamID)
        {
            SetLocalTeamColor();
        }
        else
        {
            SetOtherTeamColor();
        }
    }

    private void SetLocalTeamColor()
    {
        sliderImages.color = localTeamColor;
    }

    private void SetOtherTeamColor()
    {
        sliderImages.color = otherTeamColor;
    }
}