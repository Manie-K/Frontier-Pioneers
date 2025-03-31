using System;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Resources
{
    public class VisualDepletion : MonoBehaviour
    {
        public int AllResourcePiecesCount => _resourcePieces.Length;
        public int VisibleResourcePiecesCount => Array.FindAll(_resourcePieces, x => x.enabled).Length;
        public float CurrentPercentage => (float)VisibleResourcePiecesCount / AllResourcePiecesCount;
        
        MeshRenderer[] _resourcePieces;
        float _onePiecePercentage;
        
        protected void Awake()
        {
            _resourcePieces = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
            foreach(var piece in _resourcePieces)
            {
                piece.enabled = true;
            }
            
            _onePiecePercentage = 1f / _resourcePieces.Length;
        }

        void Start()
        {
            transform.parent.GetComponent<ResourceNode>().OnSpecialResourceCollected += SetVisualPercentage;
        }

        void OnDisable()
        {
            transform.parent.GetComponent<ResourceNode>().OnSpecialResourceCollected -= SetVisualPercentage;
        }

        void SetVisualPercentage(float percentage)
        {
            if (percentage is < 0f or > 1f)
            {
                throw new ArgumentException($"Invalid percentage value -> {percentage}");
            }

            int piecesVisible = 0;
            float tempPercentage = percentage;
            
            if(tempPercentage < _onePiecePercentage && tempPercentage > 0)
            {
                piecesVisible = 1;
            }
            else
            {
                while(true)
                {
                    if(tempPercentage >= _onePiecePercentage)
                    {
                        piecesVisible++;
                        tempPercentage -= _onePiecePercentage;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            int index = 0;
            for (; index < piecesVisible; index++)
            {
                _resourcePieces[index].enabled = true;
            }
            for(;index < _resourcePieces.Length; index++)
            {
                _resourcePieces[index].enabled = false;
            }
        }
        
        public void VisualDepleteAll() 
        {
            foreach(var piece in _resourcePieces)
            {
                piece.enabled = false;
            }
        }
    }
}
