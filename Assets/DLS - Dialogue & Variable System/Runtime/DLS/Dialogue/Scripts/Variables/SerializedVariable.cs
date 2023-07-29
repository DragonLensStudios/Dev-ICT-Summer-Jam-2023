namespace DLS.Dialogue
{
    [System.Serializable]
    public class SerializedVariable<T>
    {
        public string Name;
        public T Value;

        public SerializedVariable(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}