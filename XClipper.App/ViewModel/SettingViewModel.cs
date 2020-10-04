﻿using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using static Components.MainHelper;
using static Components.DefaultSettings;
using System.Windows;
using static Components.App;
using static Components.LicenseHandler;
using System.IO;
using static Components.Constants;
using System;
using Components.viewModels;
using static Components.Core;
using System.Windows.Controls;
using static Components.TranslationHelper;
using System.Security.RightsManagement;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;

#nullable enable

namespace Components
{
    public class SettingViewModel : BaseViewModel
    {

        #region Constructor

        private bool previousSecureDBValue;
        private string previousPassword;

        public SettingViewModel()
        {
            KeyDownCommand = new RelayCommand<KeyEventArgs>(OnKeyDown, null);
            SaveCommand = new RelayCommand(SaveButtonClicked);
            ResetCommand = new RelayCommand(ResetButtonClicked);
            PurchaseCommand = new RelayCommand(PurchaseButtonClicked);
            ConnectedCommand = new RelayCommand(ConnectedButtonClicked);
            ResetDataCommand = new RelayCommand(ResetDataButtonClicked);
            QRImageCommand = new RelayCommand<ImageSource>(QRImageDoubleClicked);

            previousSecureDBValue = IsSecureDB;
            previousPassword = CustomPassword;
        }

        #endregion

        #region Actual Settings

        private bool is_secure_db { get; set; } = IsSecureDB;
        private ISettingEventBinder? _settingbinder { get; set; }
        public ICommand QRImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand PurchaseCommand { get; set; }
        public ICommand ConnectedCommand { get; set; }
        public ICommand ResetDataCommand { get; set; }
        public RelayCommand<KeyEventArgs> KeyDownCommand { get; set; }
        public bool SASS { get; set; } = StartOnSystemStartup;
        public bool CAU { get; set; } = CheckApplicationUpdates;
        public bool DSN { get; set; } = DisplayStartNotification;
        public bool SDCN { get; set; } = ShowDataChangeNotification;
        public XClipperStore WTS { get; set; } = WhatToStore;
        public XClipperLocation ADL { get; set; } = AppDisplayLocation;
        public bool KEY_IC { get; set; } = IsCtrl;
        public bool KEY_IS { get; set; } = IsShift;
        public bool KEY_IA { get; set; } = IsAlt;
        public bool UCP { get; set; } = UseCustomPassword;
        public string CP { get; set; } = CustomPassword;
        public int TCL { get; set; } = TotalClipLength;
        public string KEY_HK { get; set; } = HotKey;
        public string CAL { get; set; } = CurrentAppLanguage;
        public int FMI { get; } = DatabaseMaxItem;
        public int FMIL { get; } = DatabaseMaxItemLength;
        public int FMCD { get; } = DatabaseMaxConnection;
        public string FDP { get; set; } = DatabaseEncryptPassword;
        public string UID { get; private set; } = UniqueID;
        public bool BTD { get; set; } = BindDatabase;
        public bool BIU { get; set; } = BindImage;
        public bool BFD { get; set; } = BindDelete;
        public bool ISDB
        {
            get { return is_secure_db; }
            set
            {
                if (value == true != previousSecureDBValue)
                {
                    var result = MessageBox.Show(Translation.MSG_DELETE_DB, Translation.MSG_WARNING, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        is_secure_db = value;
                        return;
                    }
                }
                is_secure_db = value;
            }
        }
        public string QRTooltip { get; set; } = QRData == null ? Translation.SETTINGS_QR_TOOLTIP_NO_BIND : Translation.SETTINGS_QR_TOOLTIP;

        #endregion

        #region Method Events

        public void SetSettingBinder(ISettingEventBinder binder)
        {
            _settingbinder = binder;
        }

        /// <summary>
        /// This event will be raised when Connected device button is clicked.
        /// </summary>
        private void ConnectedButtonClicked()
        {
            _settingbinder?.OnConnectedDeviceClicked();
        }

        /// <summary>
        /// This event will be raised when learn-more link is clicked in Connect Tab.
        /// </summary>
        private void PurchaseButtonClicked()
        {
            _settingbinder?.OnBuyButtonClicked();
        }

