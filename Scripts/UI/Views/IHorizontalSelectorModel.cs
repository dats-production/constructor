using System;
using System.Collections.Generic;
using UI.Models;
using UniRx;

namespace UI.Views
{
    public interface IHorizontalSelectorModel : IUIViewModel
    {
        IReactiveProperty<int> OnSelect { get; }
        IReadOnlyList<string> Items { get; }
        string LayerName { get; }
    }

    public class HorizontalSelectorModel : IHorizontalSelectorModel, IDisposable
    {
        public UIViewType Type => UIViewType.HorizontalSelector;
        public IReactiveProperty<int> OnSelect => onSelect;
        public IReadOnlyList<string> Items { get; }
        public string LayerName { get; }

        private readonly ReactiveProperty<int> onSelect;

        public HorizontalSelectorModel(IReadOnlyList<string> items, int selectedIndex, string layerName)
        {
            Items = items;
            onSelect = new ReactiveProperty<int>(selectedIndex);
            LayerName = layerName;
        }

        public void Dispose()
        {
            onSelect?.Dispose();
        }
    }
}