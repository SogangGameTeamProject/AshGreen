using System.Collections;
using UnityEngine;

namespace AshGreen.Character{
    public class HowitzerPattern : EnemyPatteurnStateInit
    {
        public override void Enter(EnemyController controller)
        {
            base.Enter(controller);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void Exit()
        {

        }

        protected override IEnumerator ExePatteurn()
        {
            //보스 중앙으로 이동

            

            //곡사포 발사


            //원위치로 이동

            yield return base.ExePatteurn();
        }
    }
}
