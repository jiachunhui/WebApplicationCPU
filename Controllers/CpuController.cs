using Microsoft.AspNetCore.Mvc;
using Prometheus;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebApplicationCPU.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CpuController : Controller
	{
		private static bool _isRunning = false; // 开关状态
		private static int _cpuPercentage = 30; // CPU 使用率
		private static CancellationTokenSource _cancellationTokenSource; // 用于取消任务的 Token
		private readonly ILogger<CpuController> _logger; // 日志记录器

		

		public CpuController(ILogger<CpuController> logger)
		{
			_logger = logger;
		}

		// 开关控制接口
		[HttpPost("toggle")]
		public IActionResult Toggle([FromBody] bool isRunning)
		{
			_isRunning = isRunning;

			if (_isRunning)
			{
				// 如果开关打开，启动 CPU 负载逻辑
				_cancellationTokenSource = new CancellationTokenSource();
				Task.Run(() => RunCpuLoad(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
				_logger.LogInformation("CPU control started.");
				// 启动CPU指标收集
				CpuMetrics.StartCollecting();
			}
			else
			{
				// 如果开关关闭，停止 CPU 负载逻辑
				_cancellationTokenSource?.Cancel();
				_logger.LogInformation("CPU control stopped.");
			}

			return Ok(new { IsRunning = _isRunning });
		}

		// 设置 CPU 使用率接口
		[HttpPost("setPercentage")]
		public IActionResult SetPercentage([FromBody] int percentage)
		{
			if (percentage < 0 || percentage > 100)
			{
				return BadRequest("Percentage must be between 0 and 100.");
			}

			_cpuPercentage = percentage;
			_logger.LogInformation($"CPU percentage set to {_cpuPercentage}%.");
			return Ok(new { CpuPercentage = _cpuPercentage });
		}

		// 状态查询接口
		[HttpGet("status")]
		public IActionResult GetStatus()
		{
			return Ok(new { IsRunning = _isRunning, CpuPercentage = _cpuPercentage });
		}

		// CPU 负载逻辑（多核支持）
		private void RunCpuLoad(CancellationToken cancellationToken)
		{
			var stopwatch = new Stopwatch();
			var cpuUsageTimer = new Stopwatch();
			cpuUsageTimer.Start();

			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					stopwatch.Restart();

					// 模拟 CPU 工作
					while (stopwatch.ElapsedMilliseconds < (1000 * _cpuPercentage / 100))
					{
						if (cancellationToken.IsCancellationRequested)
							return;

						int loadIntensity = (int)(1000000 * (_cpuPercentage / 100.0));
						Parallel.For(0, Environment.ProcessorCount, i =>
						{
							for (int j = 0; j < loadIntensity; j++)
							{
								double result = Math.Sqrt(j) * Math.Tan(j);
							}
						});
					}

					// 模拟空闲时间
					int sleepTime = 1000 - (int)stopwatch.ElapsedMilliseconds;
					if (sleepTime > 0)
					{
						Thread.Sleep(sleepTime);
					}
					

				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred in RunCpuLoad.");
			}
		}

		

	


		



	

	
	}
}

public class CpuMetrics
{
	private static readonly Gauge CpuUsage = Metrics.CreateGauge("cpu_usage_percent", "Current CPU usage in percent.");

	public static void StartCollecting()
	{
		Task.Run(async () =>
		{
			var process = Process.GetCurrentProcess();
			var startTime = DateTime.UtcNow;
			var startCpuUsage = process.TotalProcessorTime;

			while (true)
			{
				await Task.Delay(1000); // 每秒更新一次

				var endTime = DateTime.UtcNow;
				var endCpuUsage = process.TotalProcessorTime;

				var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
				var totalMsPassed = (endTime - startTime).TotalMilliseconds;

				var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

				CpuUsage.Set(cpuUsageTotal * 100);

				startTime = endTime;
				startCpuUsage = endCpuUsage;

				Console.WriteLine($"CPU Usage: {Math.Round(cpuUsageTotal * 100)}%");
			}
		});
	}
}