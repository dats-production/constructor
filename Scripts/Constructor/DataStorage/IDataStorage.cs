using System;
using System.Collections.Generic;
using System.Linq;
using Constructor.Details;
using UniRx;
using UnityEngine;

namespace Constructor.DataStorage
{
    public interface IDataStorage
    {
        IReadOnlyReactiveCollection<ICharacter> Characters { get; }
        IReadOnlyReactiveCollection<Layer> Layers { get; }
        IObservable<Layer> OnLayerChanged { get; }
        void AddCharacter(Character character);
        void ReplaceCharacter(ICharacter character);
        void AddLayer(Layer layer);
        void ChangeDetail(Layer layer, int detailIndex, DetailBody detailBody);
        float GetDetailActualRarity(Detail detail);
        void AddCharacters(Character[] newCharacters);
        void RemoveCharacters();
        void RemoveLayers();
        IDataStorage Copy();
    }

    public class DataStorage : IDataStorage, IDisposable
    {
        public IReadOnlyReactiveCollection<ICharacter> Characters => characters;
        public IReadOnlyReactiveCollection<Layer> Layers => layers;
        public IObservable<Layer> OnLayerChanged => onLayerChanged;

        private ReactiveCollection<ICharacter> characters = new ReactiveCollection<ICharacter>();
        private ReactiveCollection<Layer> layers = new();
        private ReactiveCommand<Layer> onLayerChanged = new();

        public void AddCharacters(Character[] newCharacters)
        {
            foreach (var character in newCharacters)
            {
                characters.Add(character);
            }
        }

        public void RemoveCharacters() => characters.Clear();

        public IDataStorage Copy()
        {
            var dataStorage = new DataStorage();

            foreach (var character in characters)
            {
                dataStorage.characters.Add(character.Copy());
            }

            foreach (var layer in layers)
            {
                dataStorage.AddLayer(layer);
            }

            return dataStorage;
        }

        public void AddCharacter(Character character)
        {
            characters.Insert(0, character);
        }
        
        public void ReplaceCharacter(ICharacter character)
        {
            var characterInStorage = characters.First(x => x.Name.Value.Equals(character.Name.Value));
            var index = characters.IndexOf(characterInStorage);
            characters[index] = character;
        }

        public void AddLayer(Layer layer)
        {
            layers.Add(layer);
        }

        public void ChangeDetail(Layer layer, int detailIndex, DetailBody detailBody)
        {
            var detail = layer.Details[detailIndex];
            detail.Name.Subscribe(_ => onLayerChanged.Execute(layer));
            detail.Rarity.Subscribe(_ => onLayerChanged.Execute(layer));
            switch (detail)
            {
                case FbxDetail fbxDetail:
                    var fbxDetailBody = (FBXDetailBody)detailBody;
                    fbxDetail.modelContainerObject = fbxDetailBody.modelContainer;
                    fbxDetail.modelGameObject = fbxDetailBody.modelGameObject;
                    fbxDetail.baseColors = fbxDetailBody.texturesForDetail;
                    UpdateTexturesLayer();
                    break;
                case TextureDetail:
                    UpdateTexturesLayer();
                    break;
                case ColorDetail colorDetail:
                    var colorDetailBody = (ColorDetailBody)detailBody;
                    colorDetail.sampleGameObject = colorDetailBody.modelContainer;
                    break;
                case BackgroundDetail backgroundDetail:
                    var backgroundDetailBody = (BackgroundDetailBody)detailBody;
                    backgroundDetail.background = backgroundDetailBody.background;
                    break;
            }

            onLayerChanged.Execute(layer);
        }

        public float GetDetailActualRarity(Detail detail)
        {
            var layer = layers.First(l => l.Details.Contains(detail));
            var countOfCurrentDetail = characters.Count(c => c.Details[layer.Name].Equals(detail));

            return 100f / characters.Count * countOfCurrentDetail;
        }

        public void RemoveLayers() => layers.Clear();
        
        private void UpdateTexturesLayer()
        {
            var texturesLayer = layers.FirstOrDefault(l => l.Details.Count > 0 && l.Details[0] is TextureDetail);
            var fbxsLayer = layers.FirstOrDefault(l => l.Details.Count > 0 && l.Details[0] is FbxDetail detail && detail.baseColors.Count > 0);
            
            if (texturesLayer == null || fbxsLayer == null) return;
            
            var anyFbxDetail = (FbxDetail)fbxsLayer.Details[0];

            for (var i = 0; i < anyFbxDetail.baseColors.Count; i++)
            {
                var texture = anyFbxDetail.baseColors[i];
                if (i == texturesLayer.Details.Count)
                    texturesLayer.AddDetail(new TextureDetail(texture.name, 0, texturesLayer.Name));
                else
                    texturesLayer.Details[i].Name.Value = texture.name;
            }
            onLayerChanged.Execute(texturesLayer);
        }

        public void Dispose()
        {
            characters?.Dispose();
            layers?.Dispose();
            onLayerChanged?.Dispose();
        }
    }
    
    public abstract class DetailBody{}

    public class FBXDetailBody : DetailBody
    {
        public GameObject modelContainer;
        public GameObject modelGameObject;
        public List<Texture2D> texturesForDetail = new List<Texture2D>();
    }

    public class ColorDetailBody : DetailBody
    {
        public GameObject modelContainer;
    }
    
    public class BackgroundDetailBody : DetailBody
    {
        public Texture2D background;
    }
}