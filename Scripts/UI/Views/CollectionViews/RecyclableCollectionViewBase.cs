using System.Collections.Generic;
using Common;
using Configs;
using EnhancedUI.EnhancedScroller;
using UI.Models;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    [RequireComponent(typeof(EnhancedScroller))]
    public abstract class RecyclableCollectionViewBase<TItemModel, TItemView> : MonoBehaviour, IEnhancedScrollerDelegate
        where TItemModel : IUIViewModel
        where TItemView : IBindable<TItemModel>
    {
        [SerializeField] private EnhancedScroller enhancedScroller;

        private IReadOnlyList<TItemModel> items;
        private IProvider<IUIViewModel, UIViewData> uiViewDataProvider;

        [Inject]
        public void Construct(IProvider<IUIViewModel, UIViewData> uiViewDataProvider)
        {
            this.uiViewDataProvider = uiViewDataProvider;
        }

        private void Start()
        {
            enhancedScroller.Delegate = this;
        }

        public void Bind(IReadOnlyList<TItemModel> items)
        {
            this.items = items;
            enhancedScroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return items?.Count ?? 0;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return uiViewDataProvider.Get(items[dataIndex]).Size;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var item = items[dataIndex];
            var data = uiViewDataProvider.Get(item);
            var cellView = enhancedScroller.GetCellView(data.CellPrefab);
            cellView.GetComponent<TItemView>().Bind(item);
            return cellView;
        }
    }
}