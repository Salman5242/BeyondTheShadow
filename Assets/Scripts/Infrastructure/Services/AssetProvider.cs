using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Infrastructure.Services
{
    public class AssetProvider : IService
    {
        public void Initialize()
        {
            Addressables.InitializeAsync();
        }

        public Task<GameObject> Instantiate(string address, Vector3 at) =>
          Addressables.InstantiateAsync(address, at, Quaternion.identity).Task;

        public Task<GameObject> Instantiate(string address) =>
          Addressables.InstantiateAsync(address).Task;
    }
}