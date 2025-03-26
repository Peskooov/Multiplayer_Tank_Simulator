using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer matchTimer; 
    [SerializeField] private TMP_Text timerText;

    private IEnumerator cor;
    
    private void Start()
    {
        matchTimer.OnTimeLeft += StartTimer;
    }

    private void OnDestroy()
    {
        matchTimer.OnTimeLeft -= StartTimer;
    }

    private void StartTimer(float seconds)
    {
        if (cor != null)
            cor = null;

        cor = Timer(seconds);
        StartCoroutine(cor);
    }

    private IEnumerator Timer(float sec)
    {
        while (sec > 0)
        {
            yield return new WaitForSeconds(1);
            sec -= 1;
            timerText.text = sec.ToString();
        }
        
        timerText.text = "0";
        cor = null;
    }
}
