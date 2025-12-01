using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.ApplicationModel.Events {
    public interface IEventPublicationService {
        void PublishEvent(object eventToPublish);
        Task PublishEventAsync(object eventToPublish);
    }
}
