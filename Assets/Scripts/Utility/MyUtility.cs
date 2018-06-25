using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using ZCC.GameObjectPool;

#if UNITY_WSA_10_0 && !UNITY_EDITOR
using Windows.UI.ViewManagement;
#endif

namespace ZCC {
    

    public static class Utility {

        static Utility() { }

        public static void Init() {
            //mainThreadId = Thread.CurrentThread.ManagedThreadId;
            //isDebugBuild = UnityEngine.Debug.isDebugBuild;
            //mainContext = SynchronizationContext.Current;
            //currentTotalFrame = 0;
            random = new System.Random();
            //ip = GetLocalIP();

        }

        #region 私有字段

        /// <summary>主线程ID</summary>
        private static int mainThreadId;

        /// <summary>Debug生成 供其它线程调用</summary>
        private static bool isDebugBuild;

        /// <summary>UI线程的上下文环境</summary>
        private static SynchronizationContext mainContext;

        private static long currentTotalFrame;

        private static System.Random random;

        private static string ip;

        #endregion

        #region 公有方法

        /// <summary>简化协程调用(不必写两个方法来开启一个协程)</summary>
        public static IEnumerator StartCoroutine(YieldInstruction yieldInstruction , Action action) {
            yield return yieldInstruction;
            action();
        }

        /// <summary>获取当前设置下支持的运行时版本 </summary>
        //public static ScriptingRuntimeVersion GetScriptingRuntimeVersion() {
        //    return PlayerSettings.scriptingRuntimeVersion;
        //}

        /// <summary>获取当前分辨率</summary>
        public static Vector2 GetResolution() {
#if UNITY_EDITOR
            Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetMainGameView.Invoke(null, null);
            var gameView = (UnityEditor.EditorWindow)Res;
            var prop = gameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var gvsize = prop.GetValue(gameView, new object[0] { });
            var gvSizeType = gvsize.GetType();
            int debug_h = (int)gvSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
            int debug_w = (int)gvSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
            return new Vector2(debug_w, debug_h);
#else
            return new Vector2(Screen.currentResolution.width, Screen.currentResolution.height); 
#endif
        }

        /// <summary>判断Win10平台下是否为触控</summary>
        public static bool IsWindows10UserInteractionModeTouch {
            get {
                bool isInTouchMode = false;
#if UNITY_WSA_10_0 &&!UNITY_EDITOR
                UnityEngine.WSA.Application.InvokeOnUIThread(() => { isInTouchMode = UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch; }, true); 
#endif
                return isInTouchMode; }
        }

        //该方法仅消耗3ms左右 相当于一次print 性能消耗忽略不计
        /// <summary>执行委托并获取目标代码的运行时间</summary>
        public static long GetTimeSpan(Action action) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>判断当前线程是否为UI(主)线程</summary>
        public static bool IsMainThread {
            get { return Thread.CurrentThread.ManagedThreadId == mainThreadId; }
            //get { return Environment.CurrentManagedThreadId == mainThreadId; }
        }

        /// <summary>返回当前项目是否是Debug版本</summary>
        public static bool IsDebugBuild {
            get { return isDebugBuild; }
        }

        /// <summary>游戏运行的"帧数戳"</summary>
        public static long CurrentTotalFrame {
            get { return currentTotalFrame; }
        }

        /// <summary>返回主线程的执行上下文</summary>
        public static SynchronizationContext MainContext {
            get { return mainContext; }
        }

        /// <summary>本地IP地址</summary>
        public static string IP {
            get{ return ip; }
        }

        

        /// <summary>返回待加密字符串的MD5码 </summary>
        public static string GetMd5(string str) {
            string strMd5Code = "";
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            for(int i = 0; i < bytes.Length; i++) {
                strMd5Code += bytes[i].ToString("x2");
            }
            return strMd5Code;
        }

        /// <summary>将Unix时间戳转化为时间 </summary>
        public static DateTime UnixStampToDateTime(this int timeStamp) {
            DateTime oriTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return oriTime.AddSeconds(timeStamp);
        }

        /// <summary>将时间转化为Unix时间戳 </summary>
        public static int DateTimeToUnixStamp(this DateTime time) {
            DateTime oriTime = new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Utc);
            return (int)(time - oriTime).TotalSeconds;
        }

        public static int GetRandomInt() {
            return random.Next();
        }

        /// <summary>手机号码正则匹配</summary>
        public static bool IsPhoneNumber(string number) {
            if (number.Length != 11)
                return false;
            string isChinaMobile = "^134[0-8]\\d{7}$|^(?:13[5-9]|147|15[0-27-9]|178|1703|1705|1706|18[2-478])\\d{7,8}$"; //移动  
            string isChinaUnion = "^(?:13[0-2]|145|15[56]|176|1704|1707|1708|1709|171|18[56])\\d{7,8}$"; //联通  
            string isChinaTelcom = "^(?:133|153|1700|1701|1702|177|173|18[019])\\d{7,8}$";//电信
            if(new Regex(isChinaMobile).IsMatch(number)|| new Regex(isChinaUnion).IsMatch(number)|| new Regex(isChinaTelcom).IsMatch(number)) {
                return true;
            }
            return false;
        }

