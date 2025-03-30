using UnityEngine;
using UnityEngine.UI;

public class UICaptureBase : MonoBehaviour
{
    [SerializeField] private ConditionCaptureBase conditionCapture;
    
    [SerializeField] private Slider localSlider;
    [SerializeField] private Slider otherSlider;

    private void Update()
    {
        if(Player.Local == null) return;

        if (Player.Local.TeamID == TeamSide.TeamRed)
        {
            UpdateSlider(localSlider, conditionCapture.RedBaseCaptureLevel);
            UpdateSlider(otherSlider, conditionCapture.BlueBaseCaptureLevel);
        } 
        
        if (Player.Local.TeamID == TeamSide.TeamBlue)
        {
            UpdateSlider(localSlider, conditionCapture.BlueBaseCaptureLevel);
            UpdateSlider(otherSlider, conditionCapture.RedBaseCaptureLevel);
        } 
    }

    private void UpdateSlider(Slider slider, float value)
    {
        if (value == 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if (slider.gameObject.activeSelf == false)
            {
                slider.gameObject.SetActive(true);
            }
            
            slider.value = value;
        }
    }
}
