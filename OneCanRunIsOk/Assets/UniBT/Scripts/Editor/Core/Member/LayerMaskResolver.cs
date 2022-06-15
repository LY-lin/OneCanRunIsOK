using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;

namespace UniBT.Editor
{
    public class LayerMaskResolver : FieldResolver<LayerMaskField,int>
    {
        public LayerMaskResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override LayerMaskField CreateEditorField(FieldInfo fieldInfo)
        {
            return new LayerMaskField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(LayerMask);
    }
}