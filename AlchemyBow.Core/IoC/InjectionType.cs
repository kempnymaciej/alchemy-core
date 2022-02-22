namespace AlchemyBow.Core.IoC
{
    [System.Flags]
    public enum InjectionType 
    {
        None = 0,
        Fields = 1,
        Properties = 2,
        Methodes = 4,
    } 
}
