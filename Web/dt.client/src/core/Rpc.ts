
class Rpc {
    private data: string;

    constructor(private serviceName: string, private methodName: string, ...params: any[]) {
        // 序列化 json RPC 调用请求
        const array: any[] = [serviceName, methodName];
        if (params && params.length > 0) {
            for (let i = 0; i < params.length; i++) {
                array.push(params[i]);
            }
        }

        this.data = JSON.stringify(array);
        console.log(this.data);
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
            xhr.onload = async () => {
                if (xhr.status == 200)
                    resolve(await Rpc.readFrame(xhr.response));
            };
            xhr.send(Rpc.writeFrame(self.data));
        });
    }

    static writeFrame(str: string): Uint8Array {
        // Frame内容
        var data = [];
        var len, c;
        len = str.length;
        for (var i = 0; i < len; i++) {
            c = str.charCodeAt(i);
            if (c >= 0x010000 && c <= 0x10FFFF) {
                data.push(((c >> 18) & 0x07) | 0xF0);
                data.push(((c >> 12) & 0x3F) | 0x80);
                data.push(((c >> 6) & 0x3F) | 0x80);
                data.push((c & 0x3F) | 0x80);
            }
            else if (c >= 0x000800 && c <= 0x00FFFF) {
                data.push(((c >> 12) & 0x0F) | 0xE0);
                data.push(((c >> 6) & 0x3F) | 0x80);
                data.push((c & 0x3F) | 0x80);
            }
            else if (c >= 0x000080 && c <= 0x0007FF) {
                data.push(((c >> 6) & 0x1F) | 0xC0);
                data.push((c & 0x3F) | 0x80);
            }
            else {
                data.push(c & 0xFF);
            }
        }

        // Frame头
        var header = [];
        // 始终不压缩
        header.push(0 & 0xFF);
        // 内容长度
        len = data.length;
        header.push((len >> 24) & 0xFF);
        header.push((len >> 16) & 0xFF);
        header.push((len >> 8) & 0xFF);
        header.push(len & 0xFF);

        return new Uint8Array(header.concat(data));
    }

    static async readFrame(buf: ArrayBuffer): Promise<string> {
        // 1字节压缩标志 + 4字节内容长度
        const arr = new Uint8Array(buf);

        // 内容未压缩
        if (arr[0] == 0)
            return new TextDecoder('utf-8').decode(arr.subarray(5));

        // 解压
        const decompressionStream = new DecompressionStream('gzip');
        const stream = new ReadableStream({
            start(controller) {
                controller.enqueue(arr.subarray(5));
                controller.close();
            }
        });

        const decompressedStream = stream.pipeThrough(decompressionStream);
        const reader = decompressedStream.getReader();
        const chunks: Uint8Array[] = [];

        while (true) {
            const { done, value } = await reader.read();
            if (done) break;
            chunks.push(value);
        }

        return new TextDecoder('utf-8').decode(chunks);
    }
}

export default Rpc;