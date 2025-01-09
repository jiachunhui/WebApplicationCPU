using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

// ���� Prometheus �м��
app.UseMetricServer(); // ��¶ָ��˵㣨Ĭ��·����/metrics��
app.UseHttpMetrics();  // ��� HTTP ����

app.MapControllers();

app.Run();
