﻿#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#endregion

namespace Dt.Mgr.Workflow
{
    public sealed partial class AlignMenu : Menu
    {
        Sketch _sketch;
        FrameworkElement _target;

        public AlignMenu(Sketch p_sketch)
        {
            InitializeComponent();
            _sketch = p_sketch;
        }

        public void OnAlign(FrameworkElement p_target)
        {
            _target = p_target;
        }

        /// <summary>
        /// 左对齐
        /// </summary>
        /// <param name="e"></param>
        void OnAlignLeft(Mi e)
        {
            double left = Canvas.GetLeft(_target);
            foreach (FrameworkElement element in _sketch.SelectedNodes)
            {
                if (element != _target)
                {
                    Canvas.SetLeft(element, left);
                }
            }
            RefreshSketch();
        }

        /// <summary>
        /// 右对齐
        /// </summary>
        /// <param name="e"></param>
        void OnAlignRight(Mi e)
        {
            double right = Canvas.GetLeft(_target) + _target.ActualWidth;
            foreach (FrameworkElement element in _sketch.SelectedNodes)
            {
                if (element != _target)
                {
                    double left = right - element.ActualWidth;
                    Canvas.SetLeft(element, left);
                }
            }
            RefreshSketch();
        }

        /// <summary>
        /// 上部对齐
        /// </summary>
        /// <param name="e"></param>
        void OnAlignTop(Mi e)
        {
            double top = Canvas.GetTop(_target);
            foreach (FrameworkElement element in _sketch.SelectedNodes)
            {
                if (element != _target)
                {
                    Canvas.SetTop(element, top);
                }
            }
            RefreshSketch();
        }

        /// <summary>
        /// 底部对齐
        /// </summary>
        /// <param name="e"></param>
        void OnAlignBottom(Mi e)
        {
            double bottom = Canvas.GetTop(_target) + _target.ActualHeight;

            foreach (FrameworkElement element in _sketch.SelectedNodes)
            {
                if (element != _target)
                {
                    double top = bottom - element.ActualHeight;
                    Canvas.SetTop(element, top);
                }
            }
            RefreshSketch();
        }

        /// <summary>
        /// 水平居中对齐
        /// </summary>
        /// <param name="e"></param>
        void OnAlignHorCenter(Mi e)
        {
            double topCenter = Canvas.GetTop(_target) + _target.ActualHeight / 2;
            foreach (FrameworkElement element in _sketch.SelectedNodes)
            {
                if (element != _target)
                {
                    Canvas.SetTop(element, topCenter - element.ActualHeight / 2);
                }
            }
            RefreshSketch();
        }

        /// <summary>
        /// 垂直居中对齐
        /// </summary>
        /// <param name="e"></param>
        void OnAlignVerCenter(Mi e)
        {
            double leftCenter = Canvas.GetLeft(_target) + _target.ActualWidth / 2;
            foreach (FrameworkElement element in _sketch.SelectedNodes)
            {
                if (element != _target)
                {
                    Canvas.SetLeft(element, leftCenter - element.ActualWidth / 2);
                }
            }
            RefreshSketch();
        }

        /// <summary>
        /// 水平间距
        /// </summary>
        /// <param name="e"></param>
        void OnHorSpace(Mi e)
        {
            double min = double.MaxValue;
            double max = 0.0;
            double nodesWidth = 0.0;
            List<SNode> Nodes = (from c in _sketch.SelectedNodes
                                 orderby Canvas.GetLeft(c)
                                 select c as SNode).ToList();
            for (int i = 0; i < Nodes.Count(); i++)
            {
                SNode node = Nodes[i] as SNode;
                double top = Canvas.GetLeft(node);
                if (min > top)
                    min = top;
                if (max < top)
                    max = top;
                if (i != _sketch.SelectedNodes.Count - 1)
                    nodesWidth += node.Width;
            }

            double totalSpace = (max - min - nodesWidth) / (_sketch.SelectedNodes.Count - 1);
            double cur = min;
            for (int i = 0; i < Nodes.Count(); i++)
            {
                SNode node = Nodes[i];
                Canvas.SetLeft(node, cur);
                cur += node.Width + totalSpace;
            }
            RefreshSketch();
        }

        /// <summary>
        /// 垂直间距
        /// </summary>
        /// <param name="e"></param>
        void OnVerSpace(Mi e)
        {
            double min = double.MaxValue;
            double max = 0.0;
            double nodesHeight = 0.0;
            List<SNode> Nodes = (from c in _sketch.SelectedNodes
                                 orderby Canvas.GetTop(c)
                                 select c as SNode).ToList();
            for (int i = 0; i < Nodes.Count(); i++)
            {
                SNode node = Nodes[i];
                double top = Canvas.GetTop(node);
                if (min > top)
                    min = top;
                if (max < top)
                    max = top;
                if (i != _sketch.SelectedNodes.Count - 1)
                    nodesHeight += node.Height;
            }

            double totalSpace = (max - min - nodesHeight) / (_sketch.SelectedNodes.Count - 1);
            double cur = min;
            for (int i = 0; i < Nodes.Count(); i++)
            {
                SNode node = Nodes[i] as SNode;
                Canvas.SetTop(node, cur);
                cur += node.Height + totalSpace;
            }
            RefreshSketch();
        }

        void OnDel(Mi e)
        {
            _sketch.DeleteSelection();
        }

        /// <summary>
        /// 刷新_sketch
        /// </summary>
        void RefreshSketch()
        {
            _sketch.SelectCurrent();
            _sketch.RefreshAllLines();
        }
    }
}
