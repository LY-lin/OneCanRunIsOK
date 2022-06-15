using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class Vector2Resolver : FieldResolver<UnityEngine.UIElements.Vector2Field, Vector2>
    {
        public Vector2Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.Vector2Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.Vector2Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Vector2);
    }
}