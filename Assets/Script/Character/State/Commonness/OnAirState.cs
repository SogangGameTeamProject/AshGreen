using UnityEngine;

namespace AshGreen.Character
{
    public class OnAirState : CharacterStateBase
    {
        public CharacterStateType onGroundType = CharacterStateType.Idle;//바닥 착지시 전환할 상태
        //바닥 체크를 위한 설정값
        public LayerMask groundLayer;
        public Vector2 groundChkOffset = Vector2.zero;
        public float groundChkRadius = 0.15f;

        public override void Enter(CharacterController character)
        {
            base.Enter(character);
        }

        public override void StateUpdate()
        {
            //레이케스트로 바닥 위인지 체크 맞으면 OnAir상태로 전환
            Vector2 playerPos = _character.transform.position;
            RaycastHit2D groundHit =
                Physics2D.CircleCast(playerPos+groundChkOffset, groundChkRadius, Vector2.up, groundChkRadius, groundLayer);
            if (groundHit.collider != null)
                _character.StateTransition(onGroundType);

            Debug.Log("OnAir");
        }

        public override void Exit()
        {
            
        }
    }
}