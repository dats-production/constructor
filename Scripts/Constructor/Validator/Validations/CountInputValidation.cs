using Constructor.DataStorage;
using Zenject;

namespace Constructor.Validator.Validations
{
    public abstract class CountInputValidation : IStringValidation
    {
        protected const int minGenerations = 1;
        
        protected IDataStorage dataStorage;
        protected int inputCount;
        protected int maxGenerations;
        
        [Inject]
        public void Constructor(IDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
        }

        public abstract bool Validate(string inputText, out string textOut);

        public abstract string GetDescription();
    }
}