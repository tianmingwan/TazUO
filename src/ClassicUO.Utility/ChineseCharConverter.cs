using System.Collections.Generic;
using System.Text;

namespace ClassicUO.Utility
{
    public static class ChineseCharConverter
    {
        private static readonly Dictionary<char, char> _traditionalToSimplified = new();

        static ChineseCharConverter()
        {
            // Traditional → Simplified character pairs
            // Parallel strings: characters at same index in both strings are pairs
            string trad = "萬與醜專業叢東絲丟兩嚴喪個爿豐臨為麗舉麼義烏樂喬習鄉書買亂爭虧亙亞產畝親億儀價優儲兒兌內兩冊寫軍塚準鳳憑擊劃劇劉則創動務勢勛勵勸勻匯匱區協單卻厭厲參叢口臺葉嘆嗎嘩嘮嘯囑壓壞壟壯壺處備復夠夢夥夾奪獎嬌孫學寧實審寫寬寶對尋導層屬崗峽島峴嵐幣帥師帳帶幫幹幾廣廳廢廚廟廠彎張強彈彌彎彙後徑從復徵衝衛製複見觀覽討讓訓記設訪評診詞試詩誠話該詳語誤說請論調談謀謂謝識議變讓豈財責貨費資賈賓賞賢賣質賽購車軍軌軒軟軸較載輔輕輝輩輪輯輸轉轟這連進遊運過達違遙還邊邏鄉鄭鄰醫釋鈣鐵銅鋼銀錢鎖鎮鏡鐵鑑鑒針釣釘約級紀紅納純紐紙紋絲組細終結給統絲經綠維綜合網線編緩練總縱繪繼續纖缺罰義習聯聰職聽肅腸膚膠膽膩腦腫腳臉臂舉艱蘇蘭處虛號虧蟲蠟術衝衛製複見觀覽計訟許註詐評詛詞該詳認語誠誤說誰課誼調談請論諍謀謂諜講謝證識議護譽讀變讓貝貞負財貢貧貨販貪貫責貯貴買貸費貼貿賀資賈賓賞賢賣質賽購贅贈贊車軍軒軟軸較載輔輕輝輩輪輯輸轉轟轎這連進遊運過達違遙遼還邊邏鄉鄭鄰鄧醫釋鑑鑒針釘釣釵鈣鈉鈍鈞鈴鈷鐵鉑鉛鉚鉤鉦鉬鉲銀銅銃銑銓銖銘銚銜銠銥錢鋼錫錘錦錨錯鎖鎮鏡鏈鏢鑽門閃閉閒間開閘閡閣閥閨閱闌闔關闡隊階陽陰陳際障隨險隱隸隻雙雜雞難電雲霧靜須項順預頑頓頒頌預領頭頻題額顏願類顧顯風颱颶飄飛食飢飯飲餅養餃餉餓餘館馬馭馮馱馳駛駝駟駐駑駕駭駱騁騎騙騖騰騮騸驅驟驗驚魚魯鮑鮮鳥鴨鴕鴛鴻鴿鵪鵲鵬鶴鷗鹽鹼麥黃黑點黨齊齒齡龍龜體髮鬆鬍鬥鬧鬱麼麽著僕兇準裡餘禦籲誌範穀颳週昇佔佈捨採鬆闆闢困慾徵衝闔瞭乾幹藉係";
            string simp = "万与丑专业丛东丝丢两严丧个丬丰临为丽举么义乌乐乔习乡书买乱争亏亘亚产亩亲亿仪价优储儿兑内两册写军冢准凤凭击划剧刘则创动务势勋励劝匀汇匮区协单却厌厉参丛口台叶叹吗哗唠啸嘱压坏垄壮壶处备复够梦伙夹夺奖娇孙学宁实审写宽宝对寻导层属岗峡岛岘岚币帅师帐带帮干几广厅废厨庙厂弯张强弹弥弯汇后径从复征冲卫制复见观览讨让训记设访评诊词试诗诚话该详语误说请论调谈谋谓谢识议变让岂财责货费资贾宾赏贤卖质赛购车军轨轩软轴较载辅轻辉辈轮辑输转轰这连进游运过达违遥还边逻乡郑邻医释钙铁铜钢银钱锁镇镜铁鉴鉴针钓钉约级纪红纳纯纽纸纹丝组细终结给统丝经绿维综合网线编缓练总纵绘继续纤缺罚义习联聪职听肃肠肤胶胆腻脑肿脚脸臂举艰苏兰处虚号亏虫蜡术冲卫制复见观览计讼许注册诈评诅词该详认语诚误说谁课谊调谈请论诤谋谓谍讲谢证识议护誉读变让贝贞负财贡贫货贩贪贯责贮贵买贷费贴贸贺资贾宾赏贤卖质赛购赘赠赞车军轩软轴较载辅轻辉辈轮辑输转轰轿这连进游运过达违遥辽还边逻乡郑邻邓医释鉴鉴针钉钓钗钙钠钝钧铃钴铁铂铅铆钩钲钼钶银铜铳铣铨铢铭铫衔铑铱钱钢锡锤锦锚错锁镇镜链镖钻门闪闭闲间开闸阂阁阀闺阅阑阖关阐队阶阳阴陈际障随险隐隶只双杂鸡难电云雾静须项顺预顽顿颁颂预领头频题额颜愿类顾显风台飓飘飞食饥饭饮饼养饺饷饿余馆马驭冯驮驰驶驼驷驻驽驾骇骆骋骑骗骛腾骝骟驱骤验惊鱼鲁鲍鲜鸟鸭鸵鸳鸿鸽鹌鹊鹏鹤鸥盐碱麦黄黑点党齐齿龄龙龟体发松胡斗闹郁么么着仆凶准里余御吁志范谷刮周升占布舍采松板辟困欲征冲阖了干干借系";

            for (int i = 0; i < trad.Length && i < simp.Length; i++)
                _traditionalToSimplified[trad[i]] = simp[i];
        }

        public static string TraditionalToSimplified(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                sb.Append(_traditionalToSimplified.TryGetValue(c, out char simplified) ? simplified : c);
            }
            return sb.ToString();
        }
    }
}