        /// <summary>
        /// This method will save the Image Stream from QR Image Source
        /// and will show the image in default image viewer.
        /// </summary>
        /// <param name="obj"></param>
        private void QRImageDoubleClicked(ImageSource obj)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)obj));
            using (FileStream stream = new FileStream(QRImageFilePath, FileMode.Create))
            {
                encoder.Save(stream);
                Process.Start(QRImageFilePath);
            }
        }

        /// <summary>
        /// This event will be raised when Reset Data button is clicked.
        /// </summary>
        private void ResetDataButtonClicked()
        {
            var result = MessageBox.Show(Translation.MSG_RESET_DATA, Translation.MSG_INFO, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                _settingbinder?.OnDataResetButtonClicked();
            }
        }

        /// <summary>
        /// This event will be raised when Reset Button is Clicked.
        /// </summary>
        private void ResetButtonClicked()
        {
            SASS = StartOnSystemStartup = false;
            CAU = CheckApplicationUpdates = true;
            DSN = DisplayStartNotification = true;
            SDCN = ShowDataChangeNotification = true;
            WhatToStore = WTS = XClipperStore.All;
            AppDisplayLocation = ADL = XClipperLocation.BottomRight;
            IsCtrl = KEY_IC = true;
            IsAlt = KEY_IA = false;
            UCP = UseCustomPassword = false;
            CP = CustomPassword = CONNECTION_PASS.Decrypt();
            FDP = DatabaseEncryptPassword = FB_DEFAULT_PASS.Decrypt();
            IsShift = KEY_IS = false;
            HotKey = KEY_HK = "Oem3";
            CurrentAppLanguage = CAL = "locales\\en.xaml";
            TotalClipLength = TCL = 20;
            BindDatabase = BTD = false;
            BindDelete = BFD = false;
            BindImage = BIU = true;
            SetAppStartupEntry();
            WriteSettings();
            MsgBoxHelper.ShowInfo(Translation.SETTINGS_RESET);
        }

        /// <summary>
        /// This event will be raised when Save Button is Clicked.
        /// </summary>
        private void SaveButtonClicked()
        {
            StartOnSystemStartup = SASS;
            CheckApplicationUpdates = CAU;
            DisplayStartNotification = DSN;
            ShowDataChangeNotification = SDCN;
            IsSecureDB = ISDB;
            WhatToStore = WTS;
            AppDisplayLocation = ADL;
            IsCtrl = KEY_IC;
            UseCustomPassword = UCP;
            IsShift = KEY_IS;
            IsAlt = KEY_IA;
            TotalClipLength = TCL;
            HotKey = KEY_HK;
            CurrentAppLanguage = CAL;
            BindDelete = BFD;
            BindImage = BIU;
            SetAppStartupEntry();

            var isBindApplied = ToggleBindDatabase();

            ToggleCustomPassword();

            ToggleSecureSqlDatabase();

            ToggleFirebasePassword();

            WriteSettings();

            if (!isBindApplied)
                MsgBoxHelper.ShowWarning(Translation.SETTINGS_SAVE_WARNING);
            else
                MsgBoxHelper.ShowInfo(Translation.SETTINGS_SAVE);
        }

        private bool ToggleBindDatabase()
        {
            if (!FirebaseHelper.PerformSafetyChecks(doOnNoConfigurationFile: () =>
            {
                _settingbinder?.OnNoConfigurationFound();
            }))
            {
                BindDatabase = BTD = false;
                return false;
            }

            BindDatabase = BTD;
            if (BindDatabase == BTD == true)
                FirebaseHelper.InitializeService();
            return true;
        }

        /// <summary>
        /// This method will change the current firebase password to new one.
        /// </summary>
        private void ToggleFirebasePassword()
        {
            if (DatabaseEncryptPassword != FDP)
            {
                var result = MessageBox.Show(Translation.SETTINGS_FB_PASSWORD, Translation.MSG_INFO, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Remove the user from firebase
                    FirebaseSingleton.GetInstance.RemoveUser().RunAsync();

                    DatabaseEncryptPassword = FDP;
                }
            }
        }

        /// <summary>
        /// This method will set which password to use for database encryption.
        /// </summary>
        private void ToggleCustomPassword()
        {
            if (UseCustomPassword)
            {
                CustomPassword = CP;
            }
            else
            {
                CustomPassword = CONNECTION_PASS.Decrypt();
            }
        }

        /// <summary>
        /// This will delete and create new secure SQL database.
        /// </summary>
        private void ToggleSecureSqlDatabase()
        {
            if (previousSecureDBValue != is_secure_db)
            {
                MigrateDatabase();
            }
            else if (previousPassword != CP)
            {
                var result = MessageBox.Show(Translation.MSG_DELETE_DB, Translation.MSG_WARNING, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    MigrateDatabase();
                }
                else
                {
                    // Restore the value
                    CP = CustomPassword = CONNECTION_PASS.Decrypt();
                }
            }
        }

        /// <summary>
        /// Migrate/Recreate database with new applied settings.
        /// </summary>
        private void MigrateDatabase()
        {
            // Get list of all data...
            var previousData = AppSingleton.GetInstance.GetAllData();

            // Close connection to database...
            AppSingleton.GetInstance.Close();

            // Delete the existing database...
            File.Delete(DatabasePath);

            // Instantiate the database...
            AppSingleton.GetInstance.Init();

            // Restore the tables in the database...
            AppSingleton.GetInstance.InsertAll(previousData);
        }

        /// <summary>
        /// This event will observe Hot Key value.
        /// </summary>
        /// <param name="args"></param>
        private void OnKeyDown(KeyEventArgs args)
        {
            //if (args.IsRepeat)
            //    return;

            if (args.Key != Key.LeftCtrl && args.Key != Key.RightCtrl && args.Key != Key.LeftShift
                && args.Key != Key.RightShift && args.Key != Key.LeftAlt && args.Key != Key.RightAlt
                && args.Key != Key.System)
                KEY_HK = args.Key.ToString();
        }

        #endregion

    }
}
