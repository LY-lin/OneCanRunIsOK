using System;
using System.Reflection;


namespace UniBT.Editor
{
    public class DoubleResolver : FieldResolver<UnityEngine.UIElements.DoubleField,double>
    {
        public DoubleResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.DoubleField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.DoubleField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(double);
    }
}