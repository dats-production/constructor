namespace Common
{
    public interface IProvider<in TArg, out TResult>
    {
        TResult Get(TArg arg);
    }
}