using UnityEngine;
using TMPro;

public class UIHitResultPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text damageText;

    public void SetTypeResult(string result)
    {
        typeText.text = result;
    }

    public void SetDamageResult(float damageAmount)
    {
        if (damageAmount <= 0)
            damageText.text = string.Empty;
        else
            damageText.text = "-" + damageAmount.ToString("F0");
    }
}
