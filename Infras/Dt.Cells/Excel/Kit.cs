#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2015-08-04
******************************************************************************/
#endregion

#region ��������
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// ģ�� Dt.Core.Kit �Ĳ��ֹ���
    /// </summary>
    public static class ExcelKit
    {
        static Window _mainWin;
        static Border _snapBorder;
        static MethodInfo _msg;
        static MethodInfo _warn;

        /// <summary>
        /// �����ڣ�App����ʱ���ⲿ����
        /// </summary>
        public static Window MainWin
        {
            get
            {
                if (_mainWin == null)
                    _mainWin = GetPropertyVal("MainWin") as Window;
                return _mainWin;
            }
            set { _mainWin = value; }
        }

        /// <summary>
        /// ��ͼ�õ�Border
        /// </summary>
        public static Border SnapBorder
        {
            get
            {
                if (_snapBorder == null)
                    _snapBorder = GetPropertyVal("SnapBorder") as Border;
                return _snapBorder;
            }
            set { _snapBorder = value; }
        }
        
        /// <summary>
        /// ������Ϣ��ʾ
        /// </summary>
        /// <param name="p_content">��ʾ����</param>
        /// <param name="p_delaySeconds">
        /// ������Զ��رգ�Ĭ��3��
        /// <para>����0��������ʱ���Զ��رգ����Ҳ�ر�</para>
        /// <para>0�����Զ��رգ�������ر�</para>
        /// <para>С��0��ʼ�ղ��رգ�ֻ�г�����ƹر�</para>
        /// </param>
        public static void Msg(string p_content, int p_delaySeconds = 3)
        {
            if (_msg == null)
                _msg = GetMethod("Msg");
            if (_msg != null)
                _msg.Invoke(null, new object[] { p_content, p_delaySeconds });
        }

        /// <summary>
        /// ������ʾ
        /// </summary>
        /// <param name="p_content">��ʾ����</param>
        /// <param name="p_delaySeconds">
        /// ������Զ��رգ�Ĭ��5��
        /// <para>����0��������ʱ���Զ��رգ����Ҳ�ر�</para>
        /// <para>0�����Զ��رգ�������ر�</para>
        /// <para>С��0��ʼ�ղ��رգ�ֻ�г�����ƹر�</para>
        /// </param>
        public static void Warn(string p_content, int p_delaySeconds = 5)
        {
            if (_warn == null)
                _warn = GetMethod("Warn");
            if (_warn != null)
                _warn.Invoke(null, new object[] { p_content, p_delaySeconds });
        }

        static MethodInfo GetMethod(string p_name)
        {
            var tp = Type.GetType("Dt.Core.Kit,Dt.Core");
            if (tp != null)
                return tp.GetMethod(p_name, BindingFlags.Public | BindingFlags.Static);
            return null;
        }

        static object GetPropertyVal(string p_name)
        {
            var tp = Type.GetType("Dt.Core.Kit,Dt.Core");
            if (tp != null)
            {
                var prop = tp.GetProperty(p_name, BindingFlags.Public | BindingFlags.Static);
                if (prop != null)
                    return prop.GetValue(null);
            }
            return null;
        }

        #region UI�̵߳���
        /**********************************************************************************************************************************************************/
        // ����WinUI������⣺
        //
        // 1. WinUI �� Window.Dispatcher �� DependencyObject.Dispatcher ʼ��null��ֻ��ʹ�� DispatcherQueue��
        //
        // 2. uno �� Window.DispatcherQueue δʵ�֣�RootGrid.DispatcherQueue �� Task �з���Ϊ null��ֻ��ʹ�� UWP ʱ�ķ�ʽ�� (uno4.4 ��ʵ��)
        //
        /***********************************************************************************************************************************************************/

        /// <summary>
        /// ȷ����UI�̵߳��ø�������
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunAsync(Action p_action)
        {
            // uno4.4 ��ʵ�� Window.DispatcherQueue
            var dispatcher = MainWin.DispatcherQueue;
            if (dispatcher.HasThreadAccess)
            {
                p_action();
            }
            else
            {
                dispatcher.TryEnqueue(new DispatcherQueueHandler(p_action));
            }
        }

        /// <summary>
        /// ȷ����UI�߳�ͬ�����ø�������
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunSync(Action p_action)
        {
            var dispatcher = MainWin.DispatcherQueue;
            if (dispatcher.HasThreadAccess)
            {
                p_action();
                return;
            }

            var taskSrc = new TaskCompletionSource<bool>();
            dispatcher.TryEnqueue(() =>
            {
                p_action();
                taskSrc.TrySetResult(true);
            });
            taskSrc.Task.Wait();
        }

        /// <summary>
        /// ʼ����UI�̵߳�DispatcherQueue�е��ø�����������RunAsync������ͬ
        /// <para>RunAsync����UI�̵߳ķ������ֱ�ӵ��� �� ��DispatcherQueue�е��ø�������</para>
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunInQueue(Action p_action)
        {
            // uno4.4 ��ʵ�� Window.DispatcherQueue
            MainWin.DispatcherQueue.TryEnqueue(new DispatcherQueueHandler(p_action));
        }
        #endregion
    }
}