using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Constructor;
using Constructor.DataStorage;
using Constructor.Details;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Cysharp.Threading.Tasks;
using Services.LocalizationService;
using UI.Factories;
using UI.Navigation;
using UniRx;
using UnityEngine;
using WalkerAlias;
using Random = UnityEngine.Random;
using Zenject;

namespace UI.Models
{
    public interface IGenerateCharactersButtonModel
    {
        IObservable<IButtonModel> Button { get; }
    }

    public class GenerateCharactersButtonModel : IGenerateCharactersButtonModel
    {
        private IReadOnlyList<Layer> _layers;

        private readonly IInputModalWindow inputModalWindow;
        private readonly IDataStorage dataStorage;
        private readonly IUIBlocker uiBlocker;
        private readonly IUINavigator uiNavigator;
        private readonly ILocalizationService localizationService;
        private readonly Validator<Layer, List<Detail>> layerValidator;
        private readonly Validator<string, string> stringValidator;

        public GenerateCharactersButtonModel(IInputModalWindow inputModalWindow,
            IDataStorage dataStorage, IUIBlocker uiBlocker,
            ValidatorFactory<Layer, List<Detail>> layerValidatorFactory, 
            ValidatorFactory<string, string> stringValidatorFactory, 
            DiContainer diContainer, IUINavigator uiNavigator, 
            ILocalizationService localizationService)
        {
            this.inputModalWindow = inputModalWindow;
            this.dataStorage = dataStorage;
            this.uiBlocker = uiBlocker;
            this.uiNavigator = uiNavigator;
            this.localizationService = localizationService;

            var layerValidationList = new List<IValidation<Layer, List<Detail>>>
                { diContainer.Instantiate<RarityValidation>() };
            layerValidator = layerValidatorFactory.Create(layerValidationList);
            
            var stringValidationList = new List<IValidation<string, string>> 
                { diContainer.Instantiate<GeneratedCharactersCountInputValidation>() };
            stringValidator = stringValidatorFactory.Create(stringValidationList);
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(GenerateCharacters).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });

        private async void GenerateCharacters(Unit _)
        {
            foreach (var layer in dataStorage.Layers)
                if(!layerValidator.Validate(layer, out var layerValidationFailDescriptions))
                {
                    localizationService.SetStringVariable("layerName", layer.Name);
                    Debug.LogError(localizationService.Localize("The sum of the () layer's rarities is not equal to 100%."));
                    return;
                }
            
            var layers = dataStorage.Layers;
            if (layers.Count == 0)
            {
                    Debug.LogError(localizationService.Localize("You need to import Layers first."));
                    return;
            }

            var enteredCountText = await inputModalWindow
                .Show(localizationService.Localize("Enter characters count"));
            if (stringValidator.Validate(enteredCountText, out var validationFailDescriptions))
            {
                inputModalWindow.Hide();
            }
            else
            {
                inputModalWindow.ToggleWarningTooltip(validationFailDescriptions);
                GenerateCharacters(_);
            }
            
            if (!int.TryParse(enteredCountText, out var count)) return;

            using var cts = new CancellationTokenSource();
            using var progressSubject = new Subject<float>();
            const float progressStep = 0.01f;
            var lastProgress = 0f;
            progressSubject.OnNext(lastProgress);
            uiBlocker.Show(progressSubject, cts.Cancel);
            
            var onePercent = count * 0.01f;
            var characters = new HashSet<Character>();
            var detailsPools = new Dictionary<string, List<Detail>>();
            foreach (var layer in layers)
            {
                var detailList = new List<Detail>();
                foreach (var detail in layer.Details)
                {
                    var requiredCount = Mathf.RoundToInt(detail.Rarity.Value * onePercent);
                    for (var i = 0; i < requiredCount; i++) detailList.Add(detail);
                }

                Shuffle(detailList);
                detailsPools.Add(layer.Name, detailList);
            }
            
            void Stop()
            {
                uiBlocker.Hide();
                progressSubject.OnCompleted();
            }

            var charactersCount = characters.Count;
            while (charactersCount < count || cts.IsCancellationRequested)
            {
                if (cts.IsCancellationRequested)
                {
                    Stop();
                    return;
                }
                
                var newCharacter = new Character($"Character_{characters.Count}");
                foreach (var detailsPool in detailsPools)
                {
                    newCharacter.SetDetail(detailsPool.Key, detailsPool.Value[Random.Range(0, detailsPool.Value.Count)]);
                }

                characters.Add(newCharacter);
                charactersCount = characters.Count;
                var progress = (float)charactersCount / count;
                if (progress - lastProgress >= progressStep)
                {
                    progressSubject.OnNext(progress);
                    lastProgress = progress;
                    await UniTask.DelayFrame(1, cancellationToken: cts.Token).SuppressCancellationThrow();
                }
            }

            dataStorage.AddCharacters(characters.ToArray());
            for (var i = 0; i < dataStorage.Layers[0].Details.Count; i++)
            {
                var back = dataStorage.Layers[0].Details[i];
                var testCharacter = new Character("Char" + i);
                testCharacter.SetDetail(dataStorage.Layers[0].Name, back);
                for (var j = 1; j < dataStorage.Layers.Count; j++)
                {
                    var layer = dataStorage.Layers[j];
                    testCharacter.SetDetail(layer.Name, layer.Details[0]);
                }

                dataStorage.AddCharacter(testCharacter);
            }

            Stop();
            uiNavigator.OpenCollectionPreviewScreen();
        }

