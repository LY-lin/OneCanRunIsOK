using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class BoundsResolver : FieldResolver<UnityEngine.UIElements.BoundsField,Bounds>
    {
        public BoundsResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.BoundsField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.BoundsField(fieldInfo.Name);
        }

        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Bounds);
    }
}