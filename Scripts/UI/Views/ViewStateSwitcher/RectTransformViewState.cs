using System;
using UnityEngine;

namespace UI.Views
{
    [Serializable]
    public struct RectTransformData
    {
        public Vector2 AnchoredPosition;
        public Vector3 LocalScale;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
    }

    [Serializable]
    public class RectTransformViewStateData : ViewStateData<RectTransform, RectTransformData>
    {
        public override void Apply(RectTransform target)
        {
            target.anchoredPosition = Value.AnchoredPosition;
            target.localScale = Value.LocalScale;
            target.anchorMin = Value.AnchorMin;
            target.anchorMax = Value.AnchorMax;
            target.pivot = Value.Pivot;
        }
    }

    [Serializable]
    public class
        RectTransformViewStatesHolder : ViewStatesHolder<RectTransform, RectTransformData, RectTransformViewStateData>
    {
    }

    public class RectTransformViewState : ViewState<RectTransformViewStatesHolder>
    {
    }
}