﻿using Microsoft.OneDrive.Sdk;
using MoneyFox.Shared.Constants;
using MoneyFox.Shared.Exceptions;
using MoneyFox.Shared.Interfaces;
using MoneyFox.Shared.Manager;
using MoneyFox.Shared.Resources;
using MvvmCross.Core.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MoneyFox.Shared.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        private readonly IBackupManager backupManager;
        private readonly IDialogService dialogService;

        public BackupViewModel(IBackupManager backupManager,
            IDialogService dialogService)
        {
            this.backupManager = backupManager;
            this.dialogService = dialogService;
        }

        /// <summary>
        ///     The Date when the backup was modified the last time.
        /// </summary>
        public DateTime BackupLastModified { get; private set; }

        /// <summary>
        ///     Prepares the View when loaded.
        /// </summary>
        public MvxCommand LoadedCommand => new MvxCommand(Loaded);

        /// <summary>
        ///     Will create a backup of the database and upload it to onedrive
        /// </summary>
        public MvxCommand BackupCommand => new MvxCommand(CreateBackup);

        /// <summary>
        ///     Will download the database backup from onedrive and overwrite the
        ///     local database with the downloaded.
        ///     All datamodels are then reloaded.
        /// </summary>
        public MvxCommand RestoreCommand => new MvxCommand(RestoreBackup);

        /// <summary>
        ///     Indicator if something is in work.
        /// </summary>
        public bool IsLoading { get; private set; }

        public bool BackupAvailable { get; private set; }

        private async void Loaded()
        {
            BackupAvailable = await backupManager.IsBackupExisting();
            BackupLastModified = await backupManager.GetBackupDate();            
        }

        private async void CreateBackup()
        {
            if(!await ShowOverwriteBackupInfo())
            {
                return;
            }

            IsLoading = true;
            await backupManager.UploadNewBackup();
            BackupLastModified = DateTime.Now;
            await ShowCompletionNote();
            IsLoading = false;
        }

        private async void RestoreBackup()
        {
            if(!await ShowOverwriteDataInfo())
            {
                return;
            }

            IsLoading = true;
            await backupManager.RestoreBackup();
            await ShowCompletionNote();
            IsLoading = false;
        }        

        private async Task<bool> ShowOverwriteBackupInfo()
            => await dialogService.ShowConfirmMessage(Strings.OverwriteTitle, Strings.OverwriteBackupMessage);

        private async Task<bool> ShowOverwriteDataInfo()
            => await dialogService.ShowConfirmMessage(Strings.OverwriteTitle, Strings.OverwriteDataMessage);

        private async Task ShowCompletionNote()
        {
            await dialogService.ShowMessage(Strings.SuccessTitle, Strings.TaskSuccessfulMessage);
        }
    }
}