using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

namespace UI.Views
{
    public class ModelPreview : MonoBehaviour
    {
        private const float DragSpeedModifierRotate = 8;
        private const float DragSpeedModifier= .008f;
        private const float CameraScrollSmoothness = 8;
        private const float ScrollSensitivity = 0.002f;
        private const float MinCameraSize = 1.2f;
        private const float MaxCameraSize = 3.7f;
        
        private Vector3 dragRotateTo;
        private Vector3 dragTo;
        private float cameraSize;
        private PlayerInput input;
        private bool isDraggingRotate;
        private bool isDragging;

        private Transform fbxModelTransform;
        private InputAction mousePosition;
        private Transform cameraTransform;
        private Camera detailCamera;
 

        [Inject]
        public void Construct(PlayerInput input)
        {
            this.input = input;
            input.actions["Drag"].performed += HandleDrag;
            input.actions["Hold"].performed += HandleHold;
            input.actions["Zoom"].performed += HandleZoom;
            input.actions["HoldRight"].performed += HandleHoldRight;
            
            mousePosition = input.actions["MousePosition"];
            cameraTransform = GameObject.FindWithTag("DetailCamera").transform;
            detailCamera = cameraTransform.GetComponent<Camera>();
            cameraSize = detailCamera.orthographicSize;
        }

        private void Update()
        {
            detailCamera.orthographicSize =
                Mathf.Lerp(detailCamera.orthographicSize, cameraSize, Time.deltaTime * CameraScrollSmoothness);
            if (isDragging)
            {
                cameraTransform.position = cameraTransform.position
                    .IncreaseX(dragTo.x)
                    .IncreaseY(dragTo.y)
                    .ClampX(-(MaxCameraSize - cameraSize), MaxCameraSize - cameraSize)
                    .ClampY(-(MaxCameraSize - cameraSize), MaxCameraSize - cameraSize);

            }
            
            if (fbxModelTransform == null) return;

            if (isDraggingRotate)
            {
                fbxModelTransform.Rotate(fbxModelTransform.InverseTransformVector(Vector3.up), dragRotateTo.x);
                fbxModelTransform.Rotate(fbxModelTransform.InverseTransformVector(Vector3.right), -dragRotateTo.y);
            }
        }

        private void OnDestroy()
        {
            if (input != null)
            {
                input.actions["Drag"].performed -= HandleDrag;
                input.actions["Hold"].performed -= HandleHold;
                input.actions["HoldRight"].performed -= HandleHoldRight;
                input.actions["Zoom"].performed -= HandleZoom;
            }
        }

        private void HandleHoldRight(InputAction.CallbackContext context)
        {
            if (context.valueType == typeof(Single))
            {
                var pointerEventData = new PointerEventData(EventSystem.current) {position = mousePosition.ReadValue<Vector2>()};
                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);
                
                isDragging = context.ReadValue<Single>() > 0 && raycastResults.Count > 0 && raycastResults[0].gameObject.layer == LayerMask.NameToLayer("UICharacterModel");
                if (isDragging)
                    dragRotateTo = Vector3.zero;
            }
        }

        private void HandleZoom(InputAction.CallbackContext context)
        {
             cameraSize = Mathf.Clamp(cameraSize - context.ReadValue<Vector2>().y * ScrollSensitivity, MinCameraSize, MaxCameraSize);
        }

        private void HandleHold(InputAction.CallbackContext context)
        {
            if (context.valueType == typeof(Single))
            {
                var pointerEventData = new PointerEventData(EventSystem.current) {position = mousePosition.ReadValue<Vector2>()};
                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);
                
                isDraggingRotate = context.ReadValue<Single>() > 0 && raycastResults.Count > 0 && raycastResults[0].gameObject.layer == LayerMask.NameToLayer("UICharacterModel");
                if (isDraggingRotate)
                    dragTo = Vector3.zero;
            }
        }

        private void HandleDrag(InputAction.CallbackContext context)
        {
            if (isDraggingRotate || isDragging) 
            {
                dragRotateTo = context.ReadValue<Vector2>() * -DragSpeedModifierRotate * Mathf.Deg2Rad;
                dragTo = context.ReadValue<Vector2>() * DragSpeedModifier;
            }
        }

        public void SetModelTransform(Transform modelTransform)
        {
            fbxModelTransform = modelTransform;
        }

        public void ResetRotation()
        {
            fbxModelTransform.rotation = Quaternion.identity;
            fbxModelTransform.Rotate(Vector3.up, 180);
        }
    }
}
