using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAnimator))]
    public class AnimateAlongAgent : MonoBehaviour
    {
        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private EnemyAnimator _animator;
        [SerializeField]
        private float _minimalVelocity = 0.1f;

        private void Update()
        {
            if(IsShouldMove())
            {
                _animator.Move(_agent.velocity.magnitude);
            }
            else
            {
                _animator.StopMoving();
            }
        }

        private bool IsShouldMove() =>
            _agent.velocity.magnitude > _minimalVelocity && _agent.remainingDistance > _agent.radius;
    }
}