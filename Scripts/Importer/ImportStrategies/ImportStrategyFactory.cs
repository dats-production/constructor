using System;
using Constructor.Details;
using Zenject;

namespace Importer.ImportStrategies
{
    public class ImportStrategyFactory : PlaceholderFactory<Type, ILayerImportStrategy>
    {
        private ILayerImportStrategy fbxLayerImportStrategy;
        private ILayerImportStrategy colorLayerImportStrategy;
        private ILayerImportStrategy textureLayerImportStrategy;
        private ILayerImportStrategy backgroundLayerImportStrategy;
        
        [Inject]
        public ImportStrategyFactory (DiContainer diContainer)
        {
            fbxLayerImportStrategy = diContainer.Instantiate<FbxLayerImportStrategy>();
            colorLayerImportStrategy = diContainer.Instantiate<ColorLayerImportStrategy>();
            textureLayerImportStrategy = diContainer.Instantiate<TextureLayerImportStrategy>();
            backgroundLayerImportStrategy = diContainer.Instantiate<BackgroundLayerImportStrategy>();
        }
        
        public override ILayerImportStrategy Create(Type layerType)
        {
            if (layerType == typeof(FbxDetail))
                return fbxLayerImportStrategy;

            if (layerType == typeof(ColorDetail))
                return colorLayerImportStrategy;

            if (layerType == typeof(TextureDetail))
                return textureLayerImportStrategy;
            
            if (layerType == typeof(BackgroundDetail))
                return backgroundLayerImportStrategy;

            throw new ArgumentException($"{layerType} is not typeof Detail");
        }
    }
}
