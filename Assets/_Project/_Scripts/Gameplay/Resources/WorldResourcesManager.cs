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
        [SerializeField] GameObject areaVisual;
        [SerializeField] Material selectingMaterial;
        [SerializeField] Material deselectingMaterial;
        [SerializeField] bool drawColliders_DEBUG;

        public List<IGatherable> SelectedResources => _selectedResources;
        public event Action<IGatherable> OnResourceSelected; //To signal single resource selection, for visual purpose etc.
        public event Action<IGatherable> OnResourceDeselected;
        public event Action OnSelectedResourcesChanged; //To signal that the collection has changed, use it for monitoring.
        
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

            {
                bool isDeselecting = false;
                if(Input.GetMouseButtonDown(0) || (isDeselecting = Input.GetMouseButtonDown(1)))
                {
                    _mouseInitialPosition = CameraController.Instance.GetWorldMousePosition();
                    cursor1.transform.position = _mouseInitialPosition;
                    _deselecting = false;
                    areaVisual.SetActive(true);
                    areaVisual.GetComponent<MeshRenderer>().material = selectingMaterial;
                    cursor1.SetActive(true);
                    cursor2.SetActive(true);
                    _deselecting = isDeselecting;
                    areaVisual.GetComponent<MeshRenderer>().material =
                        isDeselecting ? deselectingMaterial : selectingMaterial;
                }
            }

            if(Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                cursor2.transform.position = CameraController.Instance.GetWorldMousePosition();
                Vector3 start = _mouseInitialPosition;
                Vector3 end = cursor2.transform.position;
                
                Vector3 center = (start + end) / 2f;
                Vector3 size = new Vector3(Mathf.Abs(end.x - start.x), 1, Mathf.Abs(end.z - start.z));

                areaVisual.transform.position = center.With(y: 0.25f);
                areaVisual.transform.rotation = Quaternion.Euler(0, 0, 0);
                //Since Unity's plane is 10x10, we divide size by 10
                areaVisual.transform.localScale = new Vector3(size.x / 10f, 1, size.z / 10f);
            }

            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                _mouseFinalPosition = CameraController.Instance.GetWorldMousePosition();
                cursor2.transform.position = _mouseFinalPosition;
                CalculateAreaSelection();
                _deselecting = false;
                areaVisual.SetActive(false);
                cursor1.SetActive(false);
                cursor2.SetActive(false);
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
                            gatherable.OnGatherableDepleted -= OnGatherableDepleted_Delegate;
                            _selectedResources.Remove(gatherable);
                            OnResourceDeselected?.Invoke(gatherable);
                        }
                    }
                    else
                    {
                        if(!_selectedResources.Contains(gatherable))
                        {
                            gatherable.OnGatherableDepleted += OnGatherableDepleted_Delegate;
                            _selectedResources.Add(gatherable);
                            OnResourceSelected?.Invoke(gatherable);
                        }
                    }
                }
            }
            OnSelectedResourcesChanged?.Invoke();
        }

        void OnGatherableDepleted_Delegate(IGatherable gatherable)
        {
            if(_selectedResources.Contains(gatherable))
            {
                _selectedResources.Remove(gatherable);
                OnSelectedResourcesChanged?.Invoke();
            }
        }
    }
}
