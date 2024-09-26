using System.Collections;
using UnityEngine;

namespace AshGreen.Character.Skill
{
    public enum SkillType
    {
        Attack, Utill
    }

    public enum UseType
    {
        Time, Coast
    }

    public abstract class CharacterSkill : ScriptableObject
    {
        public string skillName;   // 스킬 이름
        public float activeTime;   // 스킬 사용 시간
        public float cooldownTime; // 스킬 쿨타임
        public int maxChageCnt = 1;//최대 충전 수

        public bool charging = false; //스킬 차징 여부
        public float chargingMoveSpeed = 0.75f; // 차징 시 이속

        public UseType useType = UseType.Time;


        public SkillType skillType;//스킬 타입

        //공격 스킬 시 설정
        public float damageMultiplier = 1; // 스킬 데미지 배수

        //유틸 스킬 시 설정
        public float utillValue = 1;
        public float utillTime = 1;

        public Sprite skillIcon;   // 스킬 아이콘
        public string animationTrigger; // 스킬 발동 시 애니메이션 트리거
        public AudioClip skillSound;    // 스킬 발동 소리

        //스킬 차징
        public virtual IEnumerator Chargin(SkillHolder holder)
        {
            holder.state = SkillHolder.SkillState.charge;//차징
            float charginTime = 0;

            while (holder.state == SkillHolder.SkillState.charge)
            {
                charginTime += Time.deltaTime;
                yield return null;
            }

            holder.holderCorutine = holder.StartCoroutine(Use(holder, charginTime));//차징 후 
        }

        //스킬 사용
        public abstract IEnumerator Use(SkillHolder holder, float chageTime = 0);
    }
}

 