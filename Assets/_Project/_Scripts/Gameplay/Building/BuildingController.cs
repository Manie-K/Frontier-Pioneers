using System;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    public class BuildingController : MonoBehaviour
    {
        public IStorage Storage => _storage;
        
        
        IStorage _storage;

        void Start()
        {
            _storage = GetComponent<IStorage>();
            if (_storage == null)
            {
                Debug.LogError("No storage component found on the building.");
            }
        }
    }
}
