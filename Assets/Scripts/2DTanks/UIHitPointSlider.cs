using UnityEngine;
using UnityEngine.UI;

namespace Tanks2D
{
    public class UIHitPointSlider : MonoBehaviour
    {
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private Slider slider;
        [SerializeField] private Image fillImage;

        private void Start()
        {
            vehicle.HitPointChange += OnHitPointChange;

            Color color = vehicle.Owner.GetComponent<Player>().PlayerColor;
            fillImage.color = new Color(color.r, color.g, color.b);

            slider.maxValue = vehicle.MaxHitPoints;
            slider.value = vehicle.HitPoint;
        }

        private void OnDestroy()
        {
            vehicle.HitPointChange -= OnHitPointChange;
        }

        private void OnHitPointChange(int hitPoint)
        {
            slider.value = hitPoint;
        }
    }
}