        private void Shuffle<T>(IList<T> list)
        {
            var rng = new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        private async void GenerateCharactersWalkerAlias(Unit _)
        {
            var layers = dataStorage.Layers;
            if (layers.Count == 0)
            {
                Debug.LogError(localizationService.Localize("You need to import layers first."));
                return;
            }

            var enteredCountText = await inputModalWindow
                .Show(localizationService.Localize("Enter characters count"));
            if (stringValidator.Validate(enteredCountText, out var validationFailDescriptions))
            {
                inputModalWindow.Hide();
            }
            else
            {
                inputModalWindow.ToggleWarningTooltip(validationFailDescriptions);
                GenerateCharacters(_);
            }
            
            if (!int.TryParse(enteredCountText, out var count)) return;
            
            var partitions = layers.ToDictionary(x => x.Name,
                x =>
                {
                    var values = x.Details
                        .Select(detail => new KeyValuePair<Detail, float>(detail, detail.Rarity.Value)).ToList();
                    return new WalkerAlias<Detail>(values);
                });

            using var cts = new CancellationTokenSource();
            using var progressSubject = new Subject<float>();
            const float progressStep = 0.01f;
            var lastProgress = 0f;
            progressSubject.OnNext(lastProgress);
            uiBlocker.Show(progressSubject, cts.Cancel);
            
            void Stop()
            {
                uiBlocker.Hide();
                progressSubject.OnCompleted();
            }
            
            var charactersHashSet = new HashSet<Character>();
            while (charactersHashSet.Count <  count || cts.IsCancellationRequested)
            {
                if (cts.IsCancellationRequested)
                {
                    Stop();
                    return;
                }
                
                var progress = (float)charactersHashSet.Count / count;
                if (progress - lastProgress >= progressStep)
                {
                    progressSubject.OnNext(progress);
                    lastProgress = progress;
                    await UniTask.DelayFrame(1, cancellationToken: cts.Token).SuppressCancellationThrow();
                }

                var character = new Character($"Character_{charactersHashSet.Count}");
                foreach (var pair in partitions)
                {
                    var detail = pair.Value.Sample();
                    character.SetDetail(pair.Key, detail);
                }

                charactersHashSet.Add(character);
            }
            dataStorage.AddCharacters(charactersHashSet.ToArray());
            Stop();
        }
    }
}