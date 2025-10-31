using System.Threading.Tasks;
using UnityEngine;

namespace ChessRaid
{
    public class ArcherChampion : Champion
    {
        [SerializeField] private AnimationEventCapture _eventCapture;
        [SerializeField] private Arrow _arrowOrigin;
        [SerializeField] private Transform _arrowParent;

        private bool _pendingShoot;

        public override async Task Attack(Coord location)
        {
            _animator.SetTrigger("Attack");

            _pendingShoot = true;
            _eventCapture.SetCallback(nameof(ShootArrow), ShootArrow);

            while (this != null && _pendingShoot)
            {
                await Task.Delay(1);
            }

            var arrow = Instantiate(_arrowOrigin, _arrowParent);
            arrow.transform.localPosition = Vector3.zero;
            arrow.transform.localRotation = Quaternion.identity;
            GridManager.Single.AttachArrow(arrow);
            arrow.gameObject.SetActive(true);

            await arrow.FlyOff(location);
        }

        public void ShootArrow()
        {
            _eventCapture.RemoveCallback(nameof(ShootArrow));
            _pendingShoot = false;
        }
    }
}