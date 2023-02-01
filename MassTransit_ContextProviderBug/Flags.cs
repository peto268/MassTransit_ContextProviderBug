namespace MassTransit_ContextProviderBug;

public static class Flags
{
    public static bool UseOutbox = true;

    // This actually doesn't change the result
    public static bool UseSend = true;
}