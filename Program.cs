using log4net;
using log4net.Config;

var logRepository = LogManager.GetRepository(typeof(Program).Assembly);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new Appsettings());
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IKafkaConsumerService, KafkaConsumerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<LoggerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

