using Infrastructure.Services;
using Infrastructure.States;
using UnityEngine;

namespace Logic
{
    public class LevelTransferTrigger : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        
        public string TransferTo;
        
        private IGameStateMachine _stateMachine;

        private bool _isTriggered;

        private void Awake()
        {
            _stateMachine = AllServices.Container.Single<IGameStateMachine>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_isTriggered)
                return;
            
            if (other.CompareTag(PlayerTag))
            {
                _isTriggered = true;
                _stateMachine.Enter<LoadLevelState, string>(TransferTo);
            }
        }
    }
}