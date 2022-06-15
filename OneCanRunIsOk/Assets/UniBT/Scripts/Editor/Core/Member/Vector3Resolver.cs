using System.Reflection;

using UnityEngine;

namespace UniBT.Editor
{
    public class Vector3Resolver : FieldResolver<UnityEngine.UIElements.Vector3Field,Vector3>
    {
        public Vector3Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.Vector3Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.Vector3Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Vector3);

    }
}