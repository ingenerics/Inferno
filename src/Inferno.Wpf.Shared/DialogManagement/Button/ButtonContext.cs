namespace Inferno
{
    public class ButtonContext<TChoice>
    {
        public ButtonContext(TChoice choice, bool isDefault, bool isCancel)
        {
            Choice = choice;
            IsDefault = isDefault;
            IsCancel = isCancel;
        }

        public TChoice Choice { get; }
        public bool IsDefault { get; }
        public bool IsCancel { get; }
    }
}
