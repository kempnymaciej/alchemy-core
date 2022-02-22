namespace AlchemyBow.Core
{
    public interface ICoreState
    {
        void OnInit();
        void OnDeinit();

        void OnUpdate();
        void OnFixedUpdate();
    } 
}
