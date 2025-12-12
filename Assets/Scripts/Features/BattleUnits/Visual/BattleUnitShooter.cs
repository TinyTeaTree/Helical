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
        [Tooltip("The easing function for projectile movement animation")]
        [SerializeField] private Ease _projectileEase = Ease.Linear;

        /// <summary>
        /// Shoots a projectile towards the target position using configured speed.
        /// Travel time is calculated based on distance and projectile speed.
        /// <summary>
        /// Shoots a projectile towards the target position with the specified duration.
        /// </summary>
        /// <param name="targetPosition">The 3D world position to shoot the projectile towards</param>
        /// <param name="duration">How long in seconds the projectile flight should take</param>
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
        /// <summary>
        /// Shoots a projectile towards a target hex coordinate with the specified duration.
        /// </summary>
        /// <param name="fromCoordinate">The shooter's coordinate (unused, kept for API consistency)</param>
        /// <param name="targetCoordinate">The target coordinate</param>
        /// <param name="duration">How long in seconds the projectile flight should take</param>
        public async UniTask ShootTowardsCoordinate(Vector2Int fromCoordinate, Vector2Int targetCoordinate, float duration)
        {
            // Convert target coordinate to world position
            Vector3 targetWorldPosition = GridUtils.ToWorldX0Z(targetCoordinate);
            await ShootTowardsPosition(targetWorldPosition, duration);
        }
    }
}
