using MassTransit;
using MassTransit.DependencyInjection;

namespace MassTransit_ContextProviderBug;

public class PublishFilter<TMessage>
    : IFilter<PublishContext<TMessage>>
    where TMessage : class
{
    private readonly ScopedConsumeContextProvider _contextProvider;

    public PublishFilter(ScopedConsumeContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    public async Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
    {
        Console.WriteLine($"Publish has context: {_contextProvider.HasContext}");

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(GetType().ToString());
    }
}