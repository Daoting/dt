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
    # �ֶ�����ɾ����pod��ȷ������ʱpod�ѱ�ɾ��
    kubectl delete pod $fullName --grace-period=0 --force
    helm delete --purge $fullName
    # helm delete --purge $(helm ls -q -a)
    Write-Host
}

Write-Host "ʹ��Helm����" $fullName "����" -ForegroundColor Green
$ingressPath = "/" + $appName + "/" + $svcName
helm install --values ./k8s/global.yaml --set image.repository=$fullName,image.tag=$tag,ingress.path=$ingressPath --name="$fullName" ./k8s/$svcName

# ��ʱ�ȴ�pod����
Start-Sleep 2