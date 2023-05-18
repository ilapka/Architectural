using Infrastructure.Services;
using Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace Logic
{
    public class SaveTrigger : MonoBehaviour
    {
        private ISaveLoadService _saveLoadService;

        [SerializeField]
        private BoxCollider _collider;
        
        private void Awake()
        {
            _saveLoadService = AllServices.Container.Single<ISaveLoadService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _saveLoadService.SaveProgress();
            Debug.Log($"Progress saved.");
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if(!_collider)
                return;
            
            Gizmos.color = new Color32(30, 200, 30, 130);
            Gizmos.DrawCube(transform.position + _collider.center, _collider.size);
        }
    }
}