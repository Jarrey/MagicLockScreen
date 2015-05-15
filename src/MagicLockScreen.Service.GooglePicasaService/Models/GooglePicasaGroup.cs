using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NoteOne_Core;
using NoteOne_Core.Common;

namespace MagicLockScreen_Service_GooglePicasaService.Models
{
    [DataContract]
    public class GooglePicasaGroup : ModelBase
    {
        public GooglePicasaGroup(IService[] services,
                                 uint maxPreviewCount,
                                 string title,
                                 string keyword,
                                 uint maxCount)
            : base(services)
        {
            PreviewCount = maxPreviewCount;
            Keyword = keyword;
            Title = title;

            if (PreviewGooglePicasas == null)
                PreviewGooglePicasas = new ObservableCollection<GooglePicasa>();

            if (GooglePicasas == null)
                GooglePicasas = new GooglePicasaCollection(maxCount, keyword, services);

            LoadPreviewGooglePicasas();
        }

        [DataMember]
        public string Title { get; private set; }

        [DataMember]
        public string Keyword { get; private set; }

        [DataMember]
        public uint PreviewCount { get; private set; }

        public GooglePicasaCollection GooglePicasas { get; private set; }

        public ObservableCollection<GooglePicasa> PreviewGooglePicasas { get; private set; }

        private async void LoadPreviewGooglePicasas()
        {
            var googlePicasaQueryService = this["GPQS"] as GooglePicasaQueryService;
            IList<GooglePicasa> googlePicasas = await googlePicasaQueryService.QueryDataAsync(Keyword, PreviewCount);
            if (googlePicasas != null)
                foreach (GooglePicasa googlePicasa in googlePicasas)
                    PreviewGooglePicasas.Add(googlePicasa);
        }
    }
}