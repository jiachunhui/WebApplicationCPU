启动 CPU 控制：
curl -X POST -H "Content-Type: application/json" -d "true" http://localhost:5000/api/cpu/toggle

设置 CPU 使用率
curl -X POST -H "Content-Type: application/json" -d "70" http://localhost:5000/api/cpu/setPercentage



停止 CPU 控制：
curl -X POST -H "Content-Type: application/json" -d "false" http://localhost:5000/api/cpu/toggle

curl http://localhost:5000/api/cpu/status

curl http://localhost:5000/metrics


dotnet WebApplicationCPU.dll --urls http://*:5000



sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

sudo yum install dotnet-sdk-6.0

sudo yum install aspnetcore-runtime-6.0

dotnet --list-sdks
dotnet --list-runtimes

//创建开机服务
vi /etc/systemd/system/webcpu.service

[Unit]
Description=webcpu.service
 
[Service]
WorkingDirectory=/root/webcpu     
ExecStart=/usr/bin/dotnet  /root/webcpu/WebApplicationCPU.dll --urls http://*:5000 
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=AspnetCore
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
 
[Install]
WantedBy=multi-user.target

##开机启动 start
sudo systemctl enable webcpu.service
sudo systemctl reload webcpu.service
sudo systemctl start webcpu.service
sudo systemctl status webcpu.service
