using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderImages;

    [SerializeField] private Color localTeamColor;
    [SerializeField] private Color otherTeamColor;

    private Destructible destructible;

    public void Init(Destructible destructible, int destructibleTeamID, int localPlayerTeamID)
    {
        this.destructible = destructible;

        destructible.HitPointChanged += OnHitPointChanged;
        slider.maxValue = destructible.MaxHitPoint;
        slider.value = slider.maxValue;

        if (localPlayerTeamID == destructibleTeamID)
        {
            SetLocalTeamColor();
        }
        else
        {
            SetOtherTeamColor();
        }
    }

    private void OnDestroy()
    {
        if (destructible == null) return;

        destructible.HitPointChanged -= OnHitPointChanged;
    }

    private void OnHitPointChanged(int value)
    {
        slider.value = destructible.HitPoint;
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