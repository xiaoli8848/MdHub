using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Markdig;
using Markdig.SyntaxHighlighting;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MdHub.ViewModels
{
    public partial class DocumentViewModel : ObservableRecipient
    {
        public DocumentViewModel()
        {
            OpenFileCommand = new AsyncRelayCommand(OpenFile);
            SaveTextCommand = new AsyncRelayCommand(SaveText);
            var temp = Path.GetTempFileName();
            File.Move(temp, temp + ".html");
            cachedDocumentViewFile = new FileInfo(temp + ".html");
        }

        private static readonly MarkdownPipeline pipeline = 
            new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .UseSyntaxHighlighting()
            .Build();

        [ObservableProperty]
        private FileInfo documentFile;

        [ObservableProperty]
        private string documentCachedText;

        private FileInfo cachedDocumentViewFile;

        public readonly WebView2 DocumentView = new();

        public IAsyncRelayCommand OpenFileCommand { get; }

        partial void OnDocumentCachedTextChanged(string value)
        {
            File.WriteAllText(cachedDocumentViewFile.FullName, new StringBuilder("<html><body>").Append(Markdown.Parse(value, pipeline).ToHtml()).Append("</body></html>").ToString());
            DocumentView.Source = new Uri("file://" + cachedDocumentViewFile.FullName);
        }

        private async Task OpenFile()
        {
            var picker = new FileOpenPicker();

            // 将句柄用于初始化Picker。
            WinRT.Interop.InitializeWithWindow.Initialize(picker, (App.Current as App).MainWindowHandle);
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            DocumentFile = new FileInfo(file.Path);
            DocumentCachedText = File.ReadAllText(DocumentFile.FullName);
        }

        public IAsyncRelayCommand SaveTextCommand { get; }

        private async Task SaveText()
        {
            if (DocumentFile is not null)
            {
                DocumentFile.Delete();
                DocumentFile.Create();
                File.WriteAllText(DocumentFile.FullName, DocumentCachedText);
            }
            else
            {
                var picker = new FileSavePicker();

                // 将句柄用于初始化Picker。
                WinRT.Interop.InitializeWithWindow.Initialize(picker, (App.Current as App).MainWindowHandle);

                var file = await picker.PickSaveFileAsync();
                DocumentFile = new FileInfo(file.Path);
                if (DocumentFile.Exists) DocumentFile.Delete();
                DocumentFile.Create();
                DocumentCachedText = File.ReadAllText(DocumentFile.FullName);
            }
        }
    }
}
