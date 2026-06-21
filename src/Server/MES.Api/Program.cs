using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MES.Api.Configuration;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Repositories;
using MES.Services.Production;
using MES.Services.Quality;
using MES.Services.SystemMgmt;
using MES.Services.Equipment;
using MES.Services.Warehouse;
using MES.Services.Planning;
using MES.Services.ProcessControl;
using MES.Services.Analytics;
using MES.Services.Engineering;
using MES.Adapters.Mock;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=mes_prod;Uid=root;Pwd=MyNewPass123!;Max Pool Size=100;";

builder.Services.AddDbContext<MesDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<ICarrierRepository, CarrierRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IYieldRuleRepository, YieldRuleRepository>();
builder.Services.AddScoped<IAlarmRuleRepository, AlarmRuleRepository>();
builder.Services.AddScoped<IScrapRuleRepository, ScrapRuleRepository>();
builder.Services.AddScoped<IOperationHistoryRepository, OperationHistoryRepository>();
builder.Services.AddScoped<IScrapRecordRepository, ScrapRecordRepository>();
builder.Services.AddScoped<IReworkRecordRepository, ReworkRecordRepository>();
builder.Services.AddScoped<ILotSplitRepository, LotSplitRepository>();
builder.Services.AddScoped<ILotMergeRepository, LotMergeRepository>();
builder.Services.AddScoped<ICarrierBindingRepository, CarrierBindingRepository>();
builder.Services.AddScoped<ISignatureRepository, SignatureRepository>();
builder.Services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
builder.Services.AddScoped<IAlarmRepository, AlarmRepository>();
builder.Services.AddScoped<IDispatchTaskRepository, DispatchTaskRepository>();
builder.Services.AddScoped<IQuantityTransactionRepository, QuantityTransactionRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ISignatureLevelRepository, SignatureLevelRepository>();
builder.Services.AddScoped<IPermissionConfirmRepository, PermissionConfirmRepository>();

// Services
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<ILotService, LotService>();
builder.Services.AddScoped<IComplaint8DService, Complaint8DService>();
builder.Services.AddScoped<IEngineeringChangeService, EngineeringChangeService>();
builder.Services.AddScoped<ISpcMeasurementService, SpcMeasurementService>();
builder.Services.AddScoped<IQualityInspectionService, QualityInspectionService>();
builder.Services.AddScoped<IProcessExecutionService, ProcessExecutionService>();
builder.Services.AddScoped<ITestManagementService, TestManagementService>();

// System Management Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// Equipment Management Services
builder.Services.AddScoped<IEquipmentService, MES.Services.Equipment.EquipmentService>();

// Phase 1: Quality Services
builder.Services.AddScoped<IIqcService, IqcService>();
builder.Services.AddScoped<IFqcOqcService, FqcOqcService>();
builder.Services.AddScoped<INonconformingService, NonconformingService>();
builder.Services.AddScoped<IQualityAlertService, QualityAlertService>();
builder.Services.AddScoped<IFirstArticleService, FirstArticleService>();

// Phase 1: Warehouse Services
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IIssueReturnService, IssueReturnService>();
builder.Services.AddScoped<IFinishedGoodsService, FinishedGoodsService>();

// Phase 1: Production Services
builder.Services.AddScoped<IAbnormalService, AbnormalService>();
builder.Services.AddScoped<IEquipmentMaintenanceService, EquipmentMaintenanceService>();

// Phase 2: Planning Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPlanningService, PlanningService>();
builder.Services.AddScoped<IMrpService, MrpService>();
builder.Services.AddScoped<IRushOrderService, RushOrderService>();

// Phase 3: Process Control Services
builder.Services.AddScoped<IProcessParameterService, ProcessParameterService>();
builder.Services.AddScoped<IBinService, BinService>();
builder.Services.AddScoped<IToolingService, ToolingService>();
builder.Services.AddScoped<IWireService, WireService>();
builder.Services.AddScoped<IQualificationService, QualificationService>();
builder.Services.AddScoped<IBondPullTestService, BondPullTestService>();

// Phase 5: Analytics, NPI, and Audit Services
builder.Services.AddScoped<IKpiService, KpiService>();
builder.Services.AddScoped<ICostService, CostService>();
builder.Services.AddScoped<IYieldService, YieldService>();
builder.Services.AddScoped<INpiService, NpiService>();
builder.Services.AddScoped<IReliabilityTestService, ReliabilityTestService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISystemConfigService, SystemConfigService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Phase 4: Mock Adapters (External System Integration)
builder.Services.AddMockAdapters(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Authorization - require authentication for all endpoints by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
