using System;
using System.Collections.Generic;
using MG_Utilities;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Resources
{
    public class WorldResourcesManager : Singleton<WorldResourcesManager>
    {
        [SerializeField] LayerMask resourceLayerMask;
        [SerializeField] GameObject cursor1;
        [SerializeField] GameObject cursor2;
        [SerializeField] bool drawColliders_DEBUG;

        public event Action<IGatherable> OnResourceSelected;
        public event Action<IGatherable> OnResourceDeselected;
        public event Action OnSelectedResourcesChanged;

        //TODO: Add global view manager to handle this
        public event Action OnResourceViewEnter;
        public event Action OnResourceViewExit;

        readonly List<IGatherable> _selectedResources = new();

        Vector3 _mouseInitialPosition;
        Vector3 _mouseFinalPosition;

        bool _isViewActive;
        bool _deselecting;
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                if(_isViewActive)
                {
                    _isViewActive = false;
                    OnResourceViewExit?.Invoke();
                }
                else
                {
                    _isViewActive = true;
                    OnResourceViewEnter?.Invoke();
                }
            }

            if(!_isViewActive) return;

            if(Input.GetMouseButtonDown(0))
            {
                _mouseInitialPosition = CameraController.Instance.GetWorldMousePosition();
                cursor1.transform.position = _mouseInitialPosition;
                _deselecting = false;
            }

            if(Input.GetMouseButtonDown(1))
            {
                _mouseInitialPosition = CameraController.Instance.GetWorldMousePosition();
                cursor1.transform.position = _mouseInitialPosition;
                _deselecting = true;
            }

            if(Input.GetMouseButton(0))
            {
                cursor2.transform.position = CameraController.Instance.GetWorldMousePosition();
            }

            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                _mouseFinalPosition = CameraController.Instance.GetWorldMousePosition();
                cursor2.transform.position = _mouseFinalPosition;
                CalculateAreaSelection();
                _deselecting = false;
            }
        }

        void OnDrawGizmos()
        {
            if (drawColliders_DEBUG)
            {
                foreach(IGatherable g in _selectedResources)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(((MonoBehaviour)g).GetComponent<Collider>().bounds.center, 1f);
                }
            }
        }
        
        private void CalculateAreaSelection()
        {
            Vector3 sizeOfArea = (_mouseFinalPosition - _mouseInitialPosition).With(y: 0f);
            var halfExtents = (sizeOfArea * 0.5f);
            halfExtents.x = Mathf.Abs(halfExtents.x);
            halfExtents.y = Mathf.Abs(halfExtents.y);
            halfExtents.z = Mathf.Abs(halfExtents.z);
            
            Collider[] foundResources = Physics.OverlapBox((sizeOfArea / 2) + _mouseInitialPosition,
                halfExtents, Quaternion.identity, resourceLayerMask);

            Debug.Log($"Selected {foundResources.Length} resources");
            foreach(var resource in foundResources)
            {
                if(resource != null && resource.TryGetComponent(out IGatherable gatherable))
                {
                    if(_deselecting)
                    {
                        if(_selectedResources.Contains(gatherable))
                        {
                            _selectedResources.Remove(gatherable);
                            OnResourceDeselected?.Invoke(gatherable);
                        }
                    }
                    else
                    {
                        if(!_selectedResources.Contains(gatherable))
                        {
                            _selectedResources.Add(gatherable);
                            OnResourceSelected?.Invoke(gatherable);
                        }
                    }

                    OnSelectedResourcesChanged?.Invoke();
                }
            }
        }
    }
}
