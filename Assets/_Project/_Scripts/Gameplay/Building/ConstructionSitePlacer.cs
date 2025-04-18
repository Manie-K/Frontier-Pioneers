using System.Collections.Generic;
using MG_Utilities;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{   
    //We don't need Instance, but we need to make sure only one MonoBehaviour exists
    public class ConstructionSitePlacer : Singleton<ConstructionSitePlacer>
    {
        [Header("Settings")]
        [SerializeField] List<ConstructionSiteController> availableConstructions;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float rotationDelta = 3f;
        
        [Header("Materials")]
        [SerializeField] Material buildingAllowedMaterial;
        [SerializeField] Material buildingNotAllowedMaterial;

        [Header("References")]
        [SerializeField] Transform constructionSitesTransform;
        
        BoxCollider _buildingCollider;
        Mesh _mesh;
        Quaternion _currentRotation = Quaternion.identity;
        Vector3 _position;
        
        int _currentIndex = 0;
        bool _buildingViewEnabled; //TODO: Add global view manager
        
        void Start()
        {
            _buildingCollider = availableConstructions[_currentIndex].FinishedBuildingCollider;
            _mesh = availableConstructions[_currentIndex].FinishedBuildingMesh;
        }
        
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                _buildingViewEnabled = !_buildingViewEnabled;
            }
            if(!_buildingViewEnabled)
                return;

            if(Input.GetKeyDown(KeyCode.C))
            {
                _currentIndex = (_currentIndex + 1) % availableConstructions.Count;
                _buildingCollider = availableConstructions[_currentIndex].FinishedBuildingCollider;
                _mesh = availableConstructions[_currentIndex].FinishedBuildingMesh;
            }
            
            if (Input.GetKey(KeyCode.M))
                _currentRotation *= Quaternion.AngleAxis(rotationDelta, Vector3.up);
            if (Input.GetKey(KeyCode.N))
                _currentRotation *= Quaternion.AngleAxis(-rotationDelta, Vector3.up);
            
            _currentRotation.Normalize();
            _position = CameraController.Instance.GetWorldMousePosition();

            if(Input.GetMouseButtonDown(0) && IsConstructionValid())
            {   
                var placed = Instantiate(availableConstructions[_currentIndex], 
                    _position, _currentRotation, constructionSitesTransform);
                
                _buildingViewEnabled = false;
                ConstructionSiteManager.Instance.RegisterConstructionSite(placed);
            }
            else if(IsConstructionValid()) {
                Graphics.DrawMesh(_mesh, _position, _currentRotation, buildingAllowedMaterial, 0);
            }
            else {
                Graphics.DrawMesh(_mesh, _position, _currentRotation, buildingNotAllowedMaterial, 0);
            }
        }

        bool IsConstructionValid()
        {
            Collider[] hits = Physics.OverlapBox(_position + _buildingCollider.center,
                _buildingCollider.size / 2, _currentRotation, layerMask);

            return hits.Length == 0;
        }
    }
}