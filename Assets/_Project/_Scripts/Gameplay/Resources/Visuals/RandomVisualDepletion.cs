using System;

namespace FrontierPioneers.Gameplay.Resources.Visuals
{
    public class RandomVisualDepletion : VisualDepletionBase
    {
        const int MaxIterations = 300;
        
        public override void VisualDeplete(int count)
        {
            if (count < 0 || count > resourcePieces.Length || count > VisibleResourcePiecesCount)
            {
                throw new ArgumentException($"Invalid count value -> {count}");
                return;
            }            
            
            int depletedCount = 0;
            int iterations = 0;
            while(depletedCount < count && iterations < MaxIterations)
            {
                int randomIndex = UnityEngine.Random.Range(0, resourcePieces.Length);
                if (resourcePieces[randomIndex].enabled)
                {
                    resourcePieces[randomIndex].enabled = false;
                    depletedCount++;
                }
                iterations++;
            }
        }
    }
}