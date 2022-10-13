using UnityEngine;

// https://catlikecoding.com/unity/tutorials/movement/orbit-camera/

// ReSharper disable once CheckNamespace
namespace RollABall.Controllers
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollowController : MonoBehaviour
    {
        #region Links

        [SerializeField] private Transform focus;
        [SerializeField, Range(1f, 20f)] private float distance = 12f;
        [SerializeField, Min(0f)] private float focusRadius = 1f;
        [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;

        [Header("Rendering")] [SerializeField] private RenderTexture renderTexture;
        
        #endregion

        #region Fields

        private Camera _camera;
        private Vector3 focusPoint;
        
        #endregion
        
        #region MonoBehaviour metods
        
        private void Awake ()
        {
            _camera = GetComponent<Camera>();
            focusPoint = focus.position;
        }

        private void Start()
        {
            if (renderTexture is not null)
            {
                _camera.targetTexture = renderTexture;
            }
        }


        private void LateUpdate()
        {
            UpdateFocusPoint();
            var lookDirection = transform.forward;
            // ReSharper disable once Unity.InefficientPropertyAccess
            transform.localPosition = focusPoint - lookDirection * distance;
        }
        
        #endregion
        
        #region Functionality

        private void UpdateFocusPoint()
        {
            var targetPoint = focus.position;

            if (focusRadius > 0f)
            {
                var dist = Vector3.Distance(targetPoint, focusPoint);
                var t = 1f;

                if (dist > 0.01f && focusCentering > 0f)
                {
                    t = Mathf.Pow(1f - focusCentering, Time.deltaTime);
                }

                if (dist > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / dist);
                }

                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            }
            else
            {
                focusPoint = targetPoint;
            }
        }

        #endregion
    }
}

