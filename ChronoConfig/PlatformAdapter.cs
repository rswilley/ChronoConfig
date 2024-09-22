using CommunityToolkit.Maui.Storage;

namespace ChronoConfig
{
    public interface IPlatformAdapter
    {
        Task<string> PickFolder();
        string GetPreference(string key);
        void SetPreference(string key, string value);
    }

    public class PlatformAdapter : IPlatformAdapter
    {
        private readonly IFolderPicker _folderPicker;

        public PlatformAdapter(IFolderPicker folderPicker)
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

        public string GetPreference(string key)
        {
            return Preferences.Default.Get(key, "");
        }

        public void SetPreference(string key, string value)
        {
            Preferences.Default.Set(key, value);
        }
    }
}
