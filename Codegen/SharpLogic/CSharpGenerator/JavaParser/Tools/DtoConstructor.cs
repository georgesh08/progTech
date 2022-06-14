using System.Collections.Generic;

namespace JavaParser.Tools
{
    public class DtoConstructor
    {
        public string ClassName;
        private List<Field> _objectFields = new();

        public void AddField(Field field)
        {
            _objectFields.Add(field);
        }

        public List<Field> Fields()
        {
            return _objectFields;
        }
    }
}