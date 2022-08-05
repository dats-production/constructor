namespace Constructor.Validator.Validations
{
    public interface IValidation<T, R>
    {
        bool Validate(T t, out R r);
        string GetDescription();
    }
}