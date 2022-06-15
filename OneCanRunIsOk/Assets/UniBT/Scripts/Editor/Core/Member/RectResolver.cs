using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class RectResolver : FieldResolver<UnityEngine.UIElements.RectField,Rect>
    {
        public RectResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.RectField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.RectField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Rect);
    }
}