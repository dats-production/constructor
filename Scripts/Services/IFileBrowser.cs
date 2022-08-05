using System;
using System.Collections.Generic;
using TriLibCore.SFB;

namespace Services
{
    public interface IFileBrowser
    {
        IList<ItemWithStream> OpenFilePanel(string title, string directory, ExtensionFilter[] extensions,
            bool multiselect);

        IList<ItemWithStream> OpenFolderPanel(string title, string directory, bool multiselect);

        ItemWithStream SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions);

        void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect,
            Action<IList<ItemWithStream>> cb);

        void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<IList<ItemWithStream>> cb);

        void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions,
            Action<ItemWithStream> cb);
    }

    public class FileBrowser : IFileBrowser
    {
        public IList<ItemWithStream> OpenFilePanel(string title, string directory, ExtensionFilter[] extensions,
            bool multiselect)
        {
            return StandaloneFileBrowser.OpenFilePanel(title, directory, extensions, multiselect);
        }

        public IList<ItemWithStream> OpenFolderPanel(string title, string directory, bool multiselect)
        {
            return StandaloneFileBrowser.OpenFolderPanel(title, directory, multiselect);
        }

        public ItemWithStream SaveFilePanel(string title, string directory, string defaultName,
            ExtensionFilter[] extensions)
        {
            return StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extensions);
        }

        public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect,
            Action<IList<ItemWithStream>> cb)
        {
            StandaloneFileBrowser.OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
        }

        public void OpenFolderPanelAsync(string title, string directory, bool multiselect,
            Action<IList<ItemWithStream>> cb)
        {
            StandaloneFileBrowser.OpenFolderPanelAsync(title, directory, multiselect, cb);
        }

        public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions,
            Action<ItemWithStream> cb)
        {
            StandaloneFileBrowser.SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
        }
    }
}