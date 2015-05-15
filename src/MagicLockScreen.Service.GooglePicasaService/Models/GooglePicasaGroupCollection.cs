using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_GooglePicasaService.Models
{
    public class GooglePicasaGroupCollection : ObservableCollection<GooglePicasaGroup>, INotifyPropertyChanged
    {
        private readonly XmlElement _configXml;
        private readonly uint _maxCount;
        private readonly IService[] _services;
        private bool _isLoading;

        public GooglePicasaGroupCollection(IService[] services, XmlElement configXml, uint maxCount)
        {
            _services = services;
            _configXml = configXml;
            _maxCount = maxCount;
            InitializeData(services, configXml, maxCount);
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsLoading"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeData(IService[] services, XmlElement configXml, uint maxCount)
        {
            try
            {
                IsLoading = true;
                Clear();
                XmlNodeList itemGroups = configXml.SelectNodes("ItemGroups/Item");
                foreach (IXmlNode itemGroup in itemGroups)
                {
                    uint previewCount = itemGroup.SelectSingleNode("@PreviewCount").InnerText.Check().StringToUInt();
                    string keyword = itemGroup.SelectSingleNode("@Keyword").InnerText.Check();
                    string title = itemGroup.SelectSingleNode("@Title").InnerText.Check();
                    var googlePicasaGroup = new GooglePicasaGroup(services, previewCount, title, keyword, maxCount);
                    Add(googlePicasaGroup);
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        public void RefreshCollection()
        {
            InitializeData(_services, _configXml, _maxCount);
        }
    }
}