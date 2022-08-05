namespace UI.Views
{
    public interface IBindable<in T>
    {
        void Bind(T model);
    }
}