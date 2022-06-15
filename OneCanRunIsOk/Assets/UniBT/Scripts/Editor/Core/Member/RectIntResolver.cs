using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class RectIntResolver : FieldResolver<UnityEngine.UIElements.RectIntField,RectInt>
    {
        public RectIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.RectIntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.RectIntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(RectInt);
    }
}