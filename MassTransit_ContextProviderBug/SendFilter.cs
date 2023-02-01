using MassTransit;
using MassTransit.DependencyInjection;

namespace MassTransit_ContextProviderBug;

public class SendFilter<TMessage>
    : IFilter<SendContext<TMessage>>
    where TMessage : class
{
    private readonly ScopedConsumeContextProvider _contextProvider;

    public SendFilter(ScopedConsumeContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    public async Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
    {
        Console.WriteLine($"Send has context: {_contextProvider.HasContext}");

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(GetType().ToString());
    }
}