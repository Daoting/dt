using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dt.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Dt.Sample
{
    public sealed partial class FilePickerDemo : Win
    {
        public FilePickerDemo()
        {
            InitializeComponent();
        }

        void OnPickPhoto(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickImage());
        }

        void OnPickPhotos(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickImages());
        }

        void OnPickVideo(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickVideo());
        }

        void OnPickVideos(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickVideos());
        }

        void OnPickMedia(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickMedia());
        }

        void OnPickMedias(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickMedias());
        }

        void OnPickFile(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickFile(null));
        }

        void OnPickFiles(object sender, RoutedEventArgs e)
        {
            Write(FileKit.PickFiles(null));
        }

        async void Write(Task<FileData> p_file)
        {
            var file = await p_file;
            if (file == null)
                _tbInfo.Text = "未选择";
            else
                _tbInfo.Text = $"{file.FileName}\r\n{file.Size}\r\n{file.FilePath}";
        }

        async void Write(Task<List<FileData>> p_files)
        {
            var files = await p_files;
            if (files == null)
            {
                _tbInfo.Text = "未选择";
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var file in files)
            {
                sb.AppendLine(file.FileName);
                sb.AppendLine(file.Size.ToString());
                sb.AppendLine(file.FilePath);
                sb.AppendLine();
            }
            _tbInfo.Text = sb.ToString();
        }
    }
}
