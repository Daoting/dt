export class Dict extends Map<string, unknown> {
  // 自定义 toString 标签，Object.prototype.toString 调用
  get [Symbol.toStringTag](): string {
    return "Dict";
  }
}
