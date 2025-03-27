using System;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Resources.Visuals
{
    public abstract class VisualDepletionBase : MonoBehaviour
    {
        protected MeshRenderer[] resourcePieces;

        public int AllResourcePiecesCount => resourcePieces.Length;
        public int VisibleResourcePiecesCount => Array.FindAll(resourcePieces, x => x.enabled).Length;
        protected virtual void Awake()
        {
            resourcePieces = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
            foreach(var piece in resourcePieces)
            {
                piece.enabled = true;
            }
        }

        public abstract void VisualDeplete(int count);
        
        public virtual void VisualDepleteAll() 
        {
            foreach(var piece in resourcePieces)
            {
                piece.enabled = false;
            }
        }
    }
}
