using UnityEngine;

namespace Infrastructure.AssetManagement
{
    public class  AssetProvider : IAssetProvider
    {
        public GameObject Instantiate(string path)
        {
            GameObject heroPrefab = Resources.Load<GameObject>(path);
            GameObject hero = Object.Instantiate(heroPrefab);

            return hero;
        }

        public GameObject Instantiate(string path, Vector3 at)
        {
            GameObject heroPrefab = Resources.Load<GameObject>(path);
            GameObject hero = Object.Instantiate(heroPrefab, at, Quaternion.identity);

            return hero;
        }
    }
}