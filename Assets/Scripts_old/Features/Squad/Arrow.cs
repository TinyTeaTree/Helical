using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ChessRaid
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] AnimationCurve _hightCurve;
        [SerializeField] AnimationCurve _easeCurve;

        private bool _isFlying;

        public async Task FlyOff(Coord toLocation)
        {
            _isFlying = true;
            var r = StartCoroutine(FlyRoutine(toLocation));
            StartCoroutine(LookAtFlyDirection());

            await TaskUtils.WaitUntil(() => !_isFlying);
        }

        private IEnumerator FlyRoutine(Coord toLocation)
        {
            var pos = GridManager.Single.GetHex(toLocation).transform.position;
            pos += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, UnityEngine.Random.Range(-0.2f, 0.2f));

            var from = transform.position;

            var distance = (pos - from).magnitude;

            var duration = _speed == 0 ? 5f : distance / _speed;

            float passed = 0f;

            while(passed < duration)
            {
                passed += Time.deltaTime;

                var ratio = passed / duration;
                ratio = _easeCurve.Evaluate(ratio);

                var lerpedPos = Vector3.Lerp(from, pos, ratio);
                var withHight = lerpedPos + new Vector3(0f, _hightCurve.Evaluate(ratio), 0f);

                transform.position = withHight;

                yield return null;
            }

            _isFlying = false;
            yield break;
        }

        private IEnumerator LookAtFlyDirection()
        {
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition;

            yield return null;

            while (_isFlying)
            {
                nextPosition = transform.position;
                var delta = nextPosition - currentPosition;

                transform.LookAt(nextPosition + delta);
                currentPosition = nextPosition;

                yield return null;
            }
        }
    }
}