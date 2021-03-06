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

            experience = 10

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
