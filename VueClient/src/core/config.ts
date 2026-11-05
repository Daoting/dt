import { Dict } from "./types";

export async function loadAppConfig() {
  // public目录的文件请求路径直接写 ./config.json
  const response = await fetch("./config.json");
  if (!response.ok) {
    throw new Error(`加载config.json失败：${response.status}`);
  }
  const json = await response.json();
  if (!json.server) {
    throw new Error("config.json中缺少server配置！");
  }

  if (json.server.startsWith("*"))
    json.server = json.server.replace("*", window.location.origin);
  kit.setServerUrl(json.server);
  const cfg = await kit.rpc<Dict>("cm", "SysKernel.GetConfig");
  if (!cfg.has("IsSingletonSvc")) {
    throw new Error("获取服务地址失败！");
  }
  const isSingletonSvc = cfg.get("IsSingletonSvc") as boolean;
  if (!isSingletonSvc && !cfg.has("SvcUrls"))
    throw new Error("未包含所有服务地址！服务端缺少url.json文件");
  kit.initSvcUrls(isSingletonSvc ? null : (cfg["SvcUrls"] as Dict));
}
