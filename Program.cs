using GestionBoutiqueElevate.Services;


namespace GestionBoutiqueElevate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- CONFIGURATION DES SERVICES ---
            builder.Services.AddControllersWithViews();

            // Injection du repository clients (en mémoire pour l’instant)
            builder.Services.AddSingleton<IClientRepository, InMemoryClientRepository>();

            builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

			builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

			builder.Services.AddSingleton<ICouponService, InMemoryCouponService>();

            builder.Services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();


            builder.Services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();

            builder.Services.AddSingleton<IInvoiceService, InvoiceService>();
            builder.Services.AddSingleton<IInvoiceService, NullInvoiceService>();



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

            // --- ROUTAGE PAR DÉFAUT ---
            app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