        /// <summary>密码正则匹配</summary>
        public static bool IsMatchPassword(string password) {
            string pattern = @"^[0-9a-zA-Z`~!@#$%\^&*()_+-={}|\[\]:"";'<>?,.]{6,16}$";
            if (new Regex(pattern).IsMatch(password)) {
                return true;
            }
            else return false;
        }

        ///<summary>判断是否是非法字符串</summary>
        /// 包括大小写字母 数字 下划线_ 连接符-
        public static Boolean IsLegalStr(string str) {
            char[] charStr = str.ToLower().ToCharArray();
            for (int i = 0; i < charStr.Length; i++) {
                int num = Convert.ToInt32(charStr[i]);
                if (!IsChineseLetter(num) && !((num >= 48 && num <= 57) || (num >= 97 && num <= 122) || (num >= 65 && num <= 90) || num == 45 || num == 95)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>判断字符的Unicode值是否是汉字</summary>
        private static bool IsChineseLetter(int code) {
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);
            if (code >= chfrom && code <= chend) 
                return true;     //当code在中文范围内返回true
            else return false;    //当code不在中文范围内返回false
        }

        /// <summary>从"=" 和 "&" 分割的字符串中取出键值对</summary>
        public static ICollection<KeyValuePair<string, string>> GetKeyValuePairs(string str) {
            str = str.Replace("&amp;", "&");
            List<KeyValuePair<string, string>> kvps = new List<KeyValuePair<string, string>>();
            string[] spiltStr = str.Split('&');
            foreach(string s in spiltStr) {
                string[] keyValue = s.Split('=');
                if (keyValue.Length != 2)
                    throw new Exception("无法解析的非法串");
                kvps.Add(new KeyValuePair<string, string>(keyValue[0], keyValue[1]));
            }
            return kvps;

        }

        /// <summary>执行一定时间的CPU计算操作</summary>
        public static void MakeSecondOperation(float second = 1) {
            for (int count = 0; count < second*10; count++) {
                UnityEngine.Debug.Log(CurrentTotalFrame);
                long i = 0;
                for (; i < 63000000; i++) ;
            }
        }



        #endregion

        //额外的代码性能消耗几乎可以忽略不计
        /// <summary>集合类可并行任务处理多线程优化</summary>
        public static void DoMultithreadingWork<TSource>(this ICollection<TSource> source, Action<TSource> body)
            where TSource: IEquatable<TSource> {

            TSource first = source.First();
            long duration = GetTimeSpan(() => body(source.First()));
            //如果单任务时间大于4ms并且集合项数大于2时进行多线程优化(4ms已测试)
            if (duration>4&&source.Count>1)
                Parallel.ForEach(source, (child) => {
                    if (!child.Equals(first))
                        body(child);
                });
            else
                foreach (var child in source) {
                    if (!child.Equals(first))
                        body(child);
                }
        }

        /// <summary> 集合类可并行任务处理多线程优化<para> 注意: TResult若是值类型,用可空值类型替换</para> </summary>
        public static TResult DoMultithreadingWork<TSource,TResult>(this ICollection<TSource> source, Func<TSource,TResult> body)
            where TSource : IEquatable<TSource> where TResult: IEquatable<TResult> {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            TSource first = source.First();
            TResult firstResult = body(first);
            stopwatch.Stop();
            long duration = stopwatch.ElapsedMilliseconds;
            if (!firstResult.Equals(default(TResult))) {
                return firstResult;
            }
            if (duration > 4 && source.Count > 1) {
                TResult tResult = default(TResult);
                Parallel.ForEach(source, (child, loopState) => {
                    if (!child.Equals(first)) {
                        TResult result = body(child);
                        //获取到非默认返回值 终止所有任务并返回
                        if (!result.Equals(default(TResult))) {
                            tResult = result;
                            loopState.Stop();
                            return;
                        }
                    }
                });
                return tResult;
            }
            else {
                foreach (var child in source) {
                    if (!child.Equals(first)) {
                        TResult result = body(child);
                        if (!result.Equals(default(TResult))) return result;
                    }
                }
                return default(TResult);
            }
        }

        /// <summary>集合类可并行任务处理多线程优化</summary>
        public static void DoMultithreadingWork<TKey,TValue>(this ICollection<KeyValuePair<TKey, TValue>> source,Action<TKey,TValue> body)
            where TKey: IEquatable<TKey> {

            KeyValuePair<TKey, TValue> first = source.First();
            long duration = GetTimeSpan(() => body(source.First().Key,source.First().Value));
            if (duration > 4 && source.Count > 1)
                Parallel.ForEach(source, (child) => {
                    if (!child.Key.Equals(first.Key))
                        body(child.Key, child.Value);
                });
            else
                foreach (var child in source) {
                    if (!child.Key.Equals(first.Key))
                        body(child.Key, child.Value);
                }
        }

        /// <summary> 集合类可并行任务处理多线程优化<para> 注意: TResult若是值类型,用可空值类型替换</para> </summary>
        public static TResult DoMultithreadingWork<TKey, TValue, TResult>(this ICollection<KeyValuePair<TKey, TValue>> source, Func<TKey, TValue, TResult> body)
            where TKey : IEquatable<TKey> where TResult : IEquatable<TResult> {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            KeyValuePair<TKey, TValue> first = source.First();
            TResult firstResult = body(first.Key, first.Value);
            stopwatch.Stop();
            long duration = stopwatch.ElapsedMilliseconds;
            if (!firstResult.Equals(default(TResult))) {
                return firstResult;
            }
            if (duration > 4 && source.Count > 1) {
                TResult tResult = default(TResult);
                Parallel.ForEach(source, (child, loopState) => {
                    if (!child.Equals(first)) {
                        TResult result = body(child.Key, child.Value);
                        //获取到非默认返回值 终止所有任务并返回
                        if (!result.Equals(default(TResult))) {
                            tResult = result;
                            loopState.Stop();
                            return;
                        }
                    }
                });
                return tResult;
            }
            else {
                foreach (var child in source) {
                    if (!child.Equals(first)) {
                        TResult result = body(child.Key, child.Value);
                        if (!result.Equals(default(TResult))) return result;
                    }
                }
                return default(TResult);
            }
        }

        /// <summary>打印任何变量并且返回原变量 </summary>
        public static T Log<T>(this T target) {
            UnityEngine.Debug.Log(target.ToString());
            return target;
        }

        /// <summary>打印任何变量并且返回原变量 </summary>
        public static T Log<T>(this T target, string tip) {
            UnityEngine.Debug.Log(tip + ": " + target.ToString());
            return target;
        }

        /// <summary>将一个字典合并到另一个字典中(值类型字典) </summary>
        public static void Combine<T1, T2>(this Dictionary<T1, T2> sourceDic, Dictionary<T1, T2> anotherDic) where T1 : struct where T2 : struct {
            foreach (var child in anotherDic) {
                sourceDic.Add(child.Key, child.Value);
            }
        }

        public static void SetText(this InputField inputField, Text text) {
            inputField.text = text.text;
        }
        public static void SetText(this Text text, Text s_text) {
            text.text = s_text.text;
        }

        /// <summary>返回ToggleGroup中选中的Toggle</summary>
        public static Toggle GetSelectedToggle(this ToggleGroup toggleGroup) {
            //IEnumerable<Toggle> onToggles = toggleGroup.ActiveToggles().Where((toggle) => { return toggle.isOn; });
            //if (onToggles.Count() == 0)
            //    return null;
            //UnityEngine.Debug.Log(toggleGroup.ActiveToggles().First().name);
            //UnityEngine.Debug.Log(onToggles.First().name);
            return toggleGroup.ActiveToggles().First();
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach(T item in enumeration) {
                action(item);
            }
        }

        public static void SetSizeWithCurrentAnchors(this RectTransform rect, float width, float height) {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        /// <summary>设置某个浮点数的绝对值 </summary>
        public static void SetAbs(ref float value, float abs) {
            if (value == 0 && abs == 0)
                return;
            if (value == 0) {
                throw new Exception("zero is not supported\n"+value+"  "+abs);
            }
            if (abs < 0)
                return;
                //throw new Exception("abs must over zero");
            if (value < 0)
                value = -abs;
            else value = abs;
        }

        #region 私有方法

        private static string GetLocalIP() {
            try {
                foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork") {
                        return _IPAddress.ToString();
                    }
                }
            }
            catch (Exception) {

            }
            return "ERROR_LOCAL_IP"+new System.Random().Next();
        }

        /// <summary>每个update轮询 维护当前的总帧数</summary>
        private static void RefreshFrame(object sender, EventArgs eventArgs) {
            currentTotalFrame++;
        }

        private static void CheckDeltaTime(object sender, EventArgs eventArgs) {
            //UnityEngine.Debug.Log(Time.maximumDeltaTime);
            //if (Time.deltaTime > 0.33f)
            //    //建议下面打上断点排查原因
            //    UnityEngine.Debug.LogWarning("UI线程卡顿超限, 需要优化当前帧操作: 协程 算法优化 or使用线程池异步执行\n");
        }

        #endregion


    }

    public class EnumFlags : PropertyAttribute { }


}


