using UnityEngine;

namespace Tanks2D
{
    public class VehicleCamera : MonoBehaviour
    {
        public static VehicleCamera Instance;

        [SerializeField] private Camera m_Camera;

        [SerializeField] private float m_InterpolationLinear;

        [SerializeField] private float m_InterpolationAngular;

        [SerializeField] private float m_CameraZOffset;

        [SerializeField] private float m_ForwardOffset;

        private Transform m_Target;

        public Camera Camera => m_Camera;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void FixedUpdate()
        {
            if (m_Target == null || m_Camera == null) return;

            Vector2 camPos = m_Camera.transform.position;
            Vector2 targetPos = m_Target.position + m_Target.up * m_ForwardOffset;

            Vector2 newCamPos = Vector2.Lerp(camPos, targetPos, m_InterpolationLinear * Time.deltaTime);

            m_Camera.transform.position = new Vector3(newCamPos.x, newCamPos.y, m_CameraZOffset);

            if (m_InterpolationAngular > 0)
            {
                m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation,
                    m_Target.rotation, m_InterpolationAngular * Time.deltaTime);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            m_Target = newTarget;
        }
    }
}