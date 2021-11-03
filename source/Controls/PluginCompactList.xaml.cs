﻿using CommonPluginsShared.Collections;
using CommonPluginsShared.Controls;
using CommonPluginsShared.Interfaces;
using Playnite.SDK.Models;
using SuccessStory.Models;
using SuccessStory.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuccessStory.Controls
{
    /// <summary>
    /// Logique d'interaction pour PluginCompactList.xaml
    /// </summary>
    public partial class PluginCompactList : PluginUserControlExtend
    {
        private SuccessStoryDatabase PluginDatabase = SuccessStory.PluginDatabase;
        internal override IPluginDatabase _PluginDatabase
        {
            get
            {
                return PluginDatabase;
            }
            set
            {
                PluginDatabase = (SuccessStoryDatabase)_PluginDatabase;
            }
        }

        private PluginCompactListDataContext ControlDataContext = new PluginCompactListDataContext();
        internal override IDataContext _ControlDataContext
        {
            get
            {
                return ControlDataContext;
            }
            set
            {
                ControlDataContext = (PluginCompactListDataContext)_ControlDataContext;
            }
        }


        public PluginCompactList()
        {
            InitializeComponent();
            this.DataContext = ControlDataContext;

            Task.Run(() =>
            {
                // Wait extension database are loaded
                System.Threading.SpinWait.SpinUntil(() => PluginDatabase.IsLoaded, -1);

                this.Dispatcher?.BeginInvoke((Action)delegate
                {
                    PluginDatabase.PluginSettings.PropertyChanged += PluginSettings_PropertyChanged;
                    PluginDatabase.Database.ItemUpdated += Database_ItemUpdated;
                    PluginDatabase.Database.ItemCollectionChanged += Database_ItemCollectionChanged;
                    PluginDatabase.PlayniteApi.Database.Games.ItemUpdated += Games_ItemUpdated;

                    // Apply settings
                    PluginSettings_PropertyChanged(null, null);
                });
            });
        }


        public override void SetDefaultDataContext()
        {
            ControlDataContext.IsActivated = PluginDatabase.PluginSettings.Settings.EnableIntegrationCompact;
            ControlDataContext.Height = PluginDatabase.PluginSettings.Settings.IntegrationCompactHeight + 28;

            ControlDataContext.PictureHeight = PluginDatabase.PluginSettings.Settings.IntegrationCompactHeight;
            ControlDataContext.ItemsSource = new ObservableCollection<Achievements>();
        }


        public override void SetData(Game newContext, PluginDataBaseGameBase PluginGameData)
        {
            GameAchievements gameAchievements = (GameAchievements)PluginGameData;
            ControlDataContext.ItemsSource = gameAchievements.Items.OrderByDescending(x => x.DateUnlocked)
                                                    .ThenBy(x => x.IsUnlock)
                                                    .ThenBy(x => x.Name)
                                                    .ToObservable();
        }
    }


    public class PluginCompactListDataContext : ObservableObject, IDataContext
    {
        private bool _IsActivated { get; set; }
        public bool IsActivated
        {
            get => _IsActivated;
            set
            {
                if (value.Equals(_IsActivated) == true)
                {
                    return;
                }

                _IsActivated = value;
                OnPropertyChanged();
            }
        }

        private double _Height { get; set; }
        public double Height
        {
            get => _Height;
            set
            {
                if (value.Equals(_Height) == true)
                {
                    return;
                }

                _Height = value;
                OnPropertyChanged();
            }
        }

        private double _PictureHeight { get; set; }
        public double PictureHeight
        {
            get => _PictureHeight;
            set
            {
                if (value.Equals(_PictureHeight) == true)
                {
                    return;
                }

                _PictureHeight = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Achievements> _ItemsSource { get; set; }
        public ObservableCollection<Achievements> ItemsSource
        {
            get => _ItemsSource;
            set
            {
                if (value?.Equals(_ItemsSource) == true)
                {
                    return;
                }

                _ItemsSource = value;
                OnPropertyChanged();
            }
        }
    }
}
