using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AshGreen.Character.Player
{
    public class InputHandler : NetworkBehaviour
    {
        private CharacterController _player = null;//플레이어 컨트롤러

        //커맨드들
        public PlayerCommandInit _moveCommand = null;
        public PlayerCommandInit _jumpCommand = null;
        public PlayerCommandInit _downJumpCommand = null;
        public PlayerCommandInit _mainSkillCommand = null;
        public PlayerCommandInit _secondarySkillCommand = null;
        public PlayerCommandInit _specialSkillCommand = null;

       

        private void Start()
        {
            //로컬 객체가 아니면 인풋 시스템 제거
            if (!IsOwner)
            {
                PlayerInput playerInput = GetComponent<PlayerInput>();
                Destroy(playerInput);
            }
                
            _player = GetComponent<CharacterController>();//플레이어 컨트롤러 초기화
        }

        //------입력 처리 부분------
        //이동 입력 처리
        public void OnMove(InputAction.CallbackContext context)
        {
            
            if (context.performed)
            {
                _moveCommand.Execute(_player);
                Debug.Log("Move");
            }
        }

        //점프 입력 처리
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
                _jumpCommand.Execute(_player);
        }

        //아래 점프 입력 처리
        public void OnDownJump(InputAction.CallbackContext context)
        {
            if(context.started)
                _downJumpCommand.Execute(_player);
        }

        //메인 스킬 입력 처리
        public void OnMainSkill(InputAction.CallbackContext context)
        {
            if (context.started)
                _mainSkillCommand.Execute(_player);
        }

        //보조스킬 입력 처리
        public void OnSecondarySkill(InputAction.CallbackContext context)
        {
            if (context.started)
                _secondarySkillCommand.Execute(_player);
        }

        //특수스킬 입력 처리
        public void OnSpecialSkill(InputAction.CallbackContext context)
        {
            if (context.started)
                _specialSkillCommand.Execute(_player);
        }
    }
}

