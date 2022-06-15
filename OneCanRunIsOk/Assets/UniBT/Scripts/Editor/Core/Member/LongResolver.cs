using System.Reflection;


namespace UniBT.Editor
{
    public class LongResolver : FieldResolver<UnityEngine.UIElements.LongField,long>
    {
        public LongResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.LongField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.LongField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(long);
    }
}