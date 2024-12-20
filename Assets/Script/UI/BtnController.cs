using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
namespace AshGreen.UI
{
    public class BtnController : MonoBehaviour
    {
        private ulong clientId;

        private void OnEnable()
        {
            clientId = NetworkManager.Singleton.LocalClientId;
        }

        public void OnShutdown()
        {
            ClientConnection.Instance.RemoveClient(clientId);
        }
    } 
}