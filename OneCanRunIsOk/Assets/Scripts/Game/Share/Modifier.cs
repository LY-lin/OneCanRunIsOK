using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    // you must create Modifier use Constructor
    // Modifier(float _baseValue, ModifierType _type, Object _source)
    public class Modifier
    {
        public enum ModifierType{
<<<<<<< HEAD
<<<<<<< HEAD
            experience = 10,
            healRateBuff=11,
            physicalAttackBuff=12,
            magicAttackBuff=13,
            physicalDefenceBuff=14,
            magicDefenceBuff=15,
            maxSpeedBuff=16,
            maxJumpBuff=17,
=======
            experience = 10
>>>>>>> 226bc9b81dea8e27f3baa870fa4348d17f5a9a83
=======
            experience = 10
>>>>>>> 226bc9b81dea8e27f3baa870fa4348d17f5a9a83
        }
        public float baseValue;
        public Object source;
        public ModifierType type;

        // _source states where the Modifier comes from
        // in case we cannot delete modifier via source
        public Modifier(float _baseValue, ModifierType _type, Object _source)
        {
            baseValue = _baseValue;
            type = _type;
            source = _source;
        }
        
    }
}
