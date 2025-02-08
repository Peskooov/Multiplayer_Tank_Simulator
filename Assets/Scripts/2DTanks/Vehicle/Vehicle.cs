using System;
using DG.Tweening;
using UnityEngine;
using Mirror;

namespace Tanks2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Vehicle : Destructible
    {
        /// <summary>
        /// Масса для автоматической установки у ригида.
        /// </summary>
        [Header("Space ship")] [SerializeField]
        private float m_Mass;

        [SerializeField] private bool isShip;
        [SerializeField] private Transform transformToRotate;

        /// <summary>
        /// Толкающая вперед сила.
        /// </summary>
        [SerializeField] private float m_Thrust;

        /// <summary>
        /// Вращающая сила.
        /// </summary>
        [SerializeField] private float m_Mobility;

        /// <summary>
        /// Максимальная линейная скорость.
        /// </summary>
        [SerializeField] private float m_MaxLinearVelocity;

        /// <summary>
        /// Максимальная вращательная скорость. В градусах/сек
        /// </summary>
        [SerializeField] private float m_MaxAngularVelocity;

        /// <summary>
        /// Сохраненная ссылка на ригид.
        /// </summary>
        private Rigidbody2D m_Rigid;

        [SerializeField] private Turret m_Turret;
        [SerializeField] private Turret[] m_RocketTurrets;

        [SerializeField] private float offset;

        private Camera _camera;

        #region Public API

        /// <summary>
        /// Управление линейной тягой. -1.0 до +1.0
        /// </summary>
        public float ThrustControl { get; set; }

        /// <summary>
        /// Управление вращательной тягой. -1.0 до +1.0
        /// </summary>
        public float TorqueControl { get; set; }

        #endregion

        #region Unity Event

        private void Start()
        {
            _camera = FindObjectOfType<Camera>();

            m_Rigid = GetComponent<Rigidbody2D>();
            m_Rigid.mass = m_Mass;

            m_Rigid.inertia = 1;
        }


        private void FixedUpdate()
        {
            if (isOwned || netIdentity.connectionToClient == null)
            {
                UpdateRigidBody();
            }
        }

        private void Update()
        {
            if (isOwned || netIdentity.connectionToClient == null)
            {
                MouseRotate();
            }
        }

        #endregion

        /// <summary>
        /// Метод добавления сил кораблю для движения
        /// </summary>
        private void UpdateRigidBody()
        {
            m_Rigid.AddForce(ThrustControl * m_Thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);

            m_Rigid.AddForce(-m_Rigid.velocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime,
                ForceMode2D.Force);

            if (!isShip)
            {
                m_Rigid.AddTorque(TorqueControl * m_Mobility * Time.fixedDeltaTime, ForceMode2D.Force);
                m_Rigid.AddTorque(-m_Rigid.angularVelocity * (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime,
                    ForceMode2D.Force);
            }
        }

        private void MouseRotate()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10f;

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector3 rotation = mouseWorldPosition - transform.position;

            float rotateZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transformToRotate.rotation = Quaternion.Euler(0, 0, rotateZ - 90);
        }

        public void Fire()
        {
            m_Turret.CmdFire();
        }

        public void RocketFire()
        {
            foreach (var turret in m_RocketTurrets)
            {
                turret.CmdFire();
            }
        }
    }
}