using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AshGreen.Platform
{
    public class PlatformDestroyState : PlatformStateInit
    {
        [SerializeField]
        private float destroyDelayTime = 0.5f;
        private float currentTime = 0;
        
        public override void Enter(PlatformController context)
        {
            base.Enter(context);
            currentTime = 0;
        }

        public override void StateUpdate()
        {
            if (!IsServer) return;
            currentTime += Time.deltaTime;
            if (currentTime > destroyDelayTime)
            {
                //자식 네트워크 오브젝트도 같이 디스폰
                NetworkObject[] childNetworkObjs = _controller.transform.GetComponentsInChildren<NetworkObject>()
                                                    .Where(obj => obj != _controller.GetComponent<NetworkObject>())
                                                    .ToArray();
                foreach (var netObj in childNetworkObjs)
                {
                    netObj.Despawn();
                }
                NetworkObject.Despawn(this.gameObject);
            }  
        }

        public override void Exit()
        {
            
        }
    }
}

