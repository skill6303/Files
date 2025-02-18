using Files.Shared;
using Files.App.Filesystem;
using Files.App.Helpers;
using Files.Shared.Enums;
using Files.App.ViewModels.Properties;
using CommunityToolkit.WinUI;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Files.App.Shell;

namespace Files.App.Views
{
    public sealed partial class PropertiesGeneral : PropertiesTab
    {
        public PropertiesGeneral()
        {
            this.InitializeComponent();
        }

        public override async Task<bool> SaveChangesAsync(ListedItem item)
        {
            if (BaseProperties is DriveProperties driveProps)
            {
                var drive = driveProps.Drive;
                ViewModel.ItemName = ItemFileName.Text; // Make sure Name is updated
                if (!string.IsNullOrWhiteSpace(ViewModel.ItemName) && ViewModel.OriginalItemName != ViewModel.ItemName)
                {
                    var remDrive = new System.Text.RegularExpressions.Regex(@"\s*\(\w:\)$");
                    ViewModel.ItemName = remDrive.Replace(ViewModel.ItemName, ""); // Remove "(C:)" from the new label
                    if (AppInstance.FilesystemViewModel != null)
                    {
                        Win32API.SetVolumeLabel(drive.Path, ViewModel.ItemName);
                        _ = App.Window.DispatcherQueue.EnqueueAsync(async () =>
                        {
                            await drive.UpdateLabelAsync();
                            await AppInstance.FilesystemViewModel?.SetWorkingDirectoryAsync(drive.Path);
                        });
                        return true;
                    }
                }
            }
            else if (BaseProperties is LibraryProperties libProps)
            {
                var library = libProps.Library;
                ViewModel.ItemName = ItemFileName.Text; // Make sure Name is updated
                var newName = ViewModel.ItemName;
                if (!string.IsNullOrWhiteSpace(newName) && ViewModel.OriginalItemName != newName)
                {
                    if (AppInstance.FilesystemViewModel != null && App.LibraryManager.CanCreateLibrary(newName).result)
                    {
                        var libraryPath = library.ItemPath;
                        var renamed = await AppInstance.FilesystemHelpers.RenameAsync(new StorageFileWithPath(null, libraryPath), $"{newName}{ShellLibraryItem.EXTENSION}", Windows.Storage.NameCollisionOption.FailIfExists, false);
                        if (renamed == ReturnResult.Success)
                        {
                            var newPath = Path.Combine(Path.GetDirectoryName(libraryPath), $"{newName}{ShellLibraryItem.EXTENSION}");
                            _ = App.Window.DispatcherQueue.EnqueueAsync(async () =>
                            {
                                await AppInstance.FilesystemViewModel?.SetWorkingDirectoryAsync(newPath);
                            });
                            return true;
                        }
                    }
                }
            }
            else if (BaseProperties is CombinedProperties combinedProps)
            {
                // Handle the visibility attribute for multiple files
                if (AppInstance?.SlimContentPage?.ItemManipulationModel != null) // null on homepage
                {
                    foreach (var fileOrFolder in combinedProps.List)
                    {
                        await App.Window.DispatcherQueue.EnqueueAsync(() => UIFilesystemHelpers.SetHiddenAttributeItem(fileOrFolder, ViewModel.IsHidden, AppInstance.SlimContentPage.ItemManipulationModel));
                    }
                }
                return true;
            }
            else
            {
                // Handle the visibility attribute for a single file
                if (AppInstance?.SlimContentPage?.ItemManipulationModel != null) // null on homepage
                {
                    await App.Window.DispatcherQueue.EnqueueAsync(() => UIFilesystemHelpers.SetHiddenAttributeItem(item, ViewModel.IsHidden, AppInstance.SlimContentPage.ItemManipulationModel));
                }

                ViewModel.ItemName = ItemFileName.Text; // Make sure Name is updated
                if (!string.IsNullOrWhiteSpace(ViewModel.ItemName) && ViewModel.OriginalItemName != ViewModel.ItemName)
                {
                    return await App.Window.DispatcherQueue.EnqueueAsync(() => UIFilesystemHelpers.RenameFileItemAsync(item,
                          ViewModel.ItemName,
                          AppInstance));
                }
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
        }

        private void DiskCleanupButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (BaseProperties is DriveProperties driveProps)
            {
                var drive = driveProps.Drive;

                StorageSenseHelper.OpenStorageSense(drive.Path);
            }
        }
    }
}