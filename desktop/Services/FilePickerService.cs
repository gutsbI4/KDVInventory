﻿using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class FilePickerService : IFilePickerService
    {
        private TopLevel? _topLevel;
        public static FilePickerFileType ImageAll { get; } = new("All Images")
        {
            Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.webp" },
            AppleUniformTypeIdentifiers = new[] { "public.image" },
            MimeTypes = new[] { "image/*" }
        };
        public async Task<Stream> OpenFile(IFilePickerService.Filter filter)
        {
            IReadOnlyList<IStorageFile> files = null;
            if(filter == IFilePickerService.Filter.Image)
            {
                files = await _topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Выбор файла",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { ImageAll }
                }); ;
            }
            if(files != null && files.Count >= 1)
            {
                return await files[0].OpenReadAsync();
            }
            return null;
        }

        public void RegisterProvider(object provider)
        {
            if(provider is TopLevel)
                _topLevel = (TopLevel)provider;
            Debug.WriteLine("");
        }
        public static FilePickerFileType Doc { get; } = new("Docx")
        {
            Patterns = new[] {"*.docx"},
            AppleUniformTypeIdentifiers = new[] {"public.document"},
            MimeTypes = new[] {"document/*"}
        };
        public async Task<Uri> SaveFile(IFilePickerService.Filter filter)
        {
            IStorageFile? storageFile = null;
            if (filter == IFilePickerService.Filter.Docx)
                storageFile = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "Сохранить файл",
                    FileTypeChoices = new[] {Doc}
                });
            if (storageFile != null)
                return storageFile.Path;
            return null;
        }
    }
}
