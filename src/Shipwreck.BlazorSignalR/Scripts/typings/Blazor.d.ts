declare interface DotNetStatic {
    invokeMethodAsync<T = any>(assemblyName: string, methodName: string, ...args: any[]): Promise<T>;
}
declare var DotNet: DotNetStatic;