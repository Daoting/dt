Param(
    [parameter(Mandatory = $false)][string]$svcName = "",
    [parameter(Mandatory = $false)][bool]$buildImage = $true,
    [parameter(Mandatory = $false)][bool]$clean = $true,
    [parameter(Mandatory = $false)][string]$tag = "dev"
)

$appName = "baisui"
$fullName = $appName + "-" + $svcName

# ���ɾ���
if ($buildImage) {
    $imgName = $fullName + ":" + $tag
    Write-Host "��������" $imgName "����..." -ForegroundColor Green
    docker build -f ./src/dts.$svcName/Dockerfile -t $imgName ./src
    Write-Host "���ɾ������ " $imgName -ForegroundColor Green
    Write-Host
}

# ɾ����һrelease
if ($clean) {
    Write-Host "ɾ���ɰ�releases..." -ForegroundColor Green
    helm delete --purge $fullName
    # helm delete --purge $(helm ls -q) 
    Write-Host
}

Write-Host "ʹ��Helm����" $fullName "����" -ForegroundColor Green
$ingressPath = "/" + $appName + "/" + $svcName
helm install --set image.repository=$fullName --set image.tag=$tag --set ingress.path=$ingressPath --name="$fullName" ./k8s/helm