#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Dt.Sample
{
    public static class SampleData
    {
        public static Table CreatePersonsTbl(int p_count)
        {
            Table tbl = new Table
            {
                { "id" },
                { "xm" },
                { "bh", typeof(int) },
                { "chushengrq", typeof(DateTime) },
                { "shenfenzh" },
                { "xb" },
                { "hunfou", typeof(bool) },
                { "shengao", typeof(double) },
                { "bumen" },
                { "note" },
            };

            DateTime birth = new DateTime(1950, 8, 22);
            var persons = Person.Persons;
            for (int i = 0; i < p_count; i++)
            {
                var per = persons[i % persons.Length];
                birth = birth.AddDays(10);
                tbl.AddRow(new
                {
                    id = "2202820-" + i.ToString().PadLeft(5, '0'),
                    xm = per.Xm,
                    bh = i + 1,
                    chushengrq = birth,
                    shenfenzh = per.Shenfenzh,
                    xb = per.Xb,
                    hunfou = per.Hunfou,
                    shengao = per.Shengao,
                    bumen = per.Bumen,
                    note = per.Note
                });
            }
            return tbl;
        }

        public static Nl<Person> CreatePersonsList(int p_count)
        {
            DateTime birth = new DateTime(1950, 8, 22);
            var persons = Person.Persons;
            Nl<Person> pers = new Nl<Person>();
            for (int i = 0; i < p_count; i++)
            {
                var per = persons[i % persons.Length];
                birth = birth.AddDays(10);
                var p = new Person
                {
                    ID = "2202820-" + i.ToString().PadLeft(5, '0'),
                    Xm = per.Xm,
                    Bh = i + 1,
                    Chushengrq = birth,
                    Shenfenzh = per.Shenfenzh,
                    Xb = per.Xb,
                    Hunfou = per.Hunfou,
                    Shengao = per.Shengao,
                    Bumen = per.Bumen,
                    Note = per.Note,
                };
                pers.Add(p);
            }
            return pers;
        }

        public static Nl<string> CreateXmList(int p_count)
        {
            var persons = Person.Persons;
            Nl<string> pers = new Nl<string>();
            for (int i = 0; i < p_count; i++)
            {
                var per = persons[i % persons.Length];
                pers.Add(per.Xm);
            }
            return pers;
        }
    }

    public class Person : INotifyPropertyChanged
    {
        string _xm;

        public string ID { get; set; }

        public string Xm
        {
            get { return _xm; }
            internal set
            {
                if (_xm != value)
                {
                    _xm = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Xm"));
                }
            }
        }

        public int Bh { get; set; }

        public DateTime Chushengrq { get; set; }

        public string Shenfenzh { get; set; }

        public string Xb { get; set; }

        public bool Hunfou { get; set; }

        public double Shengao { get; set; }

        public string Bumen { get; set; }

        public string Note { get; set; }

        public string Name
        {
            get { return Xm; }
        }

        static Person[] _persons;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Person[] Persons
        {
            get
            {
                if (_persons == null)
                {
                    _persons = new Person[20];
                    _persons[0] = new Person
                    {
                        Xm = "李全亮",
                        Shenfenzh = "22010419790901381x",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.71,
                        Bumen = "肾内科二",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };

                    _persons[1] = new Person
                    {
                        Xm = "杨乐",
                        Shenfenzh = "220122800511811",
                        Xb = "男",
                        Hunfou = false,
                        Shengao = 1.78,
                        Bumen = "循环门诊",
                        Note = "干好了，总结经验",
                    };
                    _persons[2] = new Person
                    {
                        Xm = "任艳莉",
                        Shenfenzh = "612132780702122",
                        Xb = "女",
                        Hunfou = true,
                        Shengao = 1.54,
                        Bumen = "内分泌门诊",
                        Note = "可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数。",
                    };
                    _persons[3] = new Person
                    {
                        Xm = "潘洋",
                        Shenfenzh = "220103197912302325",
                        Xb = "女",
                        Hunfou = false,
                        Shengao = 1.65,
                        Bumen = "内分泌门诊",
                        Note = "可动态设置",
                    };
                    _persons[4] = new Person
                    {
                        Xm = "李妍",
                        Shenfenzh = "220104197806268625",
                        Xb = "男",
                        Hunfou = false,
                        Shengao = 1.76,
                        Bumen = "肿瘤介入外科",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };
                    _persons[5] = new Person
                    {
                        Xm = "尚涛",
                        Shenfenzh = "220802198008112415",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.73,
                        Bumen = "干一科",
                        Note = "可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数。",
                    };
                    _persons[6] = new Person
                    {
                        Xm = "毕磊",
                        Shenfenzh = "22010419790117263x",
                        Xb = "男",
                        Hunfou = false,
                        Shengao = 1.79,
                        Bumen = "干三科",
                        Note = "",
                    };
                    _persons[7] = new Person
                    {
                        Xm = "吕康博",
                        Shenfenzh = "220702197804201419",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.68,
                        Bumen = "干三科",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };
                    _persons[8] = new Person
                    {
                        Xm = "田野",
                        Shenfenzh = "220105197801300012",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.71,
                        Bumen = "医务科",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };
                    _persons[9] = new Person
                    {
                        Xm = "王闯",
                        Shenfenzh = "220204197909040936",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.72,
                        Bumen = "特诊科",
                        Note = "可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数。",
                    };
                    _persons[10] = new Person
                    {
                        Xm = "朱龙有",
                        Shenfenzh = "22018219791116021x",
                        Xb = "男",
                        Hunfou = false,
                        Shengao = 1.81,
                        Bumen = "干部病房放射线",
                        Note = "可动态设置",
                    };
                    _persons[11] = new Person
                    {
                        Xm = "陈立涛",
                        Shenfenzh = "220882198002245016",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.83,
                        Bumen = "呼吸门诊",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };
                    _persons[12] = new Person
                    {
                        Xm = "宋国巍",
                        Shenfenzh = "220103790414211",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.71,
                        Bumen = "循环门诊",
                        Note = "可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数。",
                    };
                    _persons[13] = new Person
                    {
                        Xm = "隋航",
                        Shenfenzh = "22010319780321211X",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.77,
                        Bumen = "肾内科二",
                        Note = "可动态设置",
                    };
                    _persons[14] = new Person
                    {
                        Xm = "李媛媛",
                        Shenfenzh = "220104198109138424",
                        Xb = "女",
                        Hunfou = true,
                        Shengao = 1.68,
                        Bumen = "肾内科二",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };
                    _persons[15] = new Person
                    {
                        Xm = "杨波",
                        Shenfenzh = "220204197708080915",
                        Xb = "男",
                        Hunfou = false,
                        Shengao = 1.70,
                        Bumen = "肾内科二",
                        Note = "可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数。",
                    };
                    _persons[16] = new Person
                    {
                        Xm = "彭宁",
                        Shenfenzh = "220120197109130248",
                        Xb = "女",
                        Hunfou = true,
                        Shengao = 1.69,
                        Bumen = "内分泌二科",
                        Note = "可动态设置",
                    };
                    _persons[17] = new Person
                    {
                        Xm = "李亚平",
                        Shenfenzh = "220104196107295224",
                        Xb = "女",
                        Hunfou = true,
                        Shengao = 1.60,
                        Bumen = "透析室",
                        Note = "不管你的工作情况如何，写述职报告时一定要写出你的信心。干好了，总结经验;干得不好，找出问题，分析原因，制定切实可行的对策，树立来年能做好此项工作的信心",
                    };
                    _persons[18] = new Person
                    {
                        Xm = "马栋",
                        Shenfenzh = "222303195908170212",
                        Xb = "男",
                        Hunfou = true,
                        Shengao = 1.71,
                        Bumen = "皮肤科",
                        Note = "可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数。",
                    };
                    _persons[19] = new Person
                    {
                        Xm = "闻慧英",
                        Shenfenzh = "220322196205160028",
                        Xb = "女",
                        Hunfou = true,
                        Shengao = 1.73,
                        Bumen = "综合外科",
                        Note = "可动态设置",
                    };

                }
                return _persons;
            }
        }

        public override string ToString()
        {
            return $"{Xm} {Xb} {Bumen}";
        }
    }
}