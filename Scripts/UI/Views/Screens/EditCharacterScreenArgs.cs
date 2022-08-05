using Constructor;

namespace UI.Views.Screens
{
    public sealed class EditCharacterScreenArgs : IScreenArgs
    {
        public readonly ICharacter Character;

        public EditCharacterScreenArgs(ICharacter character)
        {
            Character = character;
        }
    }
}