using System;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Resources
{
    public class SelectedResourceVisual : MonoBehaviour
    {
        IGatherable _parentResource;
        Renderer _renderer;
        
        bool _resourceViewActive = false; //TODO: Add global view manager to handle this
        bool _isSelected = false;
        void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _parentResource = transform.parent.GetComponent<IGatherable>();
        }

        void Start()
        {
            WorldResourcesManager.Instance.OnResourceSelected += OnResourceSelected;
            WorldResourcesManager.Instance.OnResourceDeselected += OnResourceDeselected;
            
            WorldResourcesManager.Instance.OnResourceViewEnter += OnResourceViewEnter;
            WorldResourcesManager.Instance.OnResourceViewExit += OnResourceViewExit;
        }

        void OnDisable()
        {
            if(WorldResourcesManager.Instance == null) return;
            WorldResourcesManager.Instance.OnResourceSelected -= OnResourceSelected;
            WorldResourcesManager.Instance.OnResourceDeselected -= OnResourceDeselected;

            WorldResourcesManager.Instance.OnResourceViewEnter -= OnResourceViewEnter;
            WorldResourcesManager.Instance.OnResourceViewExit -= OnResourceViewExit;
        }

        void OnResourceViewEnter()
        {
            _resourceViewActive = true;
            if(_isSelected)
            {
                _renderer.enabled = true;
            }
        }
        void OnResourceViewExit()
        {
            _resourceViewActive = false;
            _renderer.enabled = false;
        }
        void OnResourceSelected(IGatherable gatherable)
        {
            if(gatherable == _parentResource)
            {
                _isSelected = true;
                if(_resourceViewActive)
                {
                    _renderer.enabled = true;
                }
            }
        }
        void OnResourceDeselected(IGatherable gatherable)
        {
            if(gatherable == _parentResource)
            {
                _isSelected = false;
                _renderer.enabled = false;
            }
        }

    }
}