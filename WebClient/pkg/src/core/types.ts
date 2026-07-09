export class Dict extends Map<string, unknown> {
  // 自定义 toString 标签，Object.prototype.toString 调用
  get [Symbol.toStringTag](): string {
    return "Dict";
  }
}

/**
 * 信件信息
 */
export class LetterInfo {
  /**
   * 信息标识
   */
  id: string;

  /**
   * 发送者标识
   */
  senderId: number;

  /**
   * 发送者名称
   */
  senderName: string;

  /**
   * 内容类型
   */
  letterType: number;

  /**
   * 内容
   */
  content: string;

  /**
   * 发送时间
   */
  sendTime: Date;

  constructor(
    id: string,
    senderId: number,
    senderName: string,
    letterType: number,
    content: string,
    sendTime: Date
  ) {
    this.id = id;
    this.senderId = senderId;
    this.senderName = senderName;
    this.letterType = letterType;
    this.content = content;
    this.sendTime = sendTime;
  }
}