namespace AppBlocks.Autofac.Tests.MediatR
{
    public interface IMediatRReceiverService
    {
        void RunRequest();

        void RunNotification();
    }
}