using UnityEngine;

namespace Entity
{
    public class ElfArcher : Unit
    {
        public override void PlayerWin()
        {
            Debug.Log($"[Elf Archer: Win!]");
        }
    }
}