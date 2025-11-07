using GestionBoutiqueElevate.Services;

namespace GestionBoutiqueElevate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- SERVICES ---
            builder.Services.AddControllersWithViews();

            // Repositories/services en mémoire (pas de doublons)
            builder.Services.AddSingleton<IClientRepository, InMemoryClientRepository>();
            builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            builder.Services.AddSingleton<ICouponService, InMemoryCouponService>();
            builder.Services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();
            builder.Services.AddSingleton<IAnnouncementRepository, InMemoryAnnouncementRepository>();

            // Choisis UNE implémentation d'InvoiceService
            builder.Services.AddSingleton<IInvoiceService, InvoiceService>();
            // Si tu veux désactiver la génération PDF, commente la ligne du dessus
            // et décommente la suivante :
            // builder.Services.AddSingleton<IInvoiceService, NullInvoiceService>();

            // Option : appsettings.json => "App": { "ShowSplashAtStartup": true }
            var showSplash = builder.Configuration.GetValue<bool>("App:ShowSplashAtStartup", true);

            var app = builder.Build();

            // --- MIDDLEWARES ---
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            // --- ROUTES ---
            // Si ShowSplashAtStartup = true -> /Home/Splash (qui redirige vers Dashboard)
            // Sinon -> /Home/Dashboard directement
            var defaultAction = showSplash ? "Splash" : "Dashboard";

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=" + defaultAction + "}/{id?}");

            app.Run();
        }
    }
}
