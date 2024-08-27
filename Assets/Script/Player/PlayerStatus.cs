using UnityEngine;

namespace AshGreen.Player
{
    [CreateAssetMenu(fileName = "PlayerStatus", menuName = "Scriptable Objects/PlayerStatus")]
    public class PlayerStatus : ScriptableObject
    {
        [Tooltip("�ִ�ü��")]
        public int MaxHP = 0;
        [Tooltip("���ݷ�")]
        public float AttackPower = 0;
        [Tooltip("�̵��ӵ�")]
        public float MoveSpeed = 0;
        [Tooltip("�����Ŀ�")]
        public float JumpPower = 0;
        [Tooltip("�ִ� ���� Ƚ��")]
        public int JumMaxNum = 0;
        [Tooltip("��ų����")]
        public float SkillAcceleration;
        [Tooltip("������ ����")]
        public float ItemAcceleration;
        [Tooltip("ġ��Ÿ Ȯ��")]
        public float CriticalChance;
        [Tooltip("��ų����")]
        public float CriticalDamage = 0;
    }
}