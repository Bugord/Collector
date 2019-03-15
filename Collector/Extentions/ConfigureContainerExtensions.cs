using Collector.BL.Services.AuthorizationService;
using Collector.BL.Services.ChatService;
using Collector.BL.Services.DebtsService;
using Collector.BL.Services.EmailService;
using Collector.BL.Services.Feedback;
using Collector.BL.Services.FriendListService;
using Collector.BL.Services.NotificationsService;
using Collector.BL.Services.UploadService;
using Collector.BL.Services.UserService;
using Collector.DAO.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Collector.Extentions
{
    public static class ConfigureContainerExtensions
    {
        public static void AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {

        }
        public static void AddScopedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITokenService, TokenService>();
            serviceCollection.AddScoped<IFriendListService, FriendListService>();
            serviceCollection.AddScoped<IDebtService, DebtService>();
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IFeedbackService, FeedbackService>();
            serviceCollection.AddScoped<IChatService, ChatService>();
            serviceCollection.AddScoped<IUploadService, UploadService>();
            serviceCollection.AddScoped<INotificationService, NotificationService>();
        }
    }
}
