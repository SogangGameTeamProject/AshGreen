using AshGreen.Character;
using AshGreen.Character.Skill;
using UnityEngine;

namespace AshGreen.Character.Player
{
    public class PlayerController : CharacterController
    {
        [HideInInspector]
        public GameplayManager gameplayManager;//게임플레이 메니저
        [HideInInspector]
        public PlayerUI playerUI;//플레이어 UI
        public ulong clientID;

        public MovementController _movementController = null;
        public CharacterSkillManager _characterSkillManager = null;
        public Transform firePoint = null;
    }
}
