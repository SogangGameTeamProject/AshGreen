using AshGreen.Buff;
using AshGreen.Character;
using AshGreen.Character.Player;
using AshGreen.UI;
using UnityEngine;

namespace AshGreen.Debuff
{
    public class Debuff
    {
        public EnemyController _targetEnemy;// 버프 대상 플레이어
        public DebuffData debuffData;// 버프 데이터
        public float remainingDuration;// 남은 지속 시간
        public float currentTimer;// 현재 타이머
        public int currentStacks;// 현재 중첩 수
        public int currentDamage; //

        private GameObject debuffTimer = null;

        // 버프 생성자
        public Debuff(DebuffData data, EnemyController targetEnemy, int stack)
        {
            debuffData = data;
            _targetEnemy = targetEnemy;
            currentStacks = stack;

            // 디버프 타이머 UI 생성
            debuffTimer = GameObject.Instantiate(_targetEnemy.debuffManager._debuffIconPre,
                _targetEnemy.debuffManager._debuffUICanvas);
            debuffTimer?.GetComponent<DebuffTimer>().SetDebuff(this);
        }

        public void Apply()
        {
            remainingDuration = debuffData.duration;
            currentTimer = 0;
            currentDamage = 0;
            debuffData.ApplyDebuff(_targetEnemy, this);
        }

        public void Remove()
        {
            debuffData.RemoveDebuff(_targetEnemy, this);
            GameObject.Destroy(debuffTimer);// 디버프 타이머 UI 제거
        }

        // 버프 재적용
        public void Reapply(int stack)
        {
            if (currentStacks < stack)
            {
                currentStacks = stack;

                if (_targetEnemy.IsOwner)
                {
                    debuffData.RemoveDebuff(_targetEnemy, this);
                    debuffData.ApplyDebuff(_targetEnemy, this);
                }
            }
            else
            {
                remainingDuration = debuffData.duration;
            }
        }

        public void Update(float deltaTime)
        {
            if (debuffData.durationType == DebuffDurationType.Timed)
            {
                remainingDuration -= deltaTime;
                currentTimer += deltaTime;
                if (remainingDuration <= 0)
                    _targetEnemy.debuffManager.RemoveDebuffRpc(debuffData.debuffType);
            }

            if (_targetEnemy.IsOwner)
                debuffData.UpdateDebuff(_targetEnemy, this);
        }

        public void saveDamage(float damage)
        {
            currentDamage += (int)damage;
        }
    }
}
