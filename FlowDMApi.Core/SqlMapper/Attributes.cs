using System;

namespace FlowDMApi.Core.SqlMapper
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public string _tableName { get; set; }
        public TableNameAttribute(string Name)
        {
            _tableName = Name;
        }
    }

    public  enum PropertyType
    {
        Key,
        Default
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        public string _columnName { get; set; }
        public PropertyType PropertType{ get; set; }
        public ColumnNameAttribute(string Name)
        {
            _columnName = Name;
            PropertType = PropertyType.Default;
        }

        public ColumnNameAttribute(string Name, PropertyType propertyType)
        {
            _columnName = Name;
            PropertType = propertyType;
        }

        public ColumnNameAttribute(PropertyType propertyType)
        {
            _columnName = null;
            PropertType = propertyType;
        }
      
    }
}
