namespace Compendium.API.GameModules.Subroutines
{
    public interface ICustomSubroutine : ISubroutine
    {
        Player Player { get; set; }

        void Start();
        void Update();
        void Destroy();
    }
}