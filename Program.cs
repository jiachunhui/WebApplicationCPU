using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

// 启用 Prometheus 中间件
app.UseMetricServer(); // 暴露指标端点（默认路径：/metrics）
app.UseHttpMetrics();  // 监控 HTTP 请求

app.MapControllers();

app.Run();
