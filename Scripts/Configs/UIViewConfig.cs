using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using EnhancedUI.EnhancedScroller;
using UI.Models;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class UIViewData
    {
        [SerializeField] private UIViewType type;
        [SerializeField] private EnhancedScrollerCellView cellPrefab;
        [SerializeField] private float size;

        public UIViewType Type => type;
        public EnhancedScrollerCellView CellPrefab => cellPrefab;
        public float Size => size;
    }

    public class UIViewConfig : ScriptableObject, IProvider<IUIViewModel, UIViewData>
    {
        [SerializeField] private List<UIViewData> uiViewData;

        public UIViewData Get(IUIViewModel model)
        {
            var data = uiViewData.FirstOrDefault(x => x.Type == model.Type);
            return data;
        }
    }
}