using AIProductify.API.Middleware;
using AIProductify.Application.Interfaces;
using AIProductify.Application.Services;
using AIProductify.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using AIProductify.Application.Mapping;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using AIProductify.Application.Helper;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("LogDatabase"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        },
        columnOptions: GetCustomSqlColumnOptions() // Özelleþtirilmiþ sütunlar
    )
    .CreateLogger();

builder.Host.UseSerilog();

// SQL Column Options
ColumnOptions GetCustomSqlColumnOptions()
{
    var columnOptions = new ColumnOptions();

    // Varsayýlan sütunlarý kaldýr
    columnOptions.Store.Remove(StandardColumn.Message);
    columnOptions.Store.Remove(StandardColumn.Properties);
    columnOptions.Store.Remove(StandardColumn.Level);
    columnOptions.Store.Remove(StandardColumn.TimeStamp);
    columnOptions.Store.Remove(StandardColumn.Exception);

    // Özel sütunlarý ekle
    columnOptions.AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn { ColumnName = "Path", DataType = System.Data.SqlDbType.NVarChar, DataLength = -1 },
        new SqlColumn { ColumnName = "Query", DataType = System.Data.SqlDbType.NVarChar, DataLength = -1 },
        new SqlColumn { ColumnName = "RequestBody", DataType = System.Data.SqlDbType.NVarChar, DataLength = -1 },
        new SqlColumn { ColumnName = "StatusCode", DataType = System.Data.SqlDbType.Int },
        new SqlColumn { ColumnName = "ResponseBody", DataType = System.Data.SqlDbType.NVarChar, DataLength = -1 }
    };

    return columnOptions;
}


// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<GetSettingsConfig>(builder.Configuration.GetSection("OpenAI"));


builder.Services.AddHttpClient<IHtmlCrawlService, HtmlCrawlService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOpenAiService, OpenAiService>();
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAutoMapper(typeof(MappingProfile));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("X-API-Key", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-API-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "OguzEvrensel"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "X-API-Key",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "X-API-Key"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
