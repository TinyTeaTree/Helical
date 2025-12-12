using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class BattleUnitShooter : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _shootOrigin;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _projectileLifetime = 3f;
        [SerializeField] private Ease _projectileEase = Ease.Linear;

        /// <summary>
        /// Shoots a projectile towards the target position
        /// </summary>
        /// <param name="targetPosition">The world position to shoot towards</param>
        /// <param name="duration">How long the shooting animation should take</param>
        public async UniTask ShootTowardsPosition(Vector3 targetPosition, float duration)
        {
            if (_projectilePrefab == null || _shootOrigin == null)
            {
                Debug.LogWarning("BattleUnitShooter: Projectile prefab or shoot origin not set!");
                return;
            }

            // Instantiate the projectile
            GameObject projectile = Instantiate(_projectilePrefab, _shootOrigin.position, Quaternion.identity);

            // Calculate direction to target
            Vector3 direction = (targetPosition - _shootOrigin.position).normalized;

            // Rotate projectile to face target direction
            if (direction != Vector3.zero)
            {
                projectile.transform.rotation = Quaternion.LookRotation(direction);
            }

            // Move projectile towards target
            Vector3 startPosition = projectile.transform.position;
            Tween projectileTween = projectile.transform.DOMove(targetPosition, duration)
                .SetEase(_projectileEase);

            // Wait for the projectile to reach its target or timeout
            await projectileTween.AsyncWaitForCompletion();

            // Destroy the projectile after a short delay
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));
            if (projectile != null)
            {
                Destroy(projectile);
            }
        }

        /// <summary>
        /// Shoots a projectile towards a target coordinate
        /// </summary>
        /// <param name="fromCoordinate">The shooter's coordinate</param>
        /// <param name="targetCoordinate">The target coordinate</param>
        /// <param name="duration">How long the shooting animation should take</param>
        public async UniTask ShootTowardsCoordinate(Vector2Int fromCoordinate, Vector2Int targetCoordinate, float duration)
        {
            // Convert target coordinate to world position
            Vector3 targetWorldPosition = GridUtils.ToWorldX0Z(targetCoordinate);
            await ShootTowardsPosition(targetWorldPosition, duration);
        }
    }
}
