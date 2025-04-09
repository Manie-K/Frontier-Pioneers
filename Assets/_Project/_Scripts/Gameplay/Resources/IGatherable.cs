using FrontierPioneers.Gameplay.NPC.Gatherer;

namespace FrontierPioneers.Gameplay.Resources
{
    public interface IGatherable
    {
        public void Gather(GathererController gatherer);
    }
}