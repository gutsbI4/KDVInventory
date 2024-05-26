using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IDialogService
    {
        public enum DialogType
        {
            Standard,
            YesNoDialog
        }
        public enum DialogResult
        {
            Ok,
            Yes,
            No
        }
        public Task<DialogResult> ShowDialog(string title, string description, DialogType dialogType);
    }
}
