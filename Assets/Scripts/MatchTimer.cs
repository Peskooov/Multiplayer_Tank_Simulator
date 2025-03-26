using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class MatchTimer : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private float matchTime;

    [SyncVar] private float timeLeft;
    public float TimeLeft => timeLeft;

    public UnityAction<float> OnTimeLeft;
    
    private bool isTimerEnd;

    public bool IsTriggered => isTimerEnd;

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();
    }
    
    public void OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    private void Start()
    {
        if (isServer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (isServer)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0;

                isTimerEnd = true;
            }
        }
    }
    
    private void Reset()
    {
        enabled = true;
        timeLeft = matchTime;
        OnTimeLeft?.Invoke(timeLeft);
        isTimerEnd = false;
    }
}
