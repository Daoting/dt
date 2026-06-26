import Rpc from './Rpc.ts'

export default class Kit {
    /**
     * 调用服务API
     * @param p_serviceName 服务名称
     * @param p_methodName 方法名
     * @param p_params 参数列表
     * @returns 远程调用结果
     */
    public static rpc<T>(
        p_serviceName: string,
        p_methodName: string,
        ...p_params: unknown[]
    ): Promise<T> {
        return new Rpc(
            p_serviceName,
            p_methodName,
            ...p_params
        ).call<T>();
    }

}