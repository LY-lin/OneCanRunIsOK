using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class BoundsIntResolver : FieldResolver<UnityEngine.UIElements.BoundsIntField,BoundsInt>
    {
        public BoundsIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.BoundsIntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.BoundsIntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(BoundsInt);
    }
}