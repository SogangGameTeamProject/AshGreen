using UnityEngine;
using AshGreen.Character;
namespace AshGreen.DamageObj
{
    public class TestDamageObj : MonoBehaviour
    {
        private int damage = 1;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            Debug.Log(damageable);
            // 만약 인터페이스가 존재하면 실행
            if (damageable != null)
            {
                // 데미지 10을 전달
                damageable.TakeDamage(damage);
            }
        }
    }

}
