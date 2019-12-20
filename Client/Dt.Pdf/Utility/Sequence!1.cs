#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// The sequencs object
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    internal class Sequence<T>
    {
        private Queue<T> queue;
        private Stack<T> stack;
        private readonly SequenceType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Utility.Sequence`1" /> class.
        /// </summary>
        public Sequence()
        {
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Utility.Sequence`1" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public Sequence(SequenceType type) : this()
        {
            this.type = type;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            switch (this.type)
            {
                case SequenceType.Queue:
                    this.queue.Clear();
                    return;

                case SequenceType.Stack:
                    this.stack.Clear();
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Determines whether [contains] [the specified t].
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T t)
        {
            switch (this.type)
            {
                case SequenceType.Queue:
                    return this.queue.Contains(t);

                case SequenceType.Stack:
                    return this.stack.Contains(t);
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Ins the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void In(T obj)
        {
            switch (this.type)
            {
                case SequenceType.Queue:
                    this.queue.Enqueue(obj);
                    return;

                case SequenceType.Stack:
                    this.stack.Push(obj);
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        private void Init()
        {
            switch (this.type)
            {
                case SequenceType.Queue:
                    this.queue = new Queue<T>();
                    return;

                case SequenceType.Stack:
                    this.stack = new Stack<T>();
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Outs this instance.
        /// </summary>
        /// <returns></returns>
        public T Out()
        {
            switch (this.type)
            {
                case SequenceType.Queue:
                    return this.queue.Dequeue();

                case SequenceType.Stack:
                    return this.stack.Pop();
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                switch (this.type)
                {
                    case SequenceType.Queue:
                        return this.queue.Count;

                    case SequenceType.Stack:
                        return this.stack.Count;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}

