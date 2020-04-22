using System.Threading;
using System.Threading.Tasks;

namespace Inferno.LifeCycle.Tests
{
    public class StateScreen : Screen
    {
        public bool IsClosable { get; set; }

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(IsClosable);
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
