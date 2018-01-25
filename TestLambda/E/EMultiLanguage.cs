using Spxus.Calture.Service;
using Spxus.Core.Calture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static partial class E
    {
        //多语言默认实现
        public static ICaltureService Calture { get; set; }
        public static List<SpxCaltureType> SupportCalture = new List<SpxCaltureType>()
        {
            SpxCaltureType.Chinese,
            SpxCaltureType.English,
        };

        public static Func<IEnumerable<string>, string> Text
        {
            get
            {
                return (input) =>
                {
                    var texts = input.ToList();
                    var diction = new Dictionary<int, string>();
                    for (var i = 0; i < SupportCalture.Count() && i< texts.Count; i++)
                    {
                        diction.Add((int)SupportCalture[i], texts[i]);
                    }
                    return E.Calture.Text(diction);
                };
            }
        }
        public static Func<string, string, string> CaltureCnEn
        {
            get
            {
                return (string cnText, string enText) =>
                {
                    return E.Calture.Text(new Dictionary<int, string>()
                    {
                        [(int)SpxCaltureType.Chinese] = cnText,
                        [(int)SpxCaltureType.English] = enText
                    });
                };
            }
        }

        public static void InitCalture(int calture)
        {
            Calture = new CaltureService();
            Calture.DefaultCalture = calture;
        }
    }

    public enum SpxCaltureType
    {
        Chinese = 0,
        English = 1
    }
}