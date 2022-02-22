namespace AlchemyBow.Core
{
    public sealed class OperationHandle
    {
        public bool IsDone { get; private set; }
        public void MarkDone() => IsDone = true;
    } 
}
