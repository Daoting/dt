Param(
    [parameter(Mandatory = $false)][string]$svcName = "",
    [parameter(Mandatory = $false)][bool]$buildImage = $true,
    [parameter(Mandatory = $false)][bool]$clean = $true,
    [parameter(Mandatory = $false)][string]$tag = "dev"
)

$appName = "baisui"
$fullName = $appName + "-" + $svcName

# 生成镜像
if ($buildImage) {
    $imgName = $fullName + ":" + $tag
    Write-Host "正在生成" $imgName "镜像..." -ForegroundColor Green
    docker build -f ./src/dt.$svcName/Dockerfile -t $imgName ./src
    Write-Host "生成镜像结束 " $imgName -ForegroundColor Green
    Write-Host
}

# 删除上一release
if ($clean) {
    Write-Host "删除旧版releases..." -ForegroundColor Green
    # 手动立即删除旧pod，确保部署时pod已被删除
    kubectl delete pod $fullName --grace-period=0 --force
    helm delete --purge $fullName
    # helm delete --purge $(helm ls -q -a)
    Write-Host
}

# 复制服务的不同配置，保证完整chart包
Write-Host "复制" $svcName "配置..." -ForegroundColor Green
Copy-Item ./k8s/$svcName/* ./k8s/helm -recurse
Copy-Item ./k8s/global.json ./k8s/helm/config -recurse
Write-Host

Write-Host "使用Helm部署" $fullName "服务" -ForegroundColor Green
$ingressPath = "/" + $appName + "/" + $svcName
helm install --set image.repository=$fullName,image.tag=$tag,ingress.path=$ingressPath --name="$fullName" ./k8s/helm

# 删除服务的不同配置
del ./k8s/helm/config -recurse
del ./k8s/helm/Chart.yaml -recurse

# 延时等待pod启动，不然调试无法附加
#Start-Sleep 2