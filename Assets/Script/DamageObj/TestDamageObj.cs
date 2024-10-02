using UnityEngine;
using AshGreen.Character;
namespace AshGreen.DamageObj
{
    public class DamageObjBase : MonoBehaviour
    {
        public int damage = 1;
        public bool isCritical = false;
        public bool isNockback = false;
        public float nockbackPower = 100f;
        public float nockbackTime = 0.3f;
        public bool isDestroy = false;

        private void OnTriggerStay2D(Collider2D collision)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            // 만약 인터페이스가 존재하면 실행
            if (damageable != null)
            {
                //넉백 여부에 따른 넉백
                if (isNockback)
                {
                    MovementController movementController = collision.gameObject.GetComponent<MovementController>();
                    float nockBackForceX =
                        collision.gameObject.transform.position.x > this.transform.position.x ?
                        1 : -1;
                    Vector2 nockBackForce = new Vector2(nockBackForceX, 1);

                    movementController.ExcutNockBack(nockBackForce, nockbackPower, nockbackTime);
                }

                damageable.TakeDamage(damage);

                //제거여부
                if (isDestroy)
                    Destroy(this.gameObject);
            }

        }
    }

}
