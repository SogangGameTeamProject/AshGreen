using UnityEngine;
using AshGreen.Obsever;
using System;
using System.Collections.Generic;
using AshGreen.Character;
using Unity.Netcode;
using AshGreen.State;
using Unity.VisualScripting;
using AshGreen.Character.Skill;

namespace AshGreen.Character
{

    //캐릭터 방향 타입
    public enum CharacterDirection
    {
        Left = -1, Right = 1
    }

    //전투 상태 타입
    public enum CombatStateType
    {
        Null = -1, Idle = 0, Hit = 1, Death = 2, 
    }

    public class CharacterController : Subject
    {
        //외부 컨트롤러들
        public DamageReceiver _damageReceiver = null;
        public StatusEffectManager _statusEffectManager = null;
        public Animator _animator = null;

        //------상태 패턴 관련 전역 변수 선언------
        
        //---------전투 상태--------
        public CombatStateType runningCombatStateType;
        private StateContext<CharacterController> combatStateContext = null;
        //상태 정보 관리를 위한 클래스
        [System.Serializable]
        public class CombatStateData
        {
            public CombatStateType type;//트리거할 타입
            public CharacterStateBase state;//실행할 상태
        }
        public List<CombatStateData> combatStateList//상태 관리를 위한 리스트
            = new List<CombatStateData>();

        //캐릭터 방향 관련
        private NetworkVariable<CharacterDirection> characterDirection = 
            new NetworkVariable< CharacterDirection>(CharacterDirection.Right);

        public CharacterDirection CharacterDirection
        {
            get
            {
                return characterDirection.Value;
            }

            private set
            {
                characterDirection.Value = value;
            }
        }

        [Rpc(SendTo.Server)]
        public void SetCharacterDirectionRpc(CharacterDirection chageDirection)
        {
            characterDirection.Value = chageDirection;
        }

        //방향 전환 매서드
        [Rpc(SendTo.ClientsAndHost)]
        private void OnFlipRpc(CharacterDirection previousValue, CharacterDirection newValue)
        {
            if (runningCombatStateType == CombatStateType.Death)
                return;

            Vector3 flipScale = transform.localScale;
            if (CharacterDirection == CharacterDirection.Left)
                flipScale.x = Mathf.Abs(flipScale.x) * -1;
            else
                flipScale.x = Mathf.Abs(flipScale.x);

            transform.localScale = flipScale;
        }

        //------스테이터스 관련 전역 변수 선언------
        public CharacterConfig baseConfig = null;//기본능력치가 저장되는 변수
        //레벨 관련
        private NetworkVariable<int> LevelUpEx = new NetworkVariable<int>(0);
        private NetworkVariable<int> level = new NetworkVariable<int>(1);
        private NetworkVariable<int> experience = new NetworkVariable<int>(0);

        public int Experience
        {
            get
            {
                return experience.Value;
            }
            private set
            {
                int ex = experience.Value + value;
                if(ex >= LevelUpEx.Value)
                {
                    ex -= LevelUpEx.Value;
                    level.Value++;
                }
                experience.Value = ex;
            }
        }

        //최대체력 관련 전역변수
        private NetworkVariable<int> baseMaxHP = new NetworkVariable<int>(0);
        private NetworkVariable<int> addMaxHp = new NetworkVariable<int>(0);
        private NetworkVariable<int> GrowthMaxHP = new NetworkVariable<int>(0);
        private NetworkVariable<float> GrowthPerMaxHP = new NetworkVariable<float>(0);
        public int MaxHP
        {
            get
            {
                int maxHp =
                    (int)(baseMaxHP.Value + addMaxHp.Value + (level.Value * GrowthMaxHP.Value) * (level.Value * GrowthPerMaxHP.Value));
                return maxHp > 0 ? maxHp : 1;
            }
        }
        //현재 체력 관련 전역변수
        public NetworkVariable<int> nowHp = new NetworkVariable<int>(0);

        public int NowHP
        {
            get
            {
                return nowHp.Value;
            }
            private set
            {
                nowHp.Value = Mathf.Clamp(value, 0, MaxHP);
                Debug.Log("현재체력: " + nowHp.Value);
            }
        }

        /// <summary>
        /// 플레이어의 체력을 조정하는 메서드
        /// </summary>
        /// <param name="addNowHp">증감 체력값</param>
        /// <param name="addMaxHp">증감할 최대체력 값</param>
        [Rpc(SendTo.Server)]
        public void SetHpRpc(int addNowHp, int addMaxHp = 0)
        {
            this.addMaxHp.Value += addMaxHp;
            NowHP += addNowHp;
        }

        //공격력 관련 전역변수
        private NetworkVariable<int> baseAttackPower = new NetworkVariable<int>(0);
        private NetworkVariable<int> GrowthAttackPower = new NetworkVariable<int>(0);
        private NetworkVariable<float> GrowthPerAttackPower = new NetworkVariable<float>(0);
        private NetworkVariable<int> addAttackPower = new NetworkVariable<int>(0);
        private NetworkVariable<float> addAttackPerPower = new NetworkVariable<float>(1);

