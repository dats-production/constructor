namespace Constructor.Validator.Validations
{
    public interface IStringValidation : IValidation<string, string>
    {
        new bool Validate(string text, out string textOut);
    }
}