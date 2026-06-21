using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Services;
using MES.Shared.Services;
using MES.Shell.Services;
using MES.Shell.ViewModels;
using MES.Shell.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DryIoc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

using ISessionService = MES.Shell.Services.ISessionService;

namespace MES.Shell;

public partial class App : PrismApplication
{
    private Window? _mainShell;

    public App() => DispatcherUnhandledException += App_DispatcherUnhandledException;

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        var log = $"=== Exception ===\nTime: {DateTime.Now}\n";
        var depth = 0;
        while (ex != null)
        {
            log += $"\n[Level {depth}] Type: {ex.GetType().FullName}\n";
            log += $"Message: {ex.Message}\n";
            log += $"StackTrace:\n{ex.StackTrace}\n";
            if (ex is System.Windows.Markup.XamlParseException xamlEx && xamlEx.InnerException != null)
            {
                ex = xamlEx.InnerException;
                depth++;
            }
            else
            {
                break;
            }
        }
        Directory.CreateDirectory(@"C:\temp");
        File.WriteAllText(@"C:\temp\mes_crash.txt", log);
        MessageBox.Show($"错误: {e.Exception.Message}\n\n详细日志: C:\\temp\\mes_crash.txt", "MES Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    // ====================================================================
    // 阻止 Prism 自动显示 Shell，登录成功后手动显示
    // ====================================================================
    protected override void InitializeShell(Window shell)
    {
        // Don't call shell.Show() here — wait for login
        _mainShell = shell;
    }

    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override async void OnInitialized()
    {
        // ====================================================================
        // Step 1: 初始化认证种子数据
        // ====================================================================
        var seeder = Container.Resolve<AuthDataSeeder>();
        await seeder.EnsureSeededAsync();

        // ====================================================================
        // Step 2: 显示登录窗口（模态阻塞，后台无 MainWindow）
        // ====================================================================
        var loginWindow = Container.Resolve<LoginWindow>();
        loginWindow.Owner = null; // 无所有者窗口
        loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        loginWindow.ShowDialog();

        if (!loginWindow.IsLoginSuccessful)
        {
            Current.Shutdown();
            return;
        }

        // ====================================================================
        // Step 3: 正常 Shell 初始化（使用 CreateShell 创建的同一个实例）
        // ====================================================================
        base.OnInitialized();

        if (_mainShell == null)
        {
            Current.Shutdown();
            return;
        }

        Application.Current.MainWindow = _mainShell;

        var rm = Container.Resolve<IRegionManager>();
        rm.RegisterViewWithRegion("MenuRegion", typeof(MenuView));
        rm.RegisterViewWithRegion("NavigationRegion", typeof(NavigationView));
        rm.RegisterViewWithRegion("StatusRegion", typeof(StatusBarView));

        // V2.0 视图注册
        rm.RegisterViewWithRegion("NavigationRegionV2", typeof(NavigationViewV2));
        rm.RegisterViewWithRegion("StatusBarRegionV2", typeof(StatusBarViewV2));
        rm.RegisterViewWithRegion("MainContentRegionV2", typeof(ComponentDemoView));

        // 登录后重建菜单（MenuViewModel 是单例，登录前已创建但菜单为空）
        var menuVm = Container.Resolve<MenuViewModel>();
        menuVm.BuildMenu();

        // 根据用户权限导航到默认页面
        var session = Container.Resolve<ISessionService>();
        var defaultView = GetDefaultViewForRole(session);
        rm.RequestNavigate("NavigationRegion", "NavigationView",new NavigationParameters { { "ModuleKey",defaultView.Item1} });
        rm.RequestNavigate("MainContentRegion", defaultView.Item2);

        // 显示主窗口
        _mainShell.Show();
    }

    /// <summary>
    /// 根据用户权限选择默认导航页
    /// </summary>
    private static (string,string) GetDefaultViewForRole(ISessionService session)
    {
        if (session.HasPermission("Production", "WorkOrderListView"))
            return ("Production", "WorkOrderListView");
        if (session.HasPermission("Production", "TrackInView"))
            return ("Production", "TrackInView");
        if (session.HasPermission("Production", "LotListView"))
            return ("Production", "LotListView");
        if (session.HasPermission("Quality", "SpcChartView"))
            return ("Quality", "SpcChartView");
        if (session.HasPermission("Schedule", "DispatchBoardView"))
            return ("Schedule", "DispatchBoardView");
        if (session.HasPermission("Equipment", "EquipmentOverviewView"))
            return ("Equipment", "EquipmentOverviewView");

        return ("Production", "WorkOrderListView");
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register HttpClient as a singleton - all modules share this instance
        containerRegistry.RegisterInstance(new HttpClient { BaseAddress = new Uri("http://localhost:8940/api/") });

        // Get DryIoc container for advanced registration (Reuse, Made.Of, etc.)
        var container = containerRegistry.GetContainer();

        // 基础设施 - EF Core + Repository 模式
        var mysqlConnStr = "Server=Localhost;Database=mes_prod;Uid=root;Pwd=MyNewPass123!;Max Pool Size=100;";

        // 注册 DbContextOptions（EF Core 需要这个）
        var dbOptions = new DbContextOptionsBuilder<MesDbContext>()
            .UseMySql(mysqlConnStr, ServerVersion.AutoDetect(mysqlConnStr),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null))
            .LogTo(sql => Debug.WriteLine(sql), LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;
        
        container.RegisterInstance(dbOptions);
        
        // 注册 MesDbContext（Transient - WPF 无自动 Scope 管理）
        container.Register<MesDbContext>(made: Made.Of(() => new MesDbContext(Arg.Of<DbContextOptions<MesDbContext>>())),
            Reuse.Transient);

        // 注册泛型 Repository（Transient - 每次获取新实例，DbContext 也是 Transient）
        container.Register(typeof(IRepository<>), typeof(Repository<>), Reuse.Transient);

        // 注册专用 Repository（TrackInViewModel 依赖链需要）
        container.Register<IRouteRepository, RouteRepository>(Reuse.Transient);
        container.Register<ILotRepository, LotRepository>(Reuse.Transient);
        container.Register<IWorkOrderRepository, WorkOrderRepository>(Reuse.Transient);
        container.Register<IEquipmentRepository, EquipmentRepository>(Reuse.Transient);
        container.Register<ICarrierRepository, CarrierRepository>(Reuse.Transient);
        container.Register<IRecipeRepository, RecipeRepository>(Reuse.Transient);
        container.Register<IYieldRuleRepository, YieldRuleRepository>(Reuse.Transient);
        container.Register<IAlarmRuleRepository, AlarmRuleRepository>(Reuse.Transient);
        container.Register<IScrapRuleRepository, ScrapRuleRepository>(Reuse.Transient);
        container.Register<IOperationHistoryRepository, OperationHistoryRepository>(Reuse.Transient);
        container.Register<IScrapRecordRepository, ScrapRecordRepository>(Reuse.Transient);
        container.Register<IReworkRecordRepository, ReworkRecordRepository>(Reuse.Transient);
        container.Register<ILotSplitRepository, LotSplitRepository>(Reuse.Transient);
        container.Register<ILotMergeRepository, LotMergeRepository>(Reuse.Transient);
        container.Register<ICarrierBindingRepository, CarrierBindingRepository>(Reuse.Transient);
        container.Register<ISignatureRepository, SignatureRepository>(Reuse.Transient);
        container.Register<IAuditTrailRepository, AuditTrailRepository>(Reuse.Transient);
        container.Register<IAlarmRepository, AlarmRepository>(Reuse.Transient);
        container.Register<IDispatchTaskRepository, DispatchTaskRepository>(Reuse.Transient);
        container.Register<IQuantityTransactionRepository, QuantityTransactionRepository>(Reuse.Transient);
        container.Register<IProductRepository, ProductRepository>(Reuse.Transient);

        // 注册生产数据服务（LotListViewModel 等需要）
        container.Register<IProductionDataService, ProductionDataService>(Reuse.Transient);
        container.Register<IUserRepository, UserRepository>(Reuse.Transient);
        container.Register<IRoleRepository, RoleRepository>(Reuse.Transient);
        container.Register<IDepartmentRepository, DepartmentRepository>(Reuse.Transient);
        container.Register<ISignatureLevelRepository, SignatureLevelRepository>(Reuse.Transient);
        container.Register<IPermissionConfirmRepository, PermissionConfirmRepository>(Reuse.Transient);

        // 认证与会话服务
        // 先创建共享实例，再注册到两个接口，确保所有 ViewModel 拿到同一个 SessionService
        var sharedSession = new SessionService(
            container.Resolve<IRepository<MES.Infrastructure.Persistence.Entities.SysUserPermission>>(),
            container.Resolve<IRepository<MES.Infrastructure.Persistence.Entities.SysRole>>(),
            container.Resolve<IRepository<MES.Infrastructure.Persistence.Entities.SysDepartment>>());
        container.RegisterInstance<MES.Shared.Services.ISessionService>(sharedSession);
        container.RegisterInstance<ISessionService>(sharedSession);
        containerRegistry.Register<IUserAuthenticationService, UserAuthenticationService>();
        containerRegistry.Register<IPermissionConfirmService, PermissionConfirmService>();
        containerRegistry.Register<AuthDataSeeder>();

        // 视图与 ViewModel
        containerRegistry.Register<LoginWindow>();
        containerRegistry.Register<LoginViewModel>();

        // Shell ViewModel (Singleton - 与 View 生命周期一致)
        containerRegistry.RegisterSingleton<MenuViewModel>();
        containerRegistry.RegisterSingleton<NavigationViewModel>();
        containerRegistry.RegisterSingleton<StatusBarViewModel>();

        // 导航注册
        containerRegistry.RegisterForNavigation<MenuView>();
        containerRegistry.RegisterForNavigation<NavigationView>();
        containerRegistry.RegisterForNavigation<StatusBarView>();
        
        // V2.0 导航注册
        containerRegistry.RegisterForNavigation<NavigationViewV2>();
        containerRegistry.RegisterForNavigation<StatusBarViewV2>();
        containerRegistry.RegisterForNavigation<ComponentDemoView>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        foreach (var dll in Directory.GetFiles(dir, "MES.Modules.*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                foreach (var t in asm.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface))
                    moduleCatalog.AddModule(t);
            }
            catch { }
        }
    }
}