using UnityEngine;

namespace ChessRaid
{
    public class LevelView : WagMonoton<LevelView>
    {
        [SerializeField] private Transform _leftBoundry;
        [SerializeField] private Transform _rightBoundry;
        [SerializeField] private Transform _upBoundry;
        [SerializeField] private Transform _downBoundry;

        [SerializeField] private Transform _cameraAnchor;

        public float LeftBoundry => _leftBoundry.position.x;
        public float RightBoundry => _rightBoundry.position.x;
        public float UpBoundry => _upBoundry.position.z;
        public float DownBoundry => _downBoundry.position.z;

        public Transform CameraAnchor => _cameraAnchor;
    }
}