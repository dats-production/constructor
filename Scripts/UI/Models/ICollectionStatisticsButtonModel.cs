using System;
using UI.Views;
using UniRx;

namespace UI.Models
{
    public interface ICollectionStatisticsButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class CollectionStatisticsButtonModel : ICollectionStatisticsButtonModel
    {
        private readonly CollectionStatisticsView collectionStatisticsView;

        public CollectionStatisticsButtonModel(CollectionStatisticsView collectionStatisticsView)
        {
            this.collectionStatisticsView = collectionStatisticsView;
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(_ =>
                {
                    collectionStatisticsView.SetSheet();
                    collectionStatisticsView.gameObject.SetActive(true);
                }).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}