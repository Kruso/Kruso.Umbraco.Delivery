using Kruso.Umbraco.Delivery.Models;
using Microsoft.Extensions.Logging;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliMedia : IDeliMedia
    {
        private readonly IMediaService _mediaService;
        private readonly ILogger<DeliMedia> _log;

        public DeliMedia(IMediaService mediaService, ILogger<DeliMedia> log)
        {
            _mediaService = mediaService;
            _log = log;
        }

        public IPublishedContent GetMedia(string udi)
        {
            try
            {
                var guid = new Guid(udi.Substring("umb://media/".Length));
                var media = _mediaService.GetById(guid);

                return new DeliPublishedMedia(media);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error fetching media item {udi}");
                return null;
            }
        }
    }
}
