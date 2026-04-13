using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DataTableNameAttribute : Attribute
{
    public string TableName { get; }
    public DataTableNameAttribute(string tableName) => TableName = tableName;
}