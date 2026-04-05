namespace Producion_Line_Manager.Helpers;

public static class ServiceHelper
{
    public static IServiceProvider Services { get; private set; }

    public static void Initialize(IServiceProvider serviceProvider) =>
        Services = serviceProvider;
    
    public static T GetService<T>()
    {
        if (Services == null)
        {
            throw new InvalidOperationException("Service provider not initialized. Call ServiceHelper.Initialize() with a valid service provider before using this method.");
        }
        return Services.GetService<T>();
    }
        
}
