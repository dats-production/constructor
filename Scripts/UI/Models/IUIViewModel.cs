namespace UI.Models
{
    public enum UIViewType
    {
        CharacterInfoButton,
        HorizontalSelector
    }

    public interface IUIViewModel
    {
        UIViewType Type { get; }
    }
}