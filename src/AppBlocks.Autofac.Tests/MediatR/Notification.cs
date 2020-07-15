using MediatR;

namespace AppBlocks.Autofac.Tests.MediatR
{
    public class Notification : INotification
    {
        public string Message { get; set; }

        public override string ToString() => $"Message: {Message}";
    }
}