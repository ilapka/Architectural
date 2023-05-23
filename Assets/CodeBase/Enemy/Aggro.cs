using System.Collections;
using Logic;
using UnityEngine;

namespace Enemy
{
    public class Aggro : MonoBehaviour
    {
        [SerializeField]
        private TriggerObserver _triggerObserver;
        [SerializeField]
        private Follow _follow;
        [SerializeField]
        private float _cooldown;

        private Coroutine _aggroRoutine;
        private bool _hasAggroTarget;

        private void Start()
        {
            _triggerObserver.TriggerEnter += TriggerEnter;
            _triggerObserver.TriggerExit += TriggerExit;

            SwitchFollowOff();
        }

        private void TriggerEnter(Collider obj)
        {
            if (!_hasAggroTarget)
            {
                _hasAggroTarget = true;
                StopAggroRoutine();
                SwitchFollowOn();   
            }
        }

        private void TriggerExit(Collider obj)
        {
            if(_hasAggroTarget)
            {
                _hasAggroTarget = false;
                _aggroRoutine = StartCoroutine(SwitchFollowOffAfterCooldown());
            }
        }

        private IEnumerator SwitchFollowOffAfterCooldown()
        {
            yield return new WaitForSeconds(_cooldown);
            SwitchFollowOff();
        }

        private void StopAggroRoutine()
        {
            if (_aggroRoutine != null)
            {
                StopCoroutine(_aggroRoutine);
                _aggroRoutine = null;
            }
        }

        private void SwitchFollowOn() =>
            _follow.SetEnable(true);

        private void SwitchFollowOff() =>
            _follow.SetEnable(false);

        private void OnDestroy()
        {
            _triggerObserver.TriggerEnter -= TriggerEnter;
            _triggerObserver.TriggerExit -= TriggerExit;
        }
    }
}