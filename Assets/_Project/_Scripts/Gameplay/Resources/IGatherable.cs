using System;
using FrontierPioneers.Gameplay.NPC.Gatherer;

namespace FrontierPioneers.Gameplay.Resources
{
    public interface IGatherable
    {
        public void Gather(GathererController gatherer);
        public bool CanGather(GathererController gatherer);
        public event Action<IGatherable> OnGatherableDepleted;
    }
}