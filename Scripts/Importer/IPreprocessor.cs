namespace Importer
{
    public interface IPreprocessor<T, J> where J : PreprocessorData
    {
        T Preprocess(T toProcess, J preprocessorData);
    }

    public abstract class PreprocessorData
    {
    }
}
