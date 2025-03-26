using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TrackWheelRow
{
    [SerializeField] private WheelCollider[] colliders;
    [SerializeField] private Transform[] meshes;

    public float minRpm;
    
    public void SetTorque( float motorTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].motorTorque = motorTorque;
        }
    }
    
    public void Break(float breakTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = breakTorque;
        }
    }

    public void UpdateMeshTransform()
    {
        //Find min RPM
        List<float> allRPM = new List<float>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].isGrounded)
            {
                allRPM.Add(colliders[i].rpm);
            }
        }

        if (allRPM.Count > 0)
        {
            minRpm = Mathf.Abs(allRPM[0]);

            for (int i = 0; i < allRPM.Count; i++)
            {
                if (Mathf.Abs(allRPM[i]) < minRpm)
                {
                    minRpm = Mathf.Abs(allRPM[i]);
                }
            }

            minRpm = minRpm * Mathf.Sign(allRPM[0]);
        }

        float angle = minRpm * 360 / 60 * Time.fixedDeltaTime;
        
        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;
            
            colliders[i].GetWorldPose(out position, out rotation);

            meshes[i].position = position;
            meshes[i].Rotate(angle,0,0);
        }
    }

    public void SetSidewayStiffness(float stiffness)
    {
        WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

        for (int i = 0; i < colliders.Length; i++)
        {
            wheelFrictionCurve = colliders[i].sidewaysFriction;
            wheelFrictionCurve.stiffness = stiffness;

            colliders[i].sidewaysFriction = wheelFrictionCurve;
        }
    }
    
    public void Reset()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].motorTorque = 0;
            colliders[i].brakeTorque = 0;
        }
    }

    public void UpdateMeshRotationByRPM(float rpm)
    {
        float angle = rpm * 360 / 60 * Time.fixedDeltaTime;

        for (int i =0; i < meshes.Length; i++)
        {
            Vector3 pos;
            Quaternion rot;
            
            colliders[i].GetWorldPose(out pos, out rot);

            meshes[i].position = pos;
            meshes[i].Rotate(angle,0,0);
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class TrackTank : Vehicle
{
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject destroyedTank;
    [SerializeField] private Transform centerOfMass;
    
    [Header("Traks")] 
    [SerializeField] private TrackWheelRow leftWheelRow;
    [SerializeField] private TrackWheelRow rightWheelRow;

    [Header("Movement")] 
    [SerializeField] private ParameterCurve forwardTorqueCurve;
    [SerializeField] private float maxForwardMotorTorque;
    [SerializeField] private ParameterCurve backwardTorqueCurve;
    [SerializeField] private float maxBackwardMotorTorque;
    [SerializeField] private float breakTorque;
    [SerializeField] private float rollingResistance;

    [Header("Rotation")] 
    [SerializeField] private float rotateTorqueInPlace;
    [SerializeField] private float rotateBreakInPlace;
    [Space(3)] 
    [SerializeField] private float rotateTorqueInMotion;
    [SerializeField] private float rotateBreakInMotion;

    [Header("Friction")] 
    [SerializeField] private float minSidewayStiffnessInPlace;
    [SerializeField] private float minSidewayStiffnessInMotion;
    
    public override float LinearVelocity => rigidBody.velocity.magnitude;
    public float currentMotorTorque;

    public float LeftWheelRPM => leftWheelRow.minRpm;
    public float RightWheelRPM => rightWheelRow.minRpm;
    
    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        if (isOwned)
        {
            UpdateMotorTorque();
            
            CmdUpdateWheelRpm(LeftWheelRPM,RightWheelRPM);
        }
    }

    [Command]
    private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        SvUpdateWheelRpm(leftRpm,rightRpm);
    }

    [Server]
    private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        RpcUpdateWheelRpm(leftRpm, rightRpm);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        leftWheelRow.minRpm = leftRpm;
        rightWheelRow.minRpm = rightRpm;

        leftWheelRow.UpdateMeshRotationByRPM(leftRpm);
        rightWheelRow.UpdateMeshRotationByRPM(rightRpm);
    }
    
    
    private void UpdateMotorTorque()
    {
        float targetMotorTorque = targetInputControl.z>0 ? maxForwardMotorTorque * Mathf.RoundToInt(targetInputControl.z) : maxBackwardMotorTorque * Mathf.RoundToInt(targetInputControl.z);
        float breakTorque = this.breakTorque * targetInputControl.y;
        float steering = targetInputControl.x;
        
        //Update target motor torque
        if (targetMotorTorque > 0)
        {
            currentMotorTorque = forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }
        if (targetMotorTorque < 0)
        {
            currentMotorTorque = backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque == 0)
        {
            currentMotorTorque = forwardTorqueCurve.Reset();
            currentMotorTorque = backwardTorqueCurve.Reset();
        }

        //Break
        leftWheelRow.Break(breakTorque);
        rightWheelRow.Break(breakTorque);
        
        //Rolling
        if (targetMotorTorque == 0 && steering == 0)
        {
            leftWheelRow.Break(rollingResistance);
            rightWheelRow.Break(rollingResistance);
        }
        else
        {
            leftWheelRow.Reset();
            rightWheelRow.Reset();
        }

        //Rotate in place
        if (targetMotorTorque == 0 && steering != 0)
        {
            if (Mathf.Abs(leftWheelRow.minRpm) <1 || Mathf.Abs(rightWheelRow.minRpm) <1)
            {
                leftWheelRow.SetTorque(rotateTorqueInPlace);
                rightWheelRow.SetTorque(rotateTorqueInPlace);
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInPlace);
                    rightWheelRow.SetTorque(rotateTorqueInPlace);
                }

                if (steering > 0)
                {
                    rightWheelRow.Break(rotateBreakInPlace);
                    leftWheelRow.SetTorque(rotateTorqueInPlace);
                }
            }

            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
        }
        
        //Move
        if (targetMotorTorque != 0)
        {
            if (steering==0)
            {
                if (LinearVelocity < maxLinearVelocity)
                {
                    leftWheelRow.SetTorque(currentMotorTorque);
                    rightWheelRow.SetTorque(currentMotorTorque);
                }
            }
            
            if (steering != 0 && (Mathf.Abs(leftWheelRow.minRpm) <1 || Mathf.Abs(rightWheelRow.minRpm) <1))
            {
                leftWheelRow.SetTorque(rotateTorqueInMotion);
                rightWheelRow.SetTorque(rotateTorqueInMotion);
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInMotion);
                    rightWheelRow.SetTorque(rotateTorqueInMotion);
                }

                if (steering > 0)
                {
                    rightWheelRow.Break(rotateBreakInMotion);
                    leftWheelRow.SetTorque(rotateTorqueInMotion);
                }
            }
            
            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
        }
        
        
        leftWheelRow.UpdateMeshTransform();
        rightWheelRow.UpdateMeshTransform();
    }

    protected override void OnDestructibleDestroy()
    {
        base.OnDestructibleDestroy();

        GameObject ruinedTank = Instantiate(destroyedTank);
        ruinedTank.transform.position = visualModel.transform.position;
        ruinedTank.transform.rotation = visualModel.transform.rotation;
        
        visualModel.SetActive(false);
    }
}