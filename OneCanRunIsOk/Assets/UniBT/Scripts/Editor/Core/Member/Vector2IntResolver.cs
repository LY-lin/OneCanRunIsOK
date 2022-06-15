using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class Vector2IntResolver : FieldResolver<UnityEngine.UIElements.Vector2IntField, Vector2Int>
    {
        public Vector2IntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.Vector2IntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.Vector2IntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Vector2Int);
    }
}