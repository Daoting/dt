import * as Serializer from "./serializer";
import { Dict } from "./types";

/**
 * 调用服务API
 * @param serviceName 服务名称
 * @param methodName 方法名
 * @param params 参数列表
 * @returns 远程调用结果
 */
export function rpc<T>(
  serviceName: string,
  methodName: string,
  ...params: unknown[]
): Promise<T> {
  // 序列化 json RPC 调用请求
  const arr: any[] = [serviceName, methodName];
  if (params && params.length > 0) {
    for (let i = 0; i < params.length; i++) {
      arr.push(Serializer.serialize(params[i]));
    }
  }
  const str = JSON.stringify(arr);
  console.log("Rpc: " + str);

  const url = getSvcUrl(serviceName) + "/.c";
  return new Promise(function (resolve, reject) {
    let xhr: XMLHttpRequest | null = new XMLHttpRequest();
    xhr.responseType = "arraybuffer";
    xhr.open("post", url, true);
    // 识别web客户端，允许跨域请求
    xhr.setRequestHeader("dt.client", "web");
    // 内部用户标识
    xhr.setRequestHeader("uid", "110");

    // loadstart → progress（多次）→ load / error / abort / timeout → loadend
    // 1. 仅成功时触发：处理响应
    xhr.onload = async () => {
      if (xhr && xhr.status >= 200 && xhr.status < 300) {
        let result = await readResult(xhr.response);
        resolve(parseResult<T>(result));
      } else {
        reject(new Error("⚠️ HTTP 错误:" + (xhr?.status || 0)));
      }
    };

    // 2. 所有结束场景都触发：统一收尾
    xhr.onloadend = () => {
      if (xhr) xhr = null;
    };

    // 错误/中止/超时处理
    xhr.onerror = () => reject("❌ 网络错误");
    xhr.onabort = () => reject("⏹️ 请求被中止");
    xhr.ontimeout = () => reject("⌛ 请求超时");

    xhr.send(getRequestData(str) as XMLHttpRequestBodyInit);
  });
}

let isSingletonSvc: boolean = false;
let urlCm: string = "";
let urlDefSvc: string | null = null;
let urlDict = new Map<string, string>();

export function getSvcUrl(p_svcName: string): string {
  // 单体服务(所有微服务合并成一个服务)
  if (
    isSingletonSvc
    || "cm".localeCompare(p_svcName, undefined, { sensitivity: "base" }) === 0
  ) {
    return urlCm;
  }
  // da da+dbkey
  if (p_svcName.toLowerCase().startsWith("da")) {
    return urlDefSvc ?? urlCm;
  }

  if (urlDict.has(p_svcName)) {
    return urlDict.get(p_svcName)! as string;
  }
  throw new Error(`[${p_svcName}]服务地址不存在！`);
}

export function setServerUrl(p_server: string) {
  urlCm = p_server;
}

export function initSvcUrls(p_svcUrls: Dict | null) {
  if (p_svcUrls == null) {
    // 单体服务
    isSingletonSvc = true;
  } else if (urlCm != null && urlCm.trim() !== "") {
    const match = urlCm.match(/^http[s]?:\/\/[^\s:/]+/);
    let prefix = "";
    if (match != null) {
      prefix = match[0];
    }
    // 使用Map，key忽略大小写，对应C# Dictionary StringComparer.OrdinalIgnoreCase
    urlDict = new Map<string, string>();

    for (const [key, val] of Object.entries(p_svcUrls)) {
      let url = val as string;
      if (url.startsWith("*")) {
        url = prefix + url.substring(1).replace(/\\$/, "");
      }
      // key统一小写存入Map实现忽略大小写查询（配合GetSvcUrl）
      urlDict.set(key.toLowerCase(), url);
    }

    // 查找At.OriginSvc，忽略大小写
    const originKey = Array.from(urlDict.keys()).find((k) => k === "da");
    if (!originKey) {
      throw new Error(`da 服务地址不存在！`);
    }
    urlDefSvc = urlDict.get(originKey)!;
  }
}

function getRequestData(str: string): Uint8Array {
  // 分配最大可能长度：1字节压缩标志 + 4字节内容长度 + 内容
  let buf = new Uint8Array(str.length * 4 + 5);
  const encoder = new TextEncoder();
  const result = encoder.encodeInto(str, buf.subarray(5));

  // Frame头
  // 始终不压缩
  buf[0] = 0 & 0xff;
  // 内容长度
  const len = result.written;
  buf[1] = (len >> 24) & 0xff;
  buf[2] = (len >> 16) & 0xff;
  buf[3] = (len >> 8) & 0xff;
  buf[4] = len & 0xff;

  // 零拷贝，仅新建视图
  return buf.subarray(0, len + 5);
}

async function readResult(buf: ArrayBuffer): Promise<string> {
  // 1字节压缩标志 + 4字节内容长度
  const arr = new Uint8Array(buf);

  // 内容未压缩
  if (arr[0] == 0) return new TextDecoder("utf-8").decode(arr.subarray(5));

  // 内容压缩，解压
  // 1. 创建解压流
  const ds = new DecompressionStream("gzip");

  // 2. 把 Uint8Array 构造成可读流并 pipe 解压
  const stream = new ReadableStream({
    start(controller) {
      controller.enqueue(arr.subarray(5));
      controller.close();
    },
  });

  // 3. 管道解压
  const dec = stream.pipeThrough(ds);

  // 4. 读取为字符串
  const reader = dec.getReader();
  const decoder = new TextDecoder();
  let result = "";

  while (true) {
    const { done, value } = await reader.read();
    if (done) break;

    // 流式解码，告诉解码器：后面还有数据，先别急着收尾
    result += decoder.decode(value, { stream: true });
  }
  result += decoder.decode();
  return result;
}

function parseResult<T>(json: string): T {
  let result: any = { ResultType: 0, Elapsed: 0, Info: "", Value: null };
  try {
    const text = JSON.parse(json);
    // 0成功，1错误，2警告提示
    result.ResultType = text[0];
    // 耗时
    result.Elapsed = text[1];

    if (result.ResultType == 0) result.Value = Serializer.deserialize(text[2]);
    else if (result.ResultType == 1 || result.ResultType == 2)
      result.Info = text[2];
    else result.Info = "服务器返回异常：\r\n" + text[2];
  } catch (e) {
    result.ResultType = 1;
    result.Info = "返回Json内容结构不正确！";
  }

  if (result.ResultType == 0) return result.Value;

  if (result.ResultType == 1) alert(result.Info);
  throw new Error(result.Info);
}
