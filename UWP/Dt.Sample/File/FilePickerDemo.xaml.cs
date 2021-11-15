using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;


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
            Write(Kit.PickImage());
        }

        void OnPickPhotos(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickImages());
        }

        void OnPickVideo(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickVideo());
        }

        void OnPickVideos(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickVideos());
        }

        void OnPickMedia(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickMedia());
        }

        void OnPickMedias(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickMedias());
        }

        void OnPickFile(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickFile(null));
        }

        void OnPickFiles(object sender, RoutedEventArgs e)
        {
            Write(Kit.PickFiles(null));
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
