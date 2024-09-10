using UnityEngine;
using UnityEngine.InputSystem;

namespace AshGreen.Character
{
    public class UnableState : CharacterStateBase
    {
        private Rigidbody2D rBody = null;

        public override void Enter(CharacterController character)
        {
            base.Enter(character);
        }

        public override void StateUpdate()
        {

                
            
        }

        public override void Exit()
        {
            
        }
    }
}