using UnityEngine;

namespace Entity
{
    public class MeridaUnit : Unit
    {
        public override void Start()
        {
            base.Start();
            _animation["Victory"].wrapMode = WrapMode.Once;
        }

        public override void PlayerWin() => _animation.Play("Win");
    }
}