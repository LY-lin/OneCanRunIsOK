using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class BulletNoOverride : BulletStandard
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            base.SetOverride(false);

        }
    }
}
