
using Mirror;
using UnityEngine;

public enum AIBehaviourType
{
    Patrol,
    Support,
    InvaderBase
}

[RequireComponent(typeof(NetworkIdentity))]
public class TankAI : NetworkBehaviour
{
    [SerializeField] private AIBehaviourType behaviourType;

    [Range(0, 1)]
    [SerializeField] private float patrolChance;
    [Range(0, 1)]
    [SerializeField] private float supportChance; 
    [Range(0, 1)]
    [SerializeField] private float invaderBaseChance;
    
    [SerializeField] private Vehicle vehicle;
    [SerializeField] private AIMovement movement;
    [SerializeField] private AIShooter shooter;

    private Vehicle fireTarget;
    private Vector3 movementTarget;
    private int countTeamMember;
    private int startCountTeamMember;
    
    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        vehicle.Destroyed += OnVehicleDestroyed;

        movement.enabled = false;
        shooter.enabled = false;
        
        CalcTeamMember();
        SetStartBehaviour();
    }

    private void Update()
    {
        if(isServer)
            UpdateBehaviour();
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        vehicle.Destroyed -= OnVehicleDestroyed;
    }

    private void OnMatchStart()
    {
        movement.enabled = true;
        shooter.enabled = true;
    }

    private void OnVehicleDestroyed(Destructible destructible)
    {
        movement.enabled = false;
        shooter.enabled = false;
    }
    
    private void SetStartBehaviour()
    {
        float totalChance = patrolChance + supportChance + invaderBaseChance;
        float chance = Random.Range(0.0f, totalChance);

        if (chance <= patrolChance)
        {
            StartBehaviour(AIBehaviourType.Patrol);
            return;
        }

        if (chance <= patrolChance + supportChance)
        {
            StartBehaviour(AIBehaviourType.Support);
            return;
        }

        StartBehaviour(AIBehaviourType.InvaderBase);
    }

    #region Behaviour

    private void StartBehaviour(AIBehaviourType type)
    {
        behaviourType = type;

        switch (behaviourType)
        {
            case AIBehaviourType.InvaderBase:
                movementTarget = AIPath.Instance.GetBasePoint(vehicle.TeamID);
                break;
            
            case AIBehaviourType.Patrol:
                movementTarget = AIPath.Instance.GetRandomPatrolPoint();
                break;
            
            case AIBehaviourType.Support:
                movementTarget = AIPath.Instance.GetRandomFirePoint(vehicle.TeamID);
                break;
        }

        movement.ResetPath();
    }

    private void OnReachedDestination()
    {
        if (behaviourType == AIBehaviourType.Patrol)
        {
            movementTarget = AIPath.Instance.GetRandomPatrolPoint();
            movement.ResetPath();
            movement.SetDestination(movementTarget); // Явный вызов.
        }
    }
    
    private void UpdateBehaviour()
    {
        shooter.FindTarget();
        
        if (movement.ReachedDestination)
        {
            OnReachedDestination();
        }

        if (!movement.HasPath)
        {
            movement.SetDestination(movementTarget);
        }
    }
    
    private void CalcTeamMember()
    {
        Vehicle[] v = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < v.Length; i++)
        {
            if (v[i].TeamID == vehicle.TeamID)
            {
                if (v[i] != vehicle)
                {
                    startCountTeamMember++;
                    v[i].Destroyed += OnTeamMemberDestroyed;
                }
            }
        }

        countTeamMember = startCountTeamMember;
    }

    private void OnTeamMemberDestroyed(Destructible dest)
    {
        countTeamMember--;
        dest.Destroyed -= OnTeamMemberDestroyed;
        
        if ((float)countTeamMember / (float)startCountTeamMember < 0.4f || countTeamMember <= 2)
        {
            StartBehaviour(AIBehaviourType.Patrol);
        }
    }

    #endregion
}