using ContentProtector.App_Plugins.ContentProtector.Controllers;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace ContentProtector.App_Plugins.ContentProtector.Components {
    public class Composer:IUserComposer {
        public void Compose(Composition composition) {
            composition.Components().Append<InitializePlan>();
            composition.Components().Append<ActionBlocker>();
            composition.Register<ContentProtectorApiController>();
        }
    }
}