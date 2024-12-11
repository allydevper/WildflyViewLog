using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildflyViewLog.Services
{
    public class FilePickerService
    {
        public static async Task<IReadOnlyList<IStorageFile>?> OpenTxtFileAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } _storageProvider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Abre el JSON con el formato Wildfly",
                AllowMultiple = true,
                FileTypeFilter =
                [
                    new FilePickerFileType("Text Files")
                    {
                        Patterns = ["*.txt"],
                        AppleUniformTypeIdentifiers = ["public.plain-text"],
                        MimeTypes = ["text/plain"]
                    }
                ],
            });

            return files;
        }

        public static async Task<IStorageFile?> OpenJsonFileAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } _storageProvider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Abre el JSON con el formato Wildfly",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("JSON Files")
                    {
                        Patterns = ["*.json"],
                        AppleUniformTypeIdentifiers = ["public.json"],
                        MimeTypes = ["application/json"]
                    }
                ],
            });

            return files?.Count >= 1 ? files[0] : null;
        }

        public static async Task<IStorageFile?> SaveFileAsync(string nombreConExtension)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } _storageProvider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var nameFile = string.IsNullOrEmpty(nombreConExtension) ? "export" : nombreConExtension;

            return await _storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Guarda archivo log de texto",
                DefaultExtension = "txt",
                SuggestedFileName = nameFile
            });
        }

        public static async Task<IStorageFolder?> SaveFolderAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } _storageProvider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var folders = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Selecciona carpeta donde guardar",
                AllowMultiple = false
            });

            if (folders == null || folders.Count == 0)
                return null;

            return folders[0];
        }
    }
}