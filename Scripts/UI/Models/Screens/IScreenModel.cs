using UI.Views.Screens;

namespace UI.Models.Screens
{
    public enum ScreenType
    {
        GenerationSettings,
        CollectionPreview,
        EditCharacter
    }

    public interface IScreenModel
    {
        ScreenType Type { get; }
        void Close();
    }

    public interface IScreenModel<in T> : IScreenModel where T : IScreenArgs
    {
        void Open(T args = default);
    }
}