        public int AttackPower
        {
            get
            {
                float attackPower =
                    (baseAttackPower.Value * (level.Value * GrowthPerAttackPower.Value) + addAttackPower.Value)
                    * addAttackPerPower.Value;
                return attackPower > 0 ? (int)attackPower : 1;
            }
        }
        /// <summary>
        /// 플레이어 공격력을 조정하는 메서드
        /// </summary>
        /// <param name="addAttackPower">증감할 공격력(고정)</param>
        /// <param name="addAttackPowerPer">증감할 공격력(%)</param>
        [Rpc(SendTo.Server)]
        public void SetAttackpowerRpc(int addAttackPower, float addAttackPerPower = 0)
        {
            this.addAttackPower.Value += addAttackPower;
            this.addAttackPerPower.Value += addAttackPerPower;
        }

        //이동속도 관련 변수
        private NetworkVariable<float> baseMoveSpeed = new NetworkVariable<float>(0);
        private NetworkVariable<float> addMoveSpeed = new NetworkVariable<float>(0);
        private NetworkVariable<float> addMovePerSpeed = new NetworkVariable<float>(1);
        public float MoveSpeed
        {
            get
            {
                return (baseMoveSpeed.Value + addMoveSpeed.Value) * addMovePerSpeed.Value;
            }
        }

        /// <summary>
        /// 이동속도 능력치 조정 메서드
        /// </summary>
        /// <param name="addMoveSpeed">증감할 이동속도(고정)</param>
        /// <param name="addMovePerSpeed">증감할 이동속도(%)</param>
        [Rpc(SendTo.Server)]
        public void SetMovespeedRpc(float addMoveSpeed, float addMovePerSpeed = 0)
        {
            this.addMoveSpeed.Value += addMoveSpeed;
            this.addMovePerSpeed.Value += addMovePerSpeed;
        }

        //점프파워 관련 변수
        private NetworkVariable<float> baseJumpPower = new NetworkVariable<float>(0);
        private NetworkVariable<float> addJumpPower = new NetworkVariable<float>(0);
        public float JumpPower
        {
            get { 
                return baseJumpPower.Value + addJumpPower.Value; 
            }
        }
        //점프 횟수 관련 변수
        private NetworkVariable<int> baseJumMaxNum = new NetworkVariable<int>(0);
        private NetworkVariable<int> addJumMaxNum =  new NetworkVariable<int>(0);
        public int JumMaxNum
        {
            get
            {
                return baseJumMaxNum.Value + addJumMaxNum.Value;
            }
        }
        /// <summary>
        /// 점프관련 스탯 조정 메서드
        /// </summary>
        /// <param name="addJumMaxNum"></param>
        /// <param name="addJumpPower"></param>
        [Rpc(SendTo.Server)]
        public void SetJumpRpc(int addJumMaxNum, float addJumpPower = 0)
        {
            this.addJumMaxNum.Value += addJumMaxNum;
            this.addJumpPower.Value += addJumpPower;
        }

        public int jumCnt { get; set; }
        //스킬가속 관련 변수
        private NetworkVariable<float> addSkillAcceleration =  new NetworkVariable<float>(0);
        public float SkillAcceleration {
            get
            {
                return addSkillAcceleration.Value;
            }
        }
        //아이템가속 관련 변수
        private NetworkVariable<float> addItemAcceleration =  new NetworkVariable<float>(0);
        public float ItemAcceleration
        {
            get
            {
                return addItemAcceleration.Value;
            }
        }
        /// <summary>
        /// [스킬, 아이템] 가속 스탯 증감 조정 메서드
        /// </summary>
        /// <param name="addSkillAcceleration">증감할 스킬 가속</param>
        /// <param name="addItemAcceleration">증감할 아이템 가속</param>
        [Rpc(SendTo.Server)]
        public void SetAccelerationRpc(float addSkillAcceleration, float addItemAcceleration = 0)
        {
            this.addSkillAcceleration.Value += addSkillAcceleration;
            this.addItemAcceleration.Value += addItemAcceleration;
        }
        //치명타 확률
        private NetworkVariable<float> baseCriticalChance = new NetworkVariable<float>(0);
        private NetworkVariable<float> addCriticalChance =  new NetworkVariable<float>(0);
        public float CriticalChance
        {
            get
            {
                return baseCriticalChance.Value + addCriticalChance.Value;
            }
        }
        //치명타 데미지
        private NetworkVariable<float> baseCriticalDamage = new NetworkVariable<float>(0);
        private NetworkVariable<float> addCriticalDamage  = new NetworkVariable<float>(0);
        public float CriticalDamage
        {
            get
            {
                return baseCriticalDamage.Value + addCriticalDamage.Value;
            }
        }
        /// <summary>
        /// 치명타 관련 스탯 증감 조정 메서드
        /// </summary>
        /// <param name="addCriticalChance">증감 치명타 확률</param>
        /// <param name="addCriticalDamage">증감 치명타 데미지</param>
        [Rpc(SendTo.Server)]
        public void SetCriticalRpc(float addCriticalChance, float addCriticalDamage = 0)
        {
            this.addCriticalChance.Value += addCriticalChance;
            this.addCriticalDamage.Value += addCriticalDamage;
        }

