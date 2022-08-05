using System;
using System.Collections.Generic;
using Constructor;
using Constructor.DataStorage;
using UI.Views;
using UniRx;

namespace UI.Models
{
    public interface IResetRotationButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class ResetRotationButtonModel : IResetRotationButtonModel
    {
        private readonly ModelPreview fbxView;

        public ResetRotationButtonModel(ModelPreview fbxView)
        {
            this.fbxView = fbxView;
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(ResetRotation).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });

        private void ResetRotation(Unit _)
        {
            fbxView.ResetRotation();
        }
    }
}