using CommunityToolkit.Maui.Storage;

namespace ChronoConfig
{
    public interface IDialogService
    {
        Task<string> PickFolder();
    }

    public class DialogService : IDialogService
    {
        private readonly IFolderPicker _folderPicker;

        public DialogService(IFolderPicker folderPicker)
        {
            _folderPicker = folderPicker;
        }

        public async Task<string> PickFolder()
        {
            var result = await _folderPicker.PickAsync();
            if (result.IsSuccessful)
            {
                return result.Folder.Path;
            }

            return string.Empty;
        }
    }
}