        //피해 면역 관련 
        public NetworkVariable<bool> isDamageImmunity = new NetworkVariable<bool>(false);// 피해 면역
        public LayerMask projectilesLayer;
        [Rpc(SendTo.Server)]
        public void SetDamageimmunityRpc(bool damageImmunity)
        {
            this.isDamageImmunity.Value = damageImmunity;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            combatStateContext = new StateContext<CharacterController>(this);//콘텍스트 생성

            //캐릭터 방향 값 변경 시 처리
            characterDirection.OnValueChanged += OnFlipRpc;

            //피해 면역 처리
            isDamageImmunity.OnValueChanged += DamageImmunityRpc;

            //피격 타격 액션 설정
            _damageReceiver.TakeDamageAction += TakeDamage;


            _animator.runtimeAnimatorController = baseConfig.animator;

            if (IsOwner)
            {
                OnSetStatusRpc();//스테이터스 값 초기화

                CombatStateTransitionRpc(CombatStateType.Idle);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            //델리게이터 제거
            characterDirection.OnValueChanged -= OnFlipRpc;
            isDamageImmunity.OnValueChanged -= DamageImmunityRpc;
            _damageReceiver.TakeDamageAction -= TakeDamage;

        }

        private void FixedUpdate()
        {
            if (IsSpawned)
                combatStateContext.StateUpdate();
        }

        //피해면역 처리
        [Rpc(SendTo.ClientsAndHost)]
        public void DamageImmunityRpc(bool preValue, bool newValue)
        {
            if(preValue == newValue) return;
            Debug.Log("무적 상태 전환: " + newValue);
            Rigidbody2D rBody = GetComponent<Rigidbody2D>();

            if (newValue)
            {
                rBody.excludeLayers |= projectilesLayer;
            }
            else
            {
                rBody.excludeLayers &= ~projectilesLayer;
            }
        }

        //캐릭터 스테이터스값 초기 설정
        [Rpc(SendTo.Server)]
        private void OnSetStatusRpc()
        {
            if (baseConfig)
            {

                LevelUpEx.Value = baseConfig.LevelUpEx;
                baseMaxHP.Value = baseConfig.MaxHP;
                nowHp.Value = baseMaxHP.Value;
                GrowthAttackPower.Value = baseConfig.GrowthAttackPower;
                GrowthPerAttackPower.Value = baseConfig.GrowthPerAttackPower;
                baseAttackPower.Value = baseConfig.AttackPower;
                GrowthAttackPower.Value = baseConfig.GrowthAttackPower;
                GrowthPerAttackPower.Value = baseConfig.GrowthPerAttackPower;
                baseMoveSpeed.Value = baseConfig.MoveSpeed;
                baseJumpPower.Value = baseConfig.JumpPower;
                baseJumMaxNum.Value = baseConfig.JumMaxNum;
            }
        }

        /// <summary>
        /// 피격 타격 처리 메서드
        /// </summary>
        /// 
        public void TakeDamage(float damage)
        {
            if (runningCombatStateType != CombatStateType.Death)
            {
                Debug.Log("플레이어 피격 처리");

                if (nowHp.Value-damage > 0)
                    CombatStateTransitionRpc(CombatStateType.Hit);
                else
                    CombatStateTransitionRpc(CombatStateType.Death);

                SetHpRpc(-(int)damage);
            }
        }

        /// <summary>
        /// 피격 타격 처리 메서드
        /// </summary>
        /// 
        public void DealDamage(CharacterController target, float damage, AttackType attackType, bool isCritical = false)
        {
            _damageReceiver.TakeDamage(damage);
        }

        //----------상태패턴 관련 함수들---------

        //-----전투 상태 과련 함수----
        //전투 상태 변환 함수
        [Rpc(SendTo.ClientsAndHost)]
        public void CombatStateTransitionRpc(CombatStateType type)
        {
            Debug.Log("상태전환: " + type);
            IState<CharacterController> state = null;
            CombatStateData findState = combatStateList.Find(state => state.type.Equals(type));
            if (findState != null)
            {
                state = findState.state.GetComponent<IState<CharacterController>>();
                runningCombatStateType = findState.type;
                Debug.Log(runningCombatStateType);
                combatStateContext.TransitionTo(state);
            }
        }
    }
}
