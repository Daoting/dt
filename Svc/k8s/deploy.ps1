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
    docker build -f ./src/dts.$svcName/Dockerfile -t $imgName ./src
    Write-Host "生成镜像结束 " $imgName -ForegroundColor Green
    Write-Host
}

# 删除上一release
if ($clean) {
    Write-Host "删除旧版releases..." -ForegroundColor Green
    helm delete --purge $fullName
    # helm delete --purge $(helm ls -q) 
    Write-Host
}

Write-Host "使用Helm部署" $fullName "服务" -ForegroundColor Green
$ingressPath = "/" + $appName + "/" + $svcName
helm install --set image.repository=$fullName --set image.tag=$tag --set ingress.path=$ingressPath --name="$fullName" ./k8s/helm