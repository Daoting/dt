
class Rpc {
    private data: Uint8Array;

    constructor(serviceName: string, methodName: string, ...params: any[]) {
        // 序列化 json RPC 调用请求
        const array: any[] = [serviceName, methodName];
        if (params && params.length > 0) {
            for (let i = 0; i < params.length; i++) {
                array.push(params[i]);
            }
        }
        const str = JSON.stringify(array);
        console.log("Rpc: " + str);
        this.data = Rpc.getRequestData(str);
    }

    call(): Promise<any> {
        const self = this;
        return new Promise(function (resolve, reject) {
            var xhr = new XMLHttpRequest();
            xhr.responseType = "arraybuffer";
            xhr.open("post", "https://localhost:1234/.c", true);
            xhr.setRequestHeader("dt-wasm", "true");
            // 内部用户标识
            xhr.setRequestHeader("uid", "110");

            // loadstart → progress（多次）→ load / error / abort / timeout → loadend
            // 1. 仅成功时触发：处理响应
            xhr.onload = async () => {
                if (xhr.status >= 200 && xhr.status < 300) {
                    resolve(await Rpc.readResult(xhr.response));
                } else {
                    reject(new Error('⚠️ HTTP 错误:' + xhr.status));
                }
            };

            // 2. 所有结束场景都触发：统一收尾
            xhr.onloadend = () => { if (xhr) xhr = null; };

            // 错误/中止/超时处理
            xhr.onerror = () => reject('❌ 网络错误');
            xhr.onabort = () => reject('⏹️ 请求被中止');
            xhr.ontimeout = () => reject('⌛ 请求超时');

            xhr.send(self.data);
        });
    }

    private static getRequestData(str: string): Uint8Array {
        // 分配最大可能长度：1字节压缩标志 + 4字节内容长度 + 内容
        let buf = new Uint8Array(str.length * 4 + 5);
        const encoder = new TextEncoder();
        const result = encoder.encodeInto(str, buf.subarray(5));

        // Frame头
        // 始终不压缩
        buf[0] = 0 & 0xFF;
        // 内容长度
        const len = result.written;
        buf[1] = (len >> 24) & 0xFF;
        buf[2] = (len >> 16) & 0xFF;
        buf[3] = (len >> 8) & 0xFF;
        buf[4] = len & 0xFF;

        // 零拷贝，仅新建视图
        return buf.subarray(0, len + 5);
    }

    static async readResult(buf: ArrayBuffer): Promise<string> {
        // 1字节压缩标志 + 4字节内容长度
        const arr = new Uint8Array(buf);

        // 内容未压缩
        if (arr[0] == 0)
            return new TextDecoder('utf-8').decode(arr.subarray(5));

        // 内容压缩，解压
        // 1. 创建解压流
        const ds = new DecompressionStream('gzip');

        // 2. 把 Uint8Array 构造成可读流并 pipe 解压
        const stream = new ReadableStream({
            start(controller) {
                controller.enqueue(arr.subarray(5));
                controller.close();
            }
        });

        // 3. 管道解压
        const dec = stream.pipeThrough(ds);

        // 4. 读取为字符串
        const reader = dec.getReader();
        const decoder = new TextDecoder();
        let result = '';

        while (true) {
            const { done, value } = await reader.read();
            if (done) break;

            // 流式解码，告诉解码器：后面还有数据，先别急着收尾
            result += decoder.decode(value, { stream: true });
        }
        result += decoder.decode();
        return result;
    }
}

export default Rpc;