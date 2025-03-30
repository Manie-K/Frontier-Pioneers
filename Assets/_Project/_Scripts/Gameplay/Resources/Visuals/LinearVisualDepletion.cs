using System;

namespace FrontierPioneers.Gameplay.Resources.Visuals
{
    public class LinearVisualDepletion : VisualDepletionBase
    {
        public override void VisualDeplete(int count)
        {
            if (count < 0 || count > resourcePieces.Length || count > VisibleResourcePiecesCount)
            {
                throw new ArgumentException($"Invalid count value -> {count}");
            }
            
            for (int i = 0; i < count; i++)
            {
                resourcePieces[i].enabled = false;
            }
        }
    }
}