using System;
using UnityEngine;

namespace ChessRaid
{
    public class CameraView : WagMonoton<CameraView>
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private float _moveSpeed;

        private Vector2 _previousPosition;
        private bool _isDragging;

        Vector3 _zeroDelta;

        private void Start()
        {
            var anchor = LevelView.Single.CameraAnchor;
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;

            CalculateZeroPoint();
        }

        private void CalculateZeroPoint()
        {
            var forwardDelta = transform.forward;

            var forwardYDelta = -forwardDelta.y;

            var forwardsAmountNeeded = transform.position.y / forwardYDelta;

            var pointAtZero = forwardDelta * forwardsAmountNeeded + transform.position;
            _zeroDelta = pointAtZero - transform.position;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                CheckMoveCamera();
                _isDragging = true;
            }
            else
            {
                _isDragging = false;
            }
        }

        private void CheckMoveCamera()
        {
            Vector2 mousePosition = Input.mousePosition;

            if (!_isDragging)
            {
                _previousPosition = mousePosition;
                return;
            }

            var delta = mousePosition - _previousPosition;
            var normalizedDelta = new Vector2(delta.x / Screen.width, delta.y / Screen.height);

            transform.position -= new Vector3(normalizedDelta.x, 0f, normalizedDelta.y) * _moveSpeed;

            _previousPosition = mousePosition;

            Clamp();
        }

        private void Clamp()
        {
            var zeroPoint = transform.position + _zeroDelta;

            var level = LevelView.Single;

            if (zeroPoint.x < level.LeftBoundry)
            {
                transform.position += new Vector3(level.LeftBoundry - zeroPoint.x, 0, 0);
            }
            else if (zeroPoint.x > level.RightBoundry)
            {
                transform.position -= new Vector3(zeroPoint.x - level.RightBoundry, 0, 0);
            }

            if (zeroPoint.z < level.DownBoundry)
            {
                transform.position += new Vector3(0, 0, level.DownBoundry - zeroPoint.z);
            }
            else if (zeroPoint.z > level.UpBoundry)
            {
                transform.position -= new Vector3(0, 0, zeroPoint.z - level.UpBoundry);
            }
        }
    }
}