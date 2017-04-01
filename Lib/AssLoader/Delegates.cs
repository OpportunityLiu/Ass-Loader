namespace AssLoader
{
    internal delegate object GetValueDelegate(object obj);
    internal delegate void SetValueDelegate(object obj, object value);
    internal delegate object DeserializerDelegate(string value);
    internal delegate string SerializerDelegate(object value);
    internal delegate void DeserializeDelegate(object obj, string value);
    internal delegate string SerializeDelegate(object obj);
}