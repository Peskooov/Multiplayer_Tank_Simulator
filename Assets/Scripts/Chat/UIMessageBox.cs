using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkChat
{
    public class UIMessageBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color backgroundColorForSelf;
        [SerializeField] private Color backgroundColorForSender;
        [SerializeField] private VerticalLayoutGroup layoutGroup;

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public void SetStyleBySelf()
        {
            backgroundImage.color = backgroundColorForSelf;
            //text.alignment = TextAlignmentOptions.MidlineLeft;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
        }
        
        public void SetStyleBySender()
        {
            backgroundImage.color = backgroundColorForSender;
            //text.alignment = TextAlignmentOptions.MidlineRight;
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
        }
    }